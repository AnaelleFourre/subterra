using UnityEngine;

public class Fullscreen : MonoBehaviour
{
    public void ToggleFullscreen() => Screen.fullScreen = !Screen.fullScreen;
}
