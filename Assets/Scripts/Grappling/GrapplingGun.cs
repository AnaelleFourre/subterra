using UnityEngine;
using UnityEngine.InputSystem;   // nécessaire
using UnityEngine.Rendering;

public class GrapplingGun : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 grapplePoint;
    public Transform gunTip, player;
    private SpringJoint joint;
    public GrappleSelector selector;
    private InputAction grappleAction;   // nouvelle action

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        grapplePoint = gunTip.position;
    }

    private void OnEnable()
    {
        grappleAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/x");

        grappleAction.performed += ctx => StartGrapple(); // équivalent de GetKeyDown
        grappleAction.canceled  += ctx => StopGrapple();  // équivalent de GetKeyUp

        grappleAction.Enable();
    }

    private void OnDisable()
    {
        grappleAction.Disable();
    }

    void LateUpdate()
    {
        DrawRope();
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

        // Création du joint
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grapplePoint;

        float dist = Vector3.Distance(player.position, grapplePoint);

        joint.maxDistance = dist;
        joint.minDistance = dist * 0.6f;
        joint.spring = 2000f;
        joint.damper = 30f;
        joint.massScale = 4f;

        lr.positionCount = 2;
    }


    void StopGrapple()
    {
        if (joint != null)
        {
            Destroy(joint);
            joint = null;
        }

        grapplePoint = gunTip.position;
        lr.positionCount = 0;
    }

    void DrawRope()
    {
        if (joint == null)
            return;

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, grapplePoint);
        lr.startColor = Color.yellow;
        lr.endColor = Color.yellow;
    }
}
