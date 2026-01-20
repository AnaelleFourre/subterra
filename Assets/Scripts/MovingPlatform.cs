using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{
    [Tooltip("Position du point B translaté par rapport au point de départ (Si dans le gameobject parent)")]
    [SerializeField] private Vector3 pointB = new Vector3(0, 0, 0);
    [SerializeField] private float speed = 5f;
    [SerializeField] private float pauseDuration = 3f;

    private Vector3 pointA;
    private Vector3 target;
    private Rigidbody rbPlatform;
    private bool isPaused = false;

    void Start()
    {
        rbPlatform = GetComponent<Rigidbody>();
        pointA = transform.position;
        target = pointA + pointB;
    }

    void FixedUpdate()
    {
        if (isPaused) return;

        Vector3 newPos = Vector3.MoveTowards(rbPlatform.position, target, speed * Time.fixedDeltaTime);
        rbPlatform.MovePosition(newPos);

        if (Vector3.Distance(rbPlatform.position, target) < 0.1f)
        {
            StartCoroutine(PauseAtTarget());
            target = target == pointA ? pointA + pointB : pointA;
        }
    }

    // Coroutine pour gérer la pause
    IEnumerator PauseAtTarget()
    {
        isPaused = true;
        yield return new WaitForSeconds(pauseDuration);
        isPaused = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAndTarget"))
        {
            // Attache le joueur à la plateforme
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerAndTarget"))
        {
            // Détache le joueur de la plateforme
            other.transform.SetParent(null);
        }
    }
}
