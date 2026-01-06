using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private InputActionAsset InputActions;
    [SerializeField] private float WalkSpeed = 5;
    [SerializeField] private float ClimbingSpeed = 3.5f;
    [SerializeField] private float JumpForce = 5;
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
        if (other.gameObject.tag == "Ladder")
        {
            isClimbing = true;
            rigidBody.useGravity = false;
            Debug.Log("Climbing ladder");
        }
    }
    void OnTriggerExit(Collider other)
    {

        if (other.gameObject.tag == "Ladder")
        {
            isClimbing = false;
            rigidBody.useGravity = true;
            Debug.Log("Exited ladder");
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
        cameraTarget.position = transform.position;
        cameraTarget.rotation = transform.rotation;
        rope = GetComponentInChildren<Rope>();
        isClimbing = false;
    }

    private void Update()
    {
        moveAmount = moveAction.ReadValue<Vector2>();
        if (jumpAction.WasPressedThisFrame() && CanJump())
        {
            Jump();
        }
        if (turnAction.WasPressedThisFrame())
        {
            Turn();
        }
    }

    private void FixedUpdate()
    {
        if (rope != null && rope.enabled)
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
        // Put this in FixedUpdate so that cameraTarget follows rigidBody even while jumping / falling
        cameraTarget.position = transform.position;
    }

    private bool CanJump()
    {
        return Physics.OverlapSphere(feetTransform.position, 0.1f, playerMask).Length != 0;
    }

    private void Jump()
    {
        rigidBody.AddForceAtPosition(JumpForce * Vector3.up, Vector3.zero, ForceMode.Impulse);
    }

    private void Turn()
    {
        var lookAmount = turnAction.ReadValue<Vector2>();
        var angleDegrees = lookAmount.x * 90f;
        cameraTarget.Rotate(0f, angleDegrees, 0f);
        rigidBody.MoveRotation(cameraTarget.rotation);
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
        var targetVelocity = direction * WalkSpeed; // can be Vector3.zero if moveAmount is Vector2.zero
        var floorGrip = GetIcyFloorGrip();
        if (floorGrip == -1f)
        {
            // Not on icy floor
            currentVelocity = targetVelocity;
        }
        else
        {
            // On icy floor
            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, floorGrip * Time.fixedDeltaTime);
        }
        if (currentVelocity.magnitude > 0.01f)
        {
            // Move
            var deltaPosition = currentVelocity * Time.fixedDeltaTime;
            rigidBody.MovePosition(rigidBody.position + deltaPosition);
            // Rotate
            if (direction != Vector3.zero)
            {
                var deltaRotation = direction == -transform.forward ? Quaternion.Euler(0f, 180f, 0f)
                                                                    : Quaternion.FromToRotation(transform.forward, direction);
                rigidBody.MoveRotation(rigidBody.rotation * deltaRotation);
            }
        }
    }

    private void Climb()
    {
        // Move
        var direction = (moveAmount.y * cameraTarget.up + moveAmount.x * cameraTarget.right).normalized;
        var deltaPosition = direction * ClimbingSpeed * Time.deltaTime;
        rigidBody.MovePosition(rigidBody.position + deltaPosition);
    }

    private void EnforceRopeConstraint()
    {
        Vector3 fromAnchor = rigidBody.position - rope.getGrapplePoint();
        float dist = fromAnchor.magnitude;
        float maxLength = rope.getRopeLength() + 0.5f;

        if (dist > maxLength)
        {
            rigidBody.position = rope.getGrapplePoint() + fromAnchor.normalized * maxLength;

            Vector3 v = rigidBody.linearVelocity;
            float outwardSpeed = Vector3.Dot(v, fromAnchor.normalized);

            if (outwardSpeed > 0f)
            {
                rigidBody.linearVelocity = v - fromAnchor.normalized * outwardSpeed;
            }
        }
    }

}
