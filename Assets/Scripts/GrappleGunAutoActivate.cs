using UnityEngine;

public class GrappleGunAutoActivate : MonoBehaviour
{
    public LineRenderer ropeLine;
    public Renderer gun;


    void Start()
    {
        if (gun == null)
        {
            Debug.LogError("Gun not assigned!");
            enabled = false;
            return;
        }

    }

    void Update()
    {
        if (ropeLine == null) return;

        bool ropeActive = ropeLine.positionCount > 0;

        if (ropeActive)
        {
            gun.enabled = true;
        }
        else
        {
            gun.enabled = false;
        }
    }
}
