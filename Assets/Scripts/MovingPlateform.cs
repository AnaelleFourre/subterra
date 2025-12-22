using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 pointB = new Vector3(0, 10, 0);
    public float speed = 2f;

    private Vector3 pointA;
    private Vector3 target;

    private Rigidbody rb; // Added Rigidbody reference

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pointA = transform.position;
        target = pointA + pointB;
    }

    void FixedUpdate()
    {
        Vector3 newPos = Vector3.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        
        rb.MovePosition(newPos);

        if (Vector3.Distance(rb.position, target) < 0.1f)
        {
            target = target == pointA ? pointA + pointB : pointA; //
        }
    }
}