using UnityEngine;

public class KeyScript : MonoBehaviour
{
    [SerializeField] private DoorManager door;
    [SerializeField] private GameObject wall; // Le mur à déplacer
    [SerializeField] private float floatAmplitude = 0.10f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private Vector3 wallMoveVector = 6f * Vector3.up;
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        var offsetY = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = startPosition + Vector3.up * offsetY;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door.ToggleDoor();
            MoveWall(); // Appelle la méthode pour déplacer le mur
            Destroy(gameObject);
        }
    }

    // Méthode pour déplacer le mur après avoir récupéré la clé
    private void MoveWall()
    {
        if (wall != null)
        {
            var newPosition = wall.transform.position + wallMoveVector;
            wall.transform.position = newPosition;
        }
        else
        {
            Debug.LogWarning("Aucun mur assigné dans l'inspecteur !");
        }
    }
}
