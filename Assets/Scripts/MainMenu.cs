using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public void Resume(){
        // if (pas de sauvegarde)
        SceneManager.LoadScene("SampleScene");
        // else
        // Prends la derniere sauvegarde
    }

    /**
    * number (int) : 0,1 ou 2 - Indique le numéro de la sauvegarde
    */
    public void Saves(int number){
        SaveManager.currentSlot = number;
        SaveData data = SaveManager.SlotInfo();
        if (data == null){
            return; // Si la sauvegarde est vide, on ne fait rien
        }
        SaveManager.Load();
    }

    public void Parameters(){
        
    }

    public void QuitGame(){
        Application.Quit();
    }
}
