using UnityEngine;

public class KeyScript : MonoBehaviour
{
    [SerializeField] private DoorManager door;
    [SerializeField] private GameObject wallToMove; // Le mur à déplacer
    [SerializeField] private float floatAmplitude = 0.10f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float wallMoveDistance = 6f; // Distance de déplacement du mur

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
            MoveWallUp(); // Appelle la méthode pour déplacer le mur
            Destroy(gameObject);
        }
    }

    // Méthode pour déplacer le mur en y+6
    private void MoveWallUp()
    {
        if (wallToMove != null)
        {
            Vector3 newPosition = wallToMove.transform.position + Vector3.up * wallMoveDistance;
            wallToMove.transform.position = newPosition;
        }
        else
        {
            Debug.LogWarning("Aucun mur assigné dans l'inspecteur !");
        }
    }
}
