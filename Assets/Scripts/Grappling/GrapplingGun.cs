using UnityEngine;
using UnityEngine.InputSystem;   // nécessaire
using UnityEngine.Rendering;

public class GrapplingGun : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, player;
    public Transform playerCamera;
    public float ropeLength = 5f; // Longueur fixe de la corde
    private SpringJoint joint;
    private float maxDistance = 50f;

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
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxDistance, whatIsGrappleable))
        {
            Vector3 direction = (hit.point - gunTip.position).normalized;
            grapplePoint = gunTip.position + direction * ropeLength;

            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = 100f;
            joint.damper = 0f;
            joint.massScale = 1f;

            lr.positionCount = 2;
        }
    }

    void DrawRope()
    {
        if (joint != null)
        {
            lr.SetPosition(0, gunTip.position);
            lr.SetPosition(1, grapplePoint);
        }
        else
        {
            lr.SetPosition(0, gunTip.position);
            lr.SetPosition(1, gunTip.position);
        }
    }

    void StopGrapple()
    {
        if (joint != null)
        {
            Destroy(joint);
        }
        grapplePoint = gunTip.position;
    }

    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}
