using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private InputActionAsset InputActions;
    [SerializeField] private float WalkingSpeed = 5;
    [SerializeField] private float ClimbingSpeed = 3.5f;
    [SerializeField] private float JumpForce = 5;
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
    private bool isClimbing;

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
        if (other.gameObject.CompareTag("Ladder"))
        {
            rigidBody.linearVelocity = Vector3.zero; // Reset velocity to prevent player from sliding
            rigidBody.useGravity = false;
            isClimbing = true;
        }
    }
    void OnTriggerExit(Collider other)
    {

        if (other.gameObject.CompareTag("Ladder"))
        {
            const float LadderExitSpeed = 5f;
            var pushDirection = (moveAmount.y * cameraTarget.forward).normalized;
            rigidBody.AddForce(LadderExitSpeed * pushDirection, ForceMode.VelocityChange);
            rigidBody.useGravity = true;
            isClimbing = false;
        }
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
        }
        if (turnAction.WasPressedThisFrame())
        {
            Turn();
        }
    }

    private void FixedUpdate()
    {
        if (rope.enabled)
            EnforceRopeConstraint();

        if (isClimbing)
        {
            if (moveAmount != Vector2.zero)
                Climb();
        }
        else
        {
            Walk();
        }

        // In FixedUpdate => cameraTarget follows rigidBody even while jumping or falling
        cameraTarget.position = transform.position;

        // ANIMATION ADD
        if (animator != null)
        {
            animator.SetFloat("Speed", currentVelocity.magnitude);
        }
    }

    private bool CanJump()
    {
        return Physics.OverlapSphere(feetTransform.position, 0.1f, playerMask).Length != 0;
    }

    private void Jump() => rigidBody.AddForce(JumpForce * Vector3.up, ForceMode.VelocityChange);

    private void JumpOffLadder() => rigidBody.AddForce(JumpForce * -cameraTarget.forward, ForceMode.VelocityChange);

    private void Turn()
    {
        var lookAmount = turnAction.ReadValue<Vector2>();
        var angleDegrees = lookAmount.x * 90f;
        cameraTarget.Rotate(0f, angleDegrees, 0f);

        var targetRotation = cameraTarget.rotation;
        rigidBody.MoveRotation(Quaternion.Slerp(rigidBody.rotation, targetRotation, RotationSpeed * Time.fixedDeltaTime));
    }

    // If we're stepping on ice then return an IcyFloor.Grip, else return -1
    private float GetIcyFloorGrip()
    {
        var hitColliders = Physics.OverlapSphere(feetTransform.position, 0.1f, playerMask, QueryTriggerInteraction.Ignore);
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

        var targetVelocity = direction * WalkingSpeed;
        var currentHorizontalVelocity = new Vector3(rigidBody.linearVelocity.x, 0f, rigidBody.linearVelocity.z);
        var velocityDiff = targetVelocity - currentHorizontalVelocity;

        // The player should be heavier when swinging on the rope for better control
        var massScale = (rope != null && rope.enabled && !CanJump()) ? 1f : rigidBody.mass;
        var force = gripSpeed * massScale * MovementForce * velocityDiff;
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

    public void ResetAnimationSpeed()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
        }
    }
}
