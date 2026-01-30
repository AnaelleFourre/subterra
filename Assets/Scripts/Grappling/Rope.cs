using System.Net;
using NUnit.Framework.Constraints;
using UnityEngine;

using System;

[RequireComponent(typeof(LineRenderer))]
public class Rope : MonoBehaviour
{
    [SerializeField] Mesh link;
    [SerializeField] Material linkMaterial;

    [SerializeField] float nodeDistance = 0.1f;
    [SerializeField] float nodeColliderRadius = 0.2f;
    [SerializeField] float ropeWidth = 0.1f;
    [SerializeField] LayerMask collisionMask = ~0; // All layers by default
    int totalNodes;

    [SerializeField] float gravityStrength = 2;
    [SerializeField] float velocityDampen = 0.1f;
    [SerializeField] float stiffness = 0.95f;

    [SerializeField] int iterateCollisionsEvery = 10;
    [SerializeField] int iterations = 100;
    int colliderBufferSize = 1;
    RaycastHit[] raycastHitBuffer;
    Collider[] colliderHitBuffer;
    Camera cam;


    [SerializeField] Rigidbody attachedRigidbody;
    [SerializeField] float pullForceStrength = 800f;
    [SerializeField] float pullThreshold = 0.95f;

    Vector3 gravity;

    Vector3 ropeStart;
    Vector3 ropeEnd;
    Transform gunTip;


    Vector3[] previousNodePositions;
    Vector3[] currentNodePositions;
    Quaternion[] currentNodeRotations;

    LineRenderer lineRenderer;
    Vector3[] linePositions;

    SphereCollider nodeCollider;
    GameObject nodeTester;
    Matrix4x4[] matrices;

    int MAX_LENGTH = 5;

    public void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 0;

        enabled = false;

        raycastHitBuffer = new RaycastHit[colliderBufferSize];
        colliderHitBuffer = new Collider[colliderBufferSize];
        gravity = new Vector3(0, -gravityStrength, 0);
        cam = Camera.main;

