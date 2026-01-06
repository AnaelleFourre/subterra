using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    SceneLoader sceneLoader;

    void Start()
    {
        sceneLoader = GetComponentInParent<SceneLoader>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        sceneLoader.StartTeleport();
    }
}