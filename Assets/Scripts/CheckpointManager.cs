using UnityEditor.Rendering.Universal;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager instance;

    public Checkpoint currentCheckpoint;

    private Checkpoint defaultCheckpoint;

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

    private void Start()
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
}
