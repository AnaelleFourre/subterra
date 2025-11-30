using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentPlayer : MonoBehaviour
{
    private static PersistentPlayer instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var spawnPosition = CheckpointManager.instance.GetDefaultSpawn() + Vector3.up * 0.5f;
        transform.position = spawnPosition;
    }
}
