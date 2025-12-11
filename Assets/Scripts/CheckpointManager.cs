using UnityEditor.Rendering.Universal;
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

    private void Start()
    {
        FindDefaultCheckpoint();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindDefaultCheckpoint();
    }

    private void FindDefaultCheckpoint()
    {
        var allCp = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);

        foreach (var cp in allCp)
        {
            cp.Deactivate();

            if (defaultCheckpoint == null && cp.IsDefault)
            {
                defaultCheckpoint = cp;
                currentCheckpoint = cp;
                currentCheckpoint.Activate();
            }
        }
    }

    public void ActivateCheckpoint(Checkpoint checkpoint)
    {
        if (currentCheckpoint != null && currentCheckpoint != checkpoint)
        {
            currentCheckpoint.Deactivate();
        }

        currentCheckpoint = checkpoint;
        checkpoint.Activate();
        Debug.Log($"Checkpoint actif = {checkpoint.name}");
    }

    public Vector3 GetRespawnPosition()
    {
        if (currentCheckpoint != null)
        {
            return currentCheckpoint.GetSpawnPosition();
        }
        return Vector3.zero;
    }

    public Vector3 GetDefaultSpawn()
    {
        if (defaultCheckpoint != null)
        {
            return defaultCheckpoint.GetSpawnPosition();
        }
        return Vector3.zero;
    }
}
