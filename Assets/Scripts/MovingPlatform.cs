using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Vector3 pointB;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float pauseDuration = 3f;

    private Vector3 pointA;
    private Vector3 target;
    private Rigidbody rbPlatform;
    private bool isPaused;

    public Vector3 PlatformVelocity { get; private set; }

    void Start()
    {
        rbPlatform = GetComponent<Rigidbody>();
        pointA = rbPlatform.position;
        target = pointA + pointB;
    }

    void FixedUpdate()
    {
        if (isPaused)
        {
            PlatformVelocity = Vector3.zero;
            return;
        }

        Vector3 oldPos = rbPlatform.position;
        Vector3 newPos = Vector3.MoveTowards(oldPos, target, speed * Time.fixedDeltaTime);

        rbPlatform.MovePosition(newPos);
        PlatformVelocity = (newPos - oldPos) / Time.fixedDeltaTime;

        if (Vector3.Distance(newPos, target) < 0.05f)
        {
            StartCoroutine(PauseAtTarget());
            target = (target == pointA) ? pointA + pointB : pointA;
        }
    }

    IEnumerator PauseAtTarget()
    {
        isPaused = true;
        yield return new WaitForSeconds(pauseDuration);
        isPaused = false;
    }
}
