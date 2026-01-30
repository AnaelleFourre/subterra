using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class CameraRelinker : MonoBehaviour
{
    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var target = GameObject.FindWithTag("CameraTarget");
        var cams = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);
        foreach (var cam in cams)
        {
            if (cam.name == "CinemachineCamera")
            {
                cam.Follow = target.transform;
                cam.LookAt = target.transform;
                break;
            }
        }
    }
}