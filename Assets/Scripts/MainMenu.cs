using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;

public class MainMenu : MonoBehaviour
{

    /**
    * number (int) : 0,1 ou 2 - Indique le numéro de la sauvegarde
    * Charge la sauvegarde {number}
    */
    public void Saves(int number)
    {
        SaveManager.SetSlot(number);
        SaveData data = SaveManager.SlotInfo();
        if (data == null)
        {
            Debug.Log("Avant InitSave - MainMenu");
            SaveManager.InitSave("Save n°" + (number + 1), "SampleScene");
            Debug.Log("Après InitSave - MainMenu" + CheckpointManager.instance.GetDefaultSpawn());
        }
        SaveManager.Load();
    }

    /**
    * Charge la page des paramètres
    *
    */
    public void Parameters()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
