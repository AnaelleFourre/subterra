using UnityEngine;

public class GrappleGunAutoActivate : MonoBehaviour
{
    [SerializeField] private LineRenderer ropeLine;
    [SerializeField] private Renderer gun;

    private void Start()
    {
        if (gun == null)
        {
            Debug.LogError("Gun not assigned!");
            enabled = false;
        }
    }

    private void Update()
    {
        if (ropeLine == null) return;
        var ropeActive = ropeLine.positionCount > 0;
        gun.enabled = ropeActive;
    }
}
