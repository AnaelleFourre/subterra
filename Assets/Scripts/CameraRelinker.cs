using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class CameraRelinker : MonoBehaviour
{
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
        var vcam = FindAnyObjectByType<CinemachineCamera>();
        var player = GameObject.FindWithTag("Player");
        if (vcam != null && player != null)
        {
            vcam.Follow = player.transform;
            vcam.LookAt = player.transform;
        }
    }
}