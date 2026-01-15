using UnityEngine;

public class ParametersMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject parametersMenu;

    private bool parametersMenuActive = false;

    public void changeMenu(){
        parametersMenuActive = !parametersMenuActive;
        mainMenu.SetActive(!parametersMenuActive);
        parametersMenu.SetActive(parametersMenuActive);
    }
}
