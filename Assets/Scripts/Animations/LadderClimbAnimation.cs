using UnityEngine;
using UnityEngine.InputSystem;

public class LadderClimbAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private InputActionAsset inputActions;

    private InputAction moveAction;
    private bool isOnLadder;
    private Transform currentLadder;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        var map = inputActions.FindActionMap("Player", true);
        moveAction = map.FindAction("Move", true);
    }

    private void OnEnable()
    {
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ladder"))
            return;

        isOnLadder = true;
        currentLadder = other.transform;

        animator.SetBool("IsClimbing", true);
        animator.speed = 1f;

        Vector3 lookDir = currentLadder.position - transform.position;
        lookDir.y = 0f;

        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(lookDir);

    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Ladder"))
            return;

        ExitLadder();
    }

    private void ExitLadder()
    {
        isOnLadder = false;
        currentLadder = null;

        animator.SetBool("IsClimbing", false);
        animator.SetFloat("ClimbSpeed", 0f);
        animator.speed = 1f;
    }

    private void Update()
    {
        if (!isOnLadder)
            return;

        float vertical = moveAction.ReadValue<Vector2>().y;

      
        animator.SetFloat("ClimbSpeed", Mathf.Abs(vertical));

       
        animator.speed = Mathf.Abs(vertical) > 0.01f ? 1f : 0f;
    }
}
