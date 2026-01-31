using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentPlayer : MonoBehaviour
{
    private static PersistentPlayer instance;
    [SerializeField] private GrapplingGun grapplingGun;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Vector3 spawnPosition;
        if (scene.name == "MainMenu")
        {
            spawnPosition = new Vector3(0, 2, -100);
            grapplingGun.SetGrappleAllowed(false);
        }
        else
        {
            spawnPosition = CheckpointManager.instance.GetDefaultSpawn() + Vector3.up * 0.5f;
            grapplingGun.SetGrappleAllowed(true);
        }
        transform.position = spawnPosition;
    }
}
