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

    void Update()
    {
        // Move platform
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Switch direction when reaching target
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            target = target == pointA ? pointA + pointB : pointA;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }
}