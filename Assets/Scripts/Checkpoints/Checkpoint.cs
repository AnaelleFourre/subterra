using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Checkpoint : MonoBehaviour
{
    [Header("Checkpoint Settings")]
    [SerializeField] private int levelNumber;
    [SerializeField] private bool isDefaultCheckpoint = false;

    [Header("Checkpoint Colors")]
    [SerializeField] private Material inactiveMaterial;
    [SerializeField] private Material activeMaterial;

    private Renderer _renderer;

    public bool IsDefault => isDefaultCheckpoint;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        Deactivate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CheckpointManager.instance.ActivateCheckpoint(this);
            SaveManager.Save(transform.position, levelNumber);
        }
    }

    public Vector3 GetSpawnPosition() => transform.position;

    public void Activate() => _renderer.material = activeMaterial;

    public void Deactivate() => _renderer.material = inactiveMaterial;
}