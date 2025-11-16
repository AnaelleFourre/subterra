using UnityEngine;
using UnityEngine.Timeline;

public class Checkpoint : MonoBehaviour
{
    [Header("Checkpoint Settings")]
    [SerializeField] private int levelNumber;

    [Header("Checkpoint Colors")]
    [SerializeField] private Material inactiveMaterial;
    [SerializeField] private Material activeMaterial;

    private Renderer rend;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        Deactivate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CheckpointManager.instance.ActivateCheckpoint(this);
            SaveManager.Save(transform.position, levelNumber);
            Debug.Log($"Checkpoint atteint ! Niveau sauvegardé : {levelNumber}");
        }
    }

    public Vector3 GetSpawnPosition()
    {
        return transform.position;
    }

    public void Activate()
    {
        rend.material = activeMaterial;
    }

    public void Deactivate()
    {
        rend.material = inactiveMaterial;
    }
}