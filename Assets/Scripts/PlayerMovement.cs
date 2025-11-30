using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private InputActionAsset InputActions;
    [SerializeField] private float WalkSpeed = 3;
    [SerializeField] private float JumpForce = 3;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction turnAction;
    private Vector2 moveAmount;
    private Rigidbody rigidBody;
    private Transform feetTransform;

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
        Walk();
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
        var rotationAmount = lookAmount.x * 90f;
        var deltaRotation = Quaternion.Euler(0, rotationAmount, 0);
        rigidBody.MoveRotation(rigidBody.rotation * deltaRotation);
    }

    private void Walk()
    {
        var direction = moveAmount.y * transform.forward + moveAmount.x * transform.right;
        var distance = WalkSpeed * Time.deltaTime;
        var deltaPosition = direction.normalized * distance;
        rigidBody.MovePosition(rigidBody.position + deltaPosition);
    }
}
