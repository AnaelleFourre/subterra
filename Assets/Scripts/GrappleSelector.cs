
using UnityEngine;
using System.Collections.Generic;

public class GrappleSelector : MonoBehaviour
{
    void Awake()
    {
        grappleMask = LayerMask.GetMask("GrapplePoints");
        obstructionMask = LayerMask.GetMask("Default");
    }

    [Header("Detection")]
    public float detectionRadius = 5f;
    [Tooltip("Maximum angle in degrees from camera forward to accept a target")]
    public float maxAngleDeg = 60f;
    public LayerMask grappleMask;
    public LayerMask obstructionMask;

    [Header("References")]
    public Transform cameraTransform; // if null we fallback to Camera.main

    private GrappleHighlight currentHighlight;
    private Transform bestTransformLastFrame;
    private Transform[] overlapBuffer = new Transform[64];

    void Update()
    {
        if (cameraTransform == null)
        {
            if (Camera.main != null) cameraTransform = Camera.main.transform;
            else
            {
                Debug.LogError("[GrappleSelector] No camera assigned and Camera.main is null.");
                return;
            }
        }

        FindAndHighlight();
    }

    void FindAndHighlight()
    {
        // OverlapSphereNonAlloc is optional but avoids allocations
        int count = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, GetColliderBuffer(), grappleMask);
        if (count <= 0)
        {
            if (currentHighlight != null)
            {
                currentHighlight.DisableHighlight();
                currentHighlight = null;
                Debug.Log("[GrappleSelector] No grapple candidates found in radius.");
            }
            return;
        }

        Transform best = null;
        float bestScore = -Mathf.Infinity;
        Transform firstCandidate = null;

        for (int i = 0; i < count; i++)
        {
            Collider c = colliderBuffer[i];
            if (c == null) continue;

            Transform t = c.transform;
            if (firstCandidate == null) firstCandidate = t;

            Vector3 toTarget = (t.position - cameraTransform.position);
            float dist = toTarget.magnitude;
            Vector3 dir = toTarget.normalized;

            float angle = Vector3.Angle(cameraTransform.forward, dir);
            if (angle > maxAngleDeg || dist > detectionRadius)
            {
                // outside angle cone
                continue;
            }

            // obstruction test: check if something blocks view before the candidate
            if (Physics.Raycast(cameraTransform.position, dir, out RaycastHit hit, dist, obstructionMask))
            {
                // blocked by something else
                if (hit.transform != t)
                {
                    continue;
                }
            }

            // score can be inverse distance or dot product; here we use dot / dist
            float dot = Vector3.Dot(cameraTransform.forward, dir); // closer to 1 = more in front
            float score = dot - (dist / detectionRadius) * 0.5f;

            if (score > bestScore)
            {
                bestScore = score;
                best = t;
            }
        }


        // Update highlight only when changed
        GrappleHighlight newHighlight = best ? best.GetComponent<GrappleHighlight>() : null;
        if (newHighlight != currentHighlight)
        {
            if (currentHighlight != null) currentHighlight.DisableHighlight();
            if (newHighlight != null) newHighlight.EnableHighlight();

            currentHighlight = newHighlight;
        }

        bestTransformLastFrame = best;
    }
    public Transform GetSelectedTarget()
    {
        return bestTransformLastFrame;
    }
    // ---- helpers for OverlapSphereNonAlloc (stores colliders) ----
    private Collider[] colliderBuffer = new Collider[128];
    private Collider[] GetColliderBuffer()
    {
        // OverlapSphereNonAlloc needs a Collider[]; this is a thin wrapper
        return colliderBuffer;
    }
}
