using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private InputActionAsset InputActions;
    [SerializeField] private float WalkSpeed = 5;
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
    }

    private void Update()
    {
        moveAmount = moveAction.ReadValue<Vector2>();
        if (jumpAction.WasPressedThisFrame() && canJump())
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

        Walk();
        // Put this in FixedUpdate so that cameraTarget follows rigidBody even while jumping / falling
        cameraTarget.position = transform.position;
    }

    private bool canJump()
    {
        return Physics.OverlapSphere(feetTransform.position, 0.1f, playerMask).Length != 0;
    }

    private void Jump()
    {
        rigidBody.AddForce(JumpForce * Vector3.up, ForceMode.Impulse);
    }

    private void Turn()
    {
        var lookAmount = turnAction.ReadValue<Vector2>();
        var angleDegrees = lookAmount.x * 90f;
        cameraTarget.Rotate(0f, angleDegrees, 0f);
        
        Quaternion targetRotation = cameraTarget.rotation;
        rigidBody.MoveRotation(Quaternion.Slerp(rigidBody.rotation, targetRotation, RotationSpeed * Time.fixedDeltaTime));
    }

    private void Walk()
    {
        var direction = (moveAmount.y * cameraTarget.forward + moveAmount.x * cameraTarget.right).normalized;
        float gripSpeed = 1.0f;

        if (Physics.OverlapSphere(feetTransform.position, 0.1f, playerMask, QueryTriggerInteraction.Ignore) is Collider[] hits)
        {
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<IcyFloor>(out var icyFloor))
                {
                    gripSpeed = icyFloor.Grip;
                    break;
                }
            }
        }

        Vector3 targetVelocity = direction * WalkSpeed;
        Vector3 currentHorizontalVelocity = new Vector3(rigidBody.linearVelocity.x, 0f, rigidBody.linearVelocity.z);
        Vector3 velocityDiff = targetVelocity - currentHorizontalVelocity;
        
        Vector3 force = velocityDiff * MovementForce * gripSpeed;
        rigidBody.AddForce(force, ForceMode.Force);

        // Clamp max speed
        Vector3 clampedVelocity = Vector3.ClampMagnitude(currentHorizontalVelocity, MaxSpeed);
        rigidBody.linearVelocity = new Vector3(clampedVelocity.x, rigidBody.linearVelocity.y, clampedVelocity.z);

        // Only rotate when there's input
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rigidBody.MoveRotation(Quaternion.Slerp(rigidBody.rotation, targetRotation, RotationSpeed * Time.fixedDeltaTime));
        }
    }

    private void EnforceRopeConstraint()
    {
        Vector3 fromAnchor = rigidBody.position - rope.getGrapplePoint();
        float dist = fromAnchor.magnitude;
        float maxLength = rope.getRopeLength()+0.5f;

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
