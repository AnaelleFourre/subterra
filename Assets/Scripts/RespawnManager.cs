using UnityEngine;

public static class RespawnManager
{
    public static void Respawn(GameObject player)
    {
        Vector3 respawnPosition = CheckpointManager.instance.GetRespawnPosition();

        RespawnAt(player, respawnPosition);
    }

    public static void RespawnStart(GameObject player)
    {
        Vector3 respawnPosition = CheckpointManager.instance.GetDefaultSpawn();

        RespawnAt(player, respawnPosition);
    }

    private static void RespawnAt(GameObject player, Vector3 respawnPosition)
    {
        if (respawnPosition == Vector3.zero)
        {
            return;
        }

        player.transform.position = respawnPosition + Vector3.up * 0.5f;

        if (player.TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}