using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private InputActionAsset InputActions;
    [SerializeField] private float WalkSpeed = 5;
    [SerializeField] private float JumpForce = 5;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction turnAction;
    private Vector2 moveAmount;
    private Rigidbody rigidBody;
    private Transform feetTransform;
    private Transform cameraTarget; // used rotate player model without moving camera

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
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
        if (moveAmount != Vector2.zero)
        {
            Walk();
        }
    }

    private bool canJump()
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

    private void Walk()
    {
        // Move
        var direction = (moveAmount.y * cameraTarget.forward + moveAmount.x * cameraTarget.right).normalized;
        var deltaPosition = direction * WalkSpeed * Time.deltaTime;
        rigidBody.MovePosition(rigidBody.position + deltaPosition);
        cameraTarget.position = transform.position;
        // Rotate
        var deltaRotation = direction == -transform.forward ? Quaternion.Euler(0f, 180f, 0f)
                                                            : Quaternion.FromToRotation(transform.forward, direction);
        rigidBody.MoveRotation(rigidBody.rotation * deltaRotation);
    }
}
