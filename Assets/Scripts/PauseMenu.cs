using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject conteneur;
    [SerializeField] private InputActionAsset inputActions; // Assigne ton InputActionsAsset dans l'inspecteur
    private InputAction pauseAction;

    private void Awake()
    {
        // Récupère l'action "Pause" depuis l'asset
        pauseAction = inputActions.FindAction("Pause");
        pauseAction.performed += TogglePause;
        pauseAction.Enable();
    }

    private void OnDestroy()
    {
        pauseAction.performed -= TogglePause;
        pauseAction.Disable();
    }

    private void TogglePause(InputAction.CallbackContext context)
    {
        if (conteneur.activeSelf)
        {
            Continuer();
        }
        else
        {
            conteneur.SetActive(true);
            Time.timeScale = 0;
        }
    }

    /**
     * Retire le menu et relance le jeu
     */
    public void Continuer()
    {
        conteneur.SetActive(false);
        Time.timeScale = 1;
    }

    /**
     * Renvoie au menu principal
     */
    public void MenuPrincipal()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void Quitter()
    {
        Application.Quit();
    }
}