        if (nodeTester == null)
        {
            nodeTester = new GameObject("Node Tester");
            nodeTester.layer = 8;
            DontDestroyOnLoad(nodeTester);

            nodeCollider = nodeTester.AddComponent<SphereCollider>();
            nodeCollider.radius = nodeColliderRadius;
        }
    }

    public void initRope(Transform start, Vector3 endPoint)
    {
        gunTip = start;
        ropeStart = gunTip.position;
        ropeEnd = endPoint;

        float dist = Vector3.Distance(ropeStart, ropeEnd);
        totalNodes = Mathf.CeilToInt(dist / nodeDistance) - 1;

        // Clamp to at least 2 nodes to avoid divide-by-zero.
        totalNodes = Mathf.Max(2, totalNodes);

        currentNodePositions = new Vector3[totalNodes];
        previousNodePositions = new Vector3[totalNodes];
        currentNodeRotations = new Quaternion[totalNodes];

        matrices = new Matrix4x4[totalNodes];

        for (int i = 0; i < totalNodes; i++)
        {
            float t = (float)i / (totalNodes - 1);
            Vector3 position = Vector3.Lerp(ropeStart, ropeEnd, t);

            currentNodePositions[i] = position;
            currentNodeRotations[i] = Quaternion.identity;
            previousNodePositions[i] = position;
            matrices[i] = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);
        }

        linePositions = new Vector3[totalNodes];
    }

    public void SetAttachedRigidbody(Rigidbody rb)
    {
        attachedRigidbody = rb;
    }

    public float getRopeLength()
    {
        return totalNodes * nodeDistance;
    }

    public bool isRopeTaut()
    {
        float currentDist = Vector3.Distance(ropeStart, ropeEnd);
        float ropeLength = getRopeLength();
        if (currentDist >= ropeLength * 0.98f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Vector3 getGrapplePoint()
    {
        return ropeEnd;
    }

    public Vector3 getRopeStart()
    {
        return ropeStart;
    }

    void Update()
    {
        if (isActiveAndEnabled == false) return;

        ropeStart = gunTip.position;
        currentNodePositions[0] = ropeStart;

        DrawRope();

        Graphics.DrawMeshInstanced(link, 0, linkMaterial, matrices, totalNodes);
    }

    private void OnDisable()
    {
        lineRenderer.positionCount = 0;
    }

    private void FixedUpdate()
    {
        if (isActiveAndEnabled == false) return;

        Simulate();

        for (int i = 0; i < iterations; i++)
        {
            ApplyConstraint();

            if (i % iterateCollisionsEvery == 0)
            {
                AdjustCollisions();
            }
        }

        ApplyPullForce();
        SetAngles();
        TranslateMatrices();
    }

    private void ApplyPullForce()
    {
        if (attachedRigidbody == null) return;

        Vector3 toGrapplePoint = ropeEnd - ropeStart;
        float currentDistance = toGrapplePoint.magnitude;
        float maxRopeLength = getRopeLength();

        if (currentDistance > maxRopeLength * pullThreshold)
        {
            float overstretch = currentDistance - (maxRopeLength * pullThreshold);
            float stretchRatio = overstretch / maxRopeLength;

            Vector3 pullDirection = toGrapplePoint.normalized;

            Vector3 pullForce = pullDirection * (pullForceStrength * stretchRatio);
            attachedRigidbody.AddForce(pullForce, ForceMode.Force);

            Vector3 velocity = attachedRigidbody.linearVelocity;
            float outwardVelocity = Vector3.Dot(velocity, pullDirection);

            if (outwardVelocity < 0)
            {
                Vector3 dampingForce = -pullDirection * (outwardVelocity * 0.5f);
                attachedRigidbody.AddForce(dampingForce, ForceMode.VelocityChange);
            }
        }
    }

    private void Simulate()
    {
        var fixedDt = Time.fixedDeltaTime;
        for (int i = 0; i < totalNodes; i++)
        {
            Vector3 velocity = currentNodePositions[i] - previousNodePositions[i];
            velocity *= velocityDampen;

            previousNodePositions[i] = currentNodePositions[i];

            Vector3 newPos = currentNodePositions[i] + velocity;
            newPos += gravity * fixedDt;
            Vector3 direction = currentNodePositions[i] - newPos;

            currentNodePositions[i] = newPos;
        }
    }

    private void AdjustCollisions()
    {
        for (int i = 0; i < totalNodes; i++)
        {
            if (i % 2 == 0) continue;

            int result = -1;
            result = Physics.OverlapSphereNonAlloc(currentNodePositions[i], nodeColliderRadius + 0.01f, colliderHitBuffer, collisionMask);

            for (int n = 0; n < result; n++)
            {
                Vector3 colliderPosition = colliderHitBuffer[n].transform.position;
                Quaternion colliderRotation = colliderHitBuffer[n].gameObject.transform.rotation;

                Vector3 dir;
                float distance;

                Physics.ComputePenetration(nodeCollider, currentNodePositions[i], Quaternion.identity, colliderHitBuffer[n], colliderPosition, colliderRotation, out dir, out distance);

                currentNodePositions[i] += dir * distance;
            }
        }
    }

    private void ApplyConstraint()
    {
        currentNodePositions[0] = ropeStart;
        currentNodePositions[totalNodes - 1] = ropeEnd;

        for (int i = 0; i < totalNodes - 1; i++)
        {
            var node1 = currentNodePositions[i];
            var node2 = currentNodePositions[i + 1];

            float currentDistance = (node1 - node2).magnitude;
            float difference = Mathf.Abs(currentDistance - nodeDistance);
            Vector3 direction = Vector3.zero;

            if (currentDistance > nodeDistance)
            {
                direction = (node1 - node2).normalized;
            }
            else if (currentDistance < nodeDistance)
            {
                direction = (node2 - node1).normalized;
            }

            Vector3 movement = direction * difference;

            currentNodePositions[i] -= (movement * stiffness);
            currentNodePositions[i + 1] += (movement * stiffness);
        }
    }

    void SetAngles()
    {
        for (int i = 0; i < totalNodes - 1; i++)
        {
            var node1 = currentNodePositions[i];
            var node2 = currentNodePositions[i + 1];

            var dir = (node2 - node1).normalized;
            if (dir != Vector3.zero)
            {
                Quaternion desiredRotation = Quaternion.LookRotation(dir, Vector3.right) * Quaternion.Euler(90.0f, 0.0f, 0.0f);

                currentNodeRotations[i] = desiredRotation;
                currentNodeRotations[i + 1] = desiredRotation;
            }

        }
    }

    void TranslateMatrices()
    {
        for (int i = 0; i < totalNodes; i++)
        {
            matrices[i].SetTRS(currentNodePositions[i], currentNodeRotations[i], new Vector3(20 * nodeDistance, 2 * nodeDistance, 20 * nodeDistance));
        }
    }

    private void DrawRope()
    {

        lineRenderer.startWidth = ropeWidth;
        lineRenderer.endWidth = ropeWidth;

        for (int n = 0; n < totalNodes; n++)
        {
            linePositions[n] = currentNodePositions[n];
        }

        lineRenderer.positionCount = linePositions.Length;
        lineRenderer.SetPositions(linePositions);

    }
    public void RemoveFirstNode()
    {
        if (totalNodes < 2)
        {
            return;
        }
        else
        {
            for (int i = 0; i < totalNodes -1; i++)
            {
                currentNodePositions[i] = currentNodePositions[i + 1];
                previousNodePositions[i] = previousNodePositions[i + 1];
                currentNodeRotations[i] = currentNodeRotations[i + 1];
            }

            // Redimensionner les tableaux
            totalNodes--;
            Array.Resize(ref currentNodePositions, totalNodes);
            Array.Resize(ref previousNodePositions, totalNodes);
            Array.Resize(ref currentNodeRotations, totalNodes);
            Array.Resize(ref matrices, totalNodes);
            Array.Resize(ref linePositions, totalNodes);
        }
    }

    public void AddNode()
    {
        if (currentNodePositions == null || previousNodePositions == null || currentNodeRotations == null || matrices == null || linePositions == null)
        {
            return;
        }

        int newTotal = totalNodes + 1;
        Vector3[] newCurrent = new Vector3[newTotal];
        Vector3[] newPrevious = new Vector3[newTotal];
        Quaternion[] newRotations = new Quaternion[newTotal];
        Matrix4x4[] newMatrices = new Matrix4x4[newTotal];
        Vector3[] newLinePositions = new Vector3[newTotal];

        newCurrent[0] = currentNodePositions[0];
        newPrevious[0] = previousNodePositions[0];
        newRotations[0] = currentNodeRotations[0];
        newMatrices[0] = matrices[0];
        newLinePositions[0] = linePositions[0];

        Vector3 insertPos;
        if (totalNodes >= 2)
        {
            insertPos = Vector3.Lerp(currentNodePositions[0], currentNodePositions[1], 0.5f);
        }
        else
        {
            insertPos = currentNodePositions[0];
        }

        newCurrent[1] = insertPos;
        newPrevious[1] = insertPos;
        newRotations[1] = (currentNodeRotations.Length > 1) ? currentNodeRotations[1] : Quaternion.identity;
        newMatrices[1] = Matrix4x4.TRS(newCurrent[1], newRotations[1], Vector3.one);
        newLinePositions[1] = newCurrent[1];

        for (int i = 1; i < totalNodes; i++)
        {
            newCurrent[i + 1] = currentNodePositions[i];
            newPrevious[i + 1] = previousNodePositions[i];
            newRotations[i + 1] = currentNodeRotations[i];
            newMatrices[i + 1] = matrices[i];
            newLinePositions[i + 1] = linePositions[i];
        }

        totalNodes = newTotal;
        currentNodePositions = newCurrent;
        previousNodePositions = newPrevious;
        currentNodeRotations = newRotations;
        matrices = newMatrices;
        linePositions = newLinePositions;
    }

    public int getTotalNodes()
    {
               return totalNodes;
    }

    public bool isMaxLength()
    { 
        return getRopeLength() > MAX_LENGTH;
    }
}
