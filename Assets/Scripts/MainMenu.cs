using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Resume(){
        // if (pas de sauvegarde)
        SceneManager.LoadScene("SampleScene");
        // else
        // Prends la derniere sauvegarde
    }

    public void Saves(){
        // SceneManager.LoadScene("SavesScene"); // Va à la scene où sont les sauvegarde
    }

    public void QuitGame(){
        Application.Quit();
    }
}
