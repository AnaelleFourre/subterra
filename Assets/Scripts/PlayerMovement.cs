using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    private Rope rope;
    private Vector3 currentVelocity;

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
        if (rope.enabled)
            EnforceRopeConstraint();

        // if (moveAmount != Vector2.zero)
        // {
        //     Walk();
        // }
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
        rigidBody.AddForceAtPosition(JumpForce * Vector3.up, Vector3.zero, ForceMode.Impulse);
    }

    private void Turn()
    {
        var lookAmount = turnAction.ReadValue<Vector2>();
        var angleDegrees = lookAmount.x * 90f;
        cameraTarget.Rotate(0f, angleDegrees, 0f);
        rigidBody.MoveRotation(cameraTarget.rotation);
    }

    // private void Walk()
    // {
    //     // Move
    //     var direction = (moveAmount.y * cameraTarget.forward + moveAmount.x * cameraTarget.right).normalized;
    //     var deltaPosition = direction * WalkSpeed * Time.deltaTime;
    //         rigidBody.MovePosition(rigidBody.position + deltaPosition);
    //     // Rotate
    //     var deltaRotation = direction == -transform.forward ? Quaternion.Euler(0f, 180f, 0f)
    //                                                         : Quaternion.FromToRotation(transform.forward, direction);
    //     rigidBody.MoveRotation(rigidBody.rotation * deltaRotation);
    // }

    private void Walk()
    {
        var direction = (moveAmount.y * cameraTarget.forward + moveAmount.x * cameraTarget.right).normalized;
        var targetVelocity = direction * WalkSpeed;
        float gripSpeed = 50f;
        
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
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, gripSpeed * Time.fixedDeltaTime);
        if (currentVelocity.magnitude > 0.01f)
        {
            rigidBody.MovePosition(rigidBody.position + currentVelocity * Time.deltaTime);
            if (direction != Vector3.zero)
            {
                var deltaRotation = direction == -transform.forward ? Quaternion.Euler(0f, 180f, 0f)
                                                                    : Quaternion.FromToRotation(transform.forward, direction);
                rigidBody.MoveRotation(rigidBody.rotation * deltaRotation);
            }
        }
    }
    
    private void EnforceRopeConstraint()
    {
        Vector3 fromAnchor = rigidBody.position - rope.getGrapplePoint();
        float dist = fromAnchor.magnitude;
        float maxLength = rope.getRopeLength();

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
