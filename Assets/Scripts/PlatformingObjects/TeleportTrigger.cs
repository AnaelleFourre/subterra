using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    private SceneLoader sceneLoader;

    private void Start()
    {
        sceneLoader = GetComponentInParent<SceneLoader>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            sceneLoader.StartTeleport();
    }
}