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
        var target = GameObject.FindWithTag("CameraTarget");
        if (vcam != null && target != null)
        {
            vcam.Follow = target.transform;
            vcam.LookAt = target.transform;
        }
    }
}