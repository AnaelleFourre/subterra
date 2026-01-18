using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private DoorManager linkedDoor;

    TeleportTrigger trigger;
    bool isTeleporting;

    void Start()
    {
        trigger = GetComponentInChildren<TeleportTrigger>();
        isTeleporting = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isTeleporting || trigger != null || !other.CompareTag("Player") || (linkedDoor != null && !linkedDoor.isOpen))
        {
            return;
        }

        StartTeleport();
    }

    public void StartTeleport()
    {
        if (isTeleporting)
        {
            return;
        }

        isTeleporting = true;
        FadeTransition();
    }

    async void FadeTransition()
    {
        var playerMovement = FindAnyObjectByType<PlayerMovement>();
        if (playerMovement)
        {
            playerMovement.ResetAnimationSpeed();
            playerMovement.enabled = false;
        }

        await ScreenFader.Instance.FadeOut();

        SceneManager.LoadScene(sceneName);
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (playerMovement)
        {
            playerMovement.enabled = true;
        }

        await ScreenFader.Instance.FadeIn();
        isTeleporting = false;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null && CheckpointManager.instance != null)
        {
            player.transform.position = CheckpointManager.instance.GetDefaultSpawn() + Vector3.up * 0.5f;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
