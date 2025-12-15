using UnityEngine;
using UnityEngine.InputSystem;   // nécessaire
using UnityEngine.Rendering;

public class GrapplingGun : MonoBehaviour
{
    private Vector3 grapplePoint;
    public Transform gunTip, player;
    public GrappleSelector selector;
    private InputAction grappleAction;   // nouvelle action
    private bool isGrappling;
    public Rope rope;
    void Awake()
    {
        isGrappling = false;
        grapplePoint = gunTip.position;
   
    }

    private void OnEnable()
    {
        grappleAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/x");

        grappleAction.performed += ctx => GrappleManager();

        grappleAction.Enable();
    }

    private void OnDisable()
    {
        grappleAction.Disable();
    }

    void GrappleManager()
    {
        isGrappling = !isGrappling;
        if (isGrappling)
        {
            StartGrapple();
        }
        else
        {
            StopGrapple();
        }

    }
    void StartGrapple()
    {
        if (selector == null)
        {
            Debug.LogError("Missing GrappleSelector reference");
            return;
        }

        Transform best = selector.GetSelectedTarget();
        if (best == null)
        {
            Debug.Log("No selected grapple target");
            return;
        }

        grapplePoint = best.position;
        rope.initRope(gunTip, grapplePoint);
        rope.enabled = true;


    }


    void StopGrapple()
    {

        grapplePoint = gunTip.position;
        rope.enabled = false;
    }

}
