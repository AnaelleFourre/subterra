using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private DoorManager linkedDoor;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || (linkedDoor != null && !linkedDoor.isOpen))
        {
            return;
        }

        StartTeleport();
    }

    public void StartTeleport()
    {
        FadeTransition();
    }

    async void FadeTransition()
    {
        var playerMovement = FindAnyObjectByType<PlayerMovement>();
        if (playerMovement)
        {
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
