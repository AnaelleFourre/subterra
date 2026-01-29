using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask ladderMask;
    [SerializeField] private InputActionAsset InputActions;
    [SerializeField] private float WalkingSpeed = 5;
    [SerializeField] private float ClimbingSpeed = 3.5f;
    [SerializeField] private float JumpForce = 5;
    [SerializeField] private AudioClip jumpClip;
    float delay = 0.35f; // Délai en secondes
    [SerializeField] private AudioClip iceClip;
    [SerializeField] private float MovementForce = 10f;
    [SerializeField] private float MaxSpeed = 10f;
    [SerializeField] private float RotationSpeed = 10f;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction turnAction;
    private Vector2 moveAmount;
    private Rigidbody rigidBody;
    private Transform feetTransform;
    private Transform cameraTarget; // used rotate player model without moving camera
    private Rope rope;
    private Vector3 currentVelocity;
    public LayerMask groundLayer;

    private bool isGrounded;
    private float soundTimer = 0f;
    private bool isClimbing;
    private bool lockAnimation = false;
    private MovingPlatform currentPlatform;

    // ANIMATION ADD
    private Animator animator;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var map = InputActions.FindActionMap("Player", true);
        if (!map.enabled)
        {
            map.Enable();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ladder") && !isClimbing)
            GetOnLadder();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ladder") && isClimbing)
            GetOffLadder();
    }

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        turnAction = InputSystem.actions.FindAction("Turn");
        jumpAction = InputSystem.actions.FindAction("Jump");
        rigidBody = GetComponent<Rigidbody>();
        feetTransform = gameObject.transform.GetChild(0);
        cameraTarget = transform.parent.GetChild(1);
        cameraTarget.SetPositionAndRotation(transform.position, transform.rotation);
        rope = GetComponentInChildren<Rope>();
        isClimbing = false;

        // ANIMATION ADD
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        moveAmount = moveAction.ReadValue<Vector2>();
        if (jumpAction.WasPressedThisFrame())
        {
            if (isClimbing)
                JumpOffLadder();
            else if (CanJump())
                Jump();
                PlayJumpSoundWithDelay();
        }
        if (turnAction.WasPressedThisFrame())
        {
            Turn();
        }
    }

    private void FixedUpdate()
    {
        if (currentPlatform != null)
        {
            rigidBody.position += currentPlatform.PlatformVelocity * Time.fixedDeltaTime;
        }
        if (rope.enabled)
            EnforceRopeConstraint();

        if (isClimbing)
        {
            if (moveAmount != Vector2.zero)
                Climb();
            // climbing down and feet out of LadderCollider => get off ladder
            if (moveAmount.y < 0f && Physics.OverlapSphere(feetTransform.position, 0.1f, ladderMask).Length == 0)
                GetOffLadder();
        }
        else
        {
            Walk();
        }

        // In FixedUpdate => cameraTarget follows rigidBody even while jumping or falling
        cameraTarget.position = transform.position;

        // ANIMATION ADD
        if (animator != null && !lockAnimation)
        {
            Vector3 horizontalVelocity = new Vector3(
                rigidBody.linearVelocity.x,
                0f,
                rigidBody.linearVelocity.z
            );
            animator.SetFloat("Speed", horizontalVelocity.magnitude);
        }
    }

    private void GetOnLadder()
    {
        rigidBody.linearVelocity = Vector3.zero; // Reset velocity to prevent player from sliding
        rigidBody.useGravity = false;
        isClimbing = true;
    }

    private void GetOffLadder()
    {
        const float LadderExitSpeed = 5f;
        var pushDirection = (moveAmount.y * cameraTarget.forward).normalized;
        rigidBody.AddForce(LadderExitSpeed * pushDirection, ForceMode.VelocityChange);
        rigidBody.useGravity = true;
        isClimbing = false;
    }

    private bool CanJump()
    {
        return Physics.OverlapSphere(feetTransform.position, 0.1f, playerMask).Length != 0;
    }

    private void Jump()
    {
        rigidBody.AddForce(JumpForce * Vector3.up, ForceMode.VelocityChange);
    }
    void PlayJumpSoundWithDelay()
    {
        Invoke("PlayJumpSound", delay);
    }
    private void PlayJumpSound()
    {
        SoundFXManager.instance.PlaySoundFXClip(jumpClip, transform, 0.8f);
    }
    private void JumpOffLadder() => rigidBody.AddForce(JumpForce * -cameraTarget.forward, ForceMode.VelocityChange);

    private void Turn()
    {
        var lookAmount = turnAction.ReadValue<Vector2>();
        if (Mathf.Abs(lookAmount.x) > 0.1f) // Seuil pour éviter les micro-mouvements
        {
            float angleDegrees = Mathf.Sign(lookAmount.x) * 90f;
            cameraTarget.Rotate(0f, angleDegrees, 0f);

            var targetRotation = cameraTarget.rotation;
            rigidBody.MoveRotation(Quaternion.Slerp(rigidBody.rotation, targetRotation, RotationSpeed * Time.fixedDeltaTime));
        }
    }

    // If we're stepping on ice then return an IcyFloor.Grip, else return -1
    private float GetIcyFloorGrip()
    {
        var hitColliders = Physics.OverlapSphere(feetTransform.position, 0.2f, playerMask, QueryTriggerInteraction.Ignore);
        foreach (var hitCollider in hitColliders)
            if (hitCollider.TryGetComponent<IcyFloor>(out var icyFloor))
                return icyFloor.Grip;
        return -1f;
    }

    private void Walk()
    {
        var direction = (moveAmount.y * cameraTarget.forward + moveAmount.x * cameraTarget.right).normalized;

        var floorGrip = GetIcyFloorGrip();
        var gripSpeed = floorGrip == -1f ? 1f : floorGrip;

        if (soundTimer > 0)
        {
            soundTimer -= Time.deltaTime;
        }

        if (gripSpeed < 1f)
        {
            isGrounded = Physics.CheckSphere(feetTransform.position, 0.5f, groundLayer); // radius of 0.2f
            if (isGrounded && soundTimer <= 0)
            {
                SoundFXManager.instance.PlaySoundFXClip(iceClip, transform, 1f);
                soundTimer = iceClip.length;
            }
        }

        var targetVelocity = direction * WalkingSpeed;
        var currentHorizontalVelocity = new Vector3(rigidBody.linearVelocity.x, 0f, rigidBody.linearVelocity.z);
        Vector3 force;

        if (gripSpeed < 1f && direction == Vector3.zero)
        {
            force = Vector3.zero; //glisse
        }
        else
        {
            var velocityDiff = targetVelocity - currentHorizontalVelocity;
            var massScale = (rope != null && rope.enabled && !CanJump()) ? 1f : rigidBody.mass;
            force = gripSpeed * massScale * MovementForce * velocityDiff;
        }

        rigidBody.AddForce(force, ForceMode.Force);

        // Clamp max speed
        var clampedVelocity = Vector3.ClampMagnitude(currentHorizontalVelocity, MaxSpeed);
        rigidBody.linearVelocity = new Vector3(clampedVelocity.x, rigidBody.linearVelocity.y, clampedVelocity.z);

        // Only rotate when there's input
        if (direction != Vector3.zero)
        {
            var targetRotation = Quaternion.LookRotation(direction);
            rigidBody.MoveRotation(Quaternion.Slerp(rigidBody.rotation, targetRotation, RotationSpeed * Time.fixedDeltaTime));
        }
    }

    private void Climb()
    {
        // Move
        var direction = (moveAmount.y * cameraTarget.up + moveAmount.x * cameraTarget.right).normalized;
        var deltaPosition = ClimbingSpeed * Time.deltaTime * direction;
        rigidBody.MovePosition(rigidBody.position + deltaPosition);
    }


    private void EnforceRopeConstraint()
    {
        var fromAnchor = rope.getRopeStart() - rope.getGrapplePoint();
        var dist = fromAnchor.magnitude;
        var maxLength = rope.getRopeLength() + 0.5f;

        if (dist > maxLength)
        {
            rigidBody.position = rope.getGrapplePoint() + fromAnchor.normalized * maxLength;

            var v = rigidBody.linearVelocity;
            var outwardSpeed = Vector3.Dot(v, fromAnchor.normalized);

            if (outwardSpeed > 0f)
            {
                rigidBody.linearVelocity = v - fromAnchor.normalized * outwardSpeed;
            }
        }
    }

    public void LockWalkAnimation()
    {
        lockAnimation = true;
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
        }
        moveAmount = Vector2.zero;
        rigidBody.isKinematic = true;
    }

    public void UnlockWalkAnimation()
    {
        lockAnimation = false;
        rigidBody.isKinematic = false;
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out MovingPlatform platform))
        {
            currentPlatform = platform;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out MovingPlatform platform))
        {
            if (currentPlatform == platform)
                currentPlatform = null;
        }
    }

}
