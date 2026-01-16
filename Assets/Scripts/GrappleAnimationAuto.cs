using UnityEngine;

public class GrappleAnimationAuto : MonoBehaviour
{
    public LineRenderer ropeLine;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("IsGrappling", false);
    }

    void Update()
    {
        if (ropeLine == null) return;

        bool ropeActive = ropeLine.positionCount > 0;

        animator.SetBool("IsGrappling", ropeActive);
    }
}
