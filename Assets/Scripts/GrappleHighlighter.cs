using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GrappleHighlight : MonoBehaviour
{

    private Renderer grappleRenderer;
    public Material standardMaterial;
    public Material highlightMaterial;

    void Awake()
    {
        // find a renderer even if it's on a child object
        grappleRenderer = GetComponent<Renderer>() ?? GetComponentInChildren<Renderer>();
        if (grappleRenderer == null)
        {
            Debug.LogWarning($"[{name}] GrappleHighlight: no Renderer found on object or children.");
            return;
        }

    }

    public void EnableHighlight()
    {
        grappleRenderer.material = highlightMaterial;
    }

    public void DisableHighlight()
    {
        grappleRenderer.material = standardMaterial;
    }
}
