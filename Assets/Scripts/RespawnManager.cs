using UnityEngine;

public static class RespawnManager
{
    static bool isRespawning = false;
    public static void Respawn(GameObject player)
    {
        if (isRespawning)
        {
            return;
        }

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

        isRespawning = true;
        FadeTransition(player, respawnPosition);
    }

    static async void FadeTransition(GameObject player, Vector3 respawnPosition)
    {
        var playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement)
        {
            playerMovement.enabled = false;
        }

        await ScreenFader.Instance.FadeOut();

        player.transform.position = respawnPosition + Vector3.up * 0.5f;

        if (player.TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }


        if (playerMovement)
        {
            playerMovement.enabled = true;
        }

        await ScreenFader.Instance.FadeIn();
        isRespawning = false;
    }
}