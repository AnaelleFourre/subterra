using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager instance;

    public Checkpoint currentCheckpoint; // référence au dernier checkpoint activé

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
            return currentCheckpoint.GetSpawnPosition();
        return Vector3.zero;
    }
}
