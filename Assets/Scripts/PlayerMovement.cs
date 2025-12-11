using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private InputActionAsset InputActions;
    [SerializeField] private float WalkSpeed = 5;
    [SerializeField] private float JumpForce = 5;

    public bool followPlayer = true; 

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction turnAction;
    private Vector2 moveAmount;
    private Rigidbody rigidBody;
    private Transform feetTransform;

    public Transform cameraTarget;
    
    // Platform tracking
    private Transform currentPlatform;
    private Vector3 lastPlatformPosition;

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
        Vector3 totalMovement = Vector3.zero;
        
        // Move player with platform
        if (currentPlatform != null)
        {
            Vector3 platformDelta = currentPlatform.position - lastPlatformPosition;
            totalMovement += platformDelta;
            lastPlatformPosition = currentPlatform.position;
        }
        
        // Add player movement
        if (moveAmount != Vector2.zero)
        {
            var direction = (moveAmount.y * cameraTarget.forward + moveAmount.x * cameraTarget.right).normalized;
            totalMovement += direction * WalkSpeed * Time.deltaTime;
            
            // Handle rotation
            var deltaRotation = direction == -transform.forward ? Quaternion.Euler(0f, 180f, 0f)
                                                                : Quaternion.FromToRotation(transform.forward, direction);
            rigidBody.MoveRotation(rigidBody.rotation * deltaRotation);
        }
        
        // Apply all movement at once
        if (totalMovement != Vector3.zero)
        {
            rigidBody.MovePosition(rigidBody.position + totalMovement);
        }
        
        // Update camera target to follow player position
        if (followPlayer && cameraTarget != null)
        {
            cameraTarget.position = transform.position;
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
        var direction = (moveAmount.y * cameraTarget.forward + moveAmount.x * cameraTarget.right).normalized;
        var deltaPosition = direction * WalkSpeed * Time.deltaTime;
        rigidBody.MovePosition(rigidBody.position + deltaPosition);
        
        var deltaRotation = direction == -transform.forward ? Quaternion.Euler(0f, 180f, 0f)
                                                            : Quaternion.FromToRotation(transform.forward, direction);
        rigidBody.MoveRotation(rigidBody.rotation * deltaRotation);
    }
    
    public void SetPlatform(Transform platform)
    {
        currentPlatform = platform;
        if (platform != null)
        {
            lastPlatformPosition = platform.position;
        }
    }
}