using UnityEngine;

public class GrappleSelector : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectionRadius = 5f;
    [Tooltip("Maximum angle in degrees from camera forward to accept a target")]
    [SerializeField] private float maxAngleDeg = 60f;
    [SerializeField] private LayerMask grappleMask;
    [SerializeField] private LayerMask obstructionMask;
    [Header("References")]
    [SerializeField] private Transform cameraTransform; // If null then fallback to Camera.main
    private GrappleHighlight currentHighlight;
    private Transform bestTransformLastFrame;
    private readonly Collider[] colliderBuffer = new Collider[128]; // Used by OverlapSphereNonAlloc

    private void Awake()
    {
        grappleMask = LayerMask.GetMask("GrapplePoints");
        obstructionMask = LayerMask.GetMask("Default");
    }

    private void Update()
    {
        if (cameraTransform == null)
        {
            if (Camera.main != null)
                cameraTransform = Camera.main.transform;
            else
            {
                Debug.LogError("[GrappleSelector] No camera assigned and Camera.main is null.");
                return;
            }
        }
        FindAndHighlight();
    }

    private void FindAndHighlight()
    {
        // OverlapSphereNonAlloc is optional but avoids allocations
        var collidersCount = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, colliderBuffer, grappleMask);
        if (collidersCount <= 0)
        {
            if (currentHighlight != null)
            {
                currentHighlight.DisableHighlight();
                currentHighlight = null;
                Debug.Log("[GrappleSelector] No grapple candidates found in radius.");
            }
            return;
        }

        var firstCandidate = (Transform)null;
        var best = (Transform)null;
        var bestScore = -Mathf.Infinity;

        for (int i = 0; i < collidersCount; i++)
        {
            var collider = colliderBuffer[i];
            if (collider == null) continue;

            var transform = collider.transform;
            if (firstCandidate == null) firstCandidate = transform;

            var toTarget = transform.position - cameraTransform.position;
            var dist = toTarget.magnitude;
            var dir = toTarget.normalized;

            var angle = Vector3.Angle(cameraTransform.forward, dir);
            if (angle > maxAngleDeg || dist > detectionRadius)
                continue; // outside angle cone

            // obstruction test: check if something blocks view before the candidate
            if (Physics.Raycast(cameraTransform.position, dir, out var hit, dist, obstructionMask) && hit.transform != transform)
            {
                // blocked by something else
                continue;
            }

            // score can be inverse distance or dot product; here we use dot / dist
            var dot = Vector3.Dot(cameraTransform.forward, dir); // closer to 1 = more in front
            var score = dot - dist / detectionRadius * 0.5f;
            if (score > bestScore)
            {
                bestScore = score;
                best = transform;
            }
        }

        // Update highlight only when changed
        var newHighlight = best ? best.GetComponent<GrappleHighlight>() : null;
        if (newHighlight != currentHighlight)
        {
            if (currentHighlight != null) currentHighlight.DisableHighlight();
            if (newHighlight != null) newHighlight.EnableHighlight();
            currentHighlight = newHighlight;
        }

        bestTransformLastFrame = best;
    }

    public Transform GetSelectedTarget() => bestTransformLastFrame;
}
