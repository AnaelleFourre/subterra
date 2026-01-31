using UnityEngine;

public class Trap : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
            RespawnManager.Respawn(collision.collider.gameObject);
    }
}
