using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private DoorManager linkedDoor;

    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || linkedDoor != null && !linkedDoor.isOpen)
        {
            return;
        }

        SceneManager.LoadScene(sceneName);
        SceneManager.sceneLoaded += OnSceneLoaded;
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
