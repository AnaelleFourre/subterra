using UnityEngine;
using UnityEngine.EventSystems;

/**
* Pour le menu principal, sur un eventSystem, si il y en a plusieurs, alors déactive celui-ci
*/
public class MainMenuEventSystem : MonoBehaviour
{
    public GameObject eventSystem2Shut; // L'eventsystem à désactiver

    private void Awake()
    {
        var objects = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        if (objects.Length > 1)
            Destroy(eventSystem2Shut);
    }
}
