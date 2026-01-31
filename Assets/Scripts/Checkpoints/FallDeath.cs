using UnityEngine;

public class FallDeath : MonoBehaviour
{
    [SerializeField] private float deathTresholdY = -10;

    private void Update()
    {
        if (transform.position.y < deathTresholdY)
            RespawnManager.Respawn(gameObject);
    }
}
