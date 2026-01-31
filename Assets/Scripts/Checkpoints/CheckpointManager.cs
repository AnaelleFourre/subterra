using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager instance;
    [SerializeField] private Checkpoint currentCheckpoint;
    [SerializeField] private Checkpoint defaultCheckpoint;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => FindDefaultCheckpoint();

    private void FindDefaultCheckpoint()
    {
        defaultCheckpoint = null;
        var checkpoints = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
        foreach (var checkpoint in checkpoints)
        {
            checkpoint.Deactivate();
            if (defaultCheckpoint == null && checkpoint.IsDefault)
            {
                defaultCheckpoint = checkpoint;
                currentCheckpoint = checkpoint;
                currentCheckpoint.Activate();
            }
        }
    }

    public void ActivateCheckpoint(Checkpoint checkpoint)
    {
        if (currentCheckpoint != null && currentCheckpoint != checkpoint)
            currentCheckpoint.Deactivate();
        currentCheckpoint = checkpoint;
        checkpoint.Activate();
    }

    public Vector3 GetRespawnPosition() => currentCheckpoint == null ? Vector3.zero : currentCheckpoint.GetSpawnPosition();

    public Vector3 GetDefaultSpawn() => defaultCheckpoint == null ? Vector3.zero : defaultCheckpoint.GetSpawnPosition();
}
