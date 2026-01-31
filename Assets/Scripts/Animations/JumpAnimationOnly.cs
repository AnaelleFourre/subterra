using UnityEngine;
using UnityEngine.InputSystem;

public class JumpAnimationOnly : MonoBehaviour
{
    [Header("Same jump condition as PlayerMovement")]
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private Transform feetTransform;
    [SerializeField] private Animator animator;

    private InputAction jumpAction;
    private bool hasJumped;

    private void Awake()
    {
        jumpAction = InputSystem.actions.FindAction("Jump", true);
    }

    private void Update()
    {
      
        if (jumpAction.WasPressedThisFrame() && CanJump())
        {
            animator.SetTrigger("Jump");
            hasJumped = true;
        }

        if (hasJumped && CanJump())
        {
            hasJumped = false;
        }
    }

    private bool CanJump()
    {
        return Physics.OverlapSphere(
            feetTransform.position,
            0.1f,
            playerMask
        ).Length != 0;
    }
}
