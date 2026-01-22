using UnityEngine;

public class KeyScript : MonoBehaviour
{
    [SerializeField] private DoorManager door;
    [SerializeField] private float floatAmplitude = 0.10f; // hauteur max
    [SerializeField] private float floatSpeed = 2f;        // vitesse oscillation

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float offsetY = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = startPosition + Vector3.up * offsetY;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door.ToggleDoor();
            Destroy(gameObject);
        }
    }
}
