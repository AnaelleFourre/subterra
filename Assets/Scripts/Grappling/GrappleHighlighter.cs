using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class GrappleHighlight : MonoBehaviour
{
    private Renderer grappleRenderer;
    [SerializeField] private Material standardMaterial;
    [SerializeField] private Material highlightMaterial;

    private void Awake()
    {
        // Find renderer on object or children
        grappleRenderer = GetComponent<Renderer>();
        grappleRenderer ??= GetComponentInChildren<Renderer>();
        if (grappleRenderer == null)
            Debug.LogWarning($"[{name}] GrappleHighlight: no Renderer found on object or children.");
    }

    public void EnableHighlight() => grappleRenderer.material = highlightMaterial;

    public void DisableHighlight() => grappleRenderer.material = standardMaterial;
}
