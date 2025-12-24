using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class CameraRelinker : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
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