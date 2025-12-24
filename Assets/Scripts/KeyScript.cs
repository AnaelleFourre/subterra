using UnityEngine;

public class KeyScript : MonoBehaviour
{
    [SerializeField] private DoorManager door;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            door.ToggleDoor();
            Destroy(gameObject);
        }
    }
}
