using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;



/**
* Pour le menu principal, sur un eventSystem, si il y en a plusieurs, alors déactive celui-ci
*/
public class MainMenuEventSystem : MonoBehaviour
{

    public GameObject eventSystem2Shut; // L'eventsystem à désactiver

    void Awake(){
        if (FindObjectsOfType<EventSystem>().Length > 1){
            Destroy(eventSystem2Shut);
        } 
    }

    

}
