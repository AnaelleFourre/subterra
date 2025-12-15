using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 pointB = new Vector3(0, 10, 0);
    public float speed = 2f;

    private Vector3 pointA;
    private Vector3 target;

    void Start()
    {
        pointA = transform.position;
        target = pointA + pointB;
    }

    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            target = target == pointA ? pointA + pointB : pointA;
        }
    }
}