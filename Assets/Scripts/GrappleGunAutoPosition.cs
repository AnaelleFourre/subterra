using UnityEngine;

public class GrappleGunAutoPosition : MonoBehaviour
{
    public LineRenderer ropeLine;
    public Transform gun;

    [Header("Grapple Pose")]
    public Vector3 grappleLocalPosition;
    public Vector3 grappleLocalRotation;

    private Vector3 originalLocalPosition;
    private Vector3 originalLocalRotation;

    void Start()
    {
        if (gun == null)
        {
            Debug.LogError("Gun not assigned!");
            enabled = false;
            return;
        }

        originalLocalPosition = gun.localPosition;
        originalLocalRotation = gun.localEulerAngles;
    }

    void Update()
    {
        if (ropeLine == null) return;

        bool ropeActive = ropeLine.positionCount > 0;

        if (ropeActive)
        {
            gun.localPosition = grappleLocalPosition;
            gun.localEulerAngles = grappleLocalRotation;
        }
        else
        {
            gun.localPosition = originalLocalPosition;
            gun.localEulerAngles = originalLocalRotation;
        }
    }
}
