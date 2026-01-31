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
    private Rigidbody _rigidbody;
    private bool isPaused;

    public Vector3 PlatformVelocity { get; private set; }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        pointA = _rigidbody.position;
        target = pointA + pointB;
    }

    private void FixedUpdate()
    {
        if (isPaused)
        {
            PlatformVelocity = Vector3.zero;
            return;
        }

        var oldPos = _rigidbody.position;
        var newPos = Vector3.MoveTowards(oldPos, target, speed * Time.fixedDeltaTime);

        _rigidbody.MovePosition(newPos);
        PlatformVelocity = (newPos - oldPos) / Time.fixedDeltaTime;

        if (Vector3.Distance(newPos, target) < 0.05f)
        {
            StartCoroutine(PauseAtTarget());
            target = (target == pointA) ? pointA + pointB : pointA;
        }
    }

    private IEnumerator PauseAtTarget()
    {
        isPaused = true;
        yield return new WaitForSeconds(pauseDuration);
        isPaused = false;
    }
}
