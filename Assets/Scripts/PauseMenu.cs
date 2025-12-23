using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


// NE PAS OUBLIER !!!!!!
// Ajouter un EventSystem avec Input System UI Input Module de base
// Le player à l'air d'être un EventSystem, alors le Input System UI Input Module doit être
// ajouté comme composant de Player
public class PauseMenu : MonoBehaviour
{

    public GameObject conteneur;

    /**
    * Si la touche ESC est appuyé, met en pause et affiche le menu
    */
    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame){
            conteneur.SetActive(true);
            Time.timeScale = 0;
        }
    }

    /**
    * Retire le menu et relance le jeu
    */ 
    public void Continuer(){
        conteneur.SetActive(false);
        Time.timeScale = 1;
    }

    /**
    * Renvoie au menu principal
    */
    public void MenuPrincipal(){
        SceneManager.LoadScene("MainMenu");
    }

    public void Quitter(){
        Application.Quit();
    }

}
