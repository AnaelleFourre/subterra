using UnityEngine;
using UnityEngine.EventSystems;

public class ParametersMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject parametersMenu;

    private bool parametersMenuActive = false;
    [SerializeField] private GameObject firstButton;
    [SerializeField] private GameObject fallbackSelection;

    public void changeMenu()
    {
        parametersMenuActive = !parametersMenuActive;
        mainMenu.SetActive(!parametersMenuActive);
        parametersMenu.SetActive(parametersMenuActive);

        if (parametersMenuActive && firstButton != null)
        {
            EventSystem.current.SetSelectedGameObject(firstButton);
        }
        else if (!parametersMenuActive)
        {
            EventSystem.current.SetSelectedGameObject(fallbackSelection);
        }
    }
}
