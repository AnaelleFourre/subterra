using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.IO;

public class MainMenu : MonoBehaviour
{
    private const string Level1SceneName = "Level1-remake";
    [Header("Scene Dropdown")]
    public TMP_Dropdown sceneDropdown;

    private List<string> sceneNames = new List<string>();

    void Start()
    {
        Time.timeScale = 1;
        InitSceneDropdown();
    }

    void InitSceneDropdown()
    {
        sceneDropdown.ClearOptions();
        sceneNames.Clear();

        int count = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < count; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = Path.GetFileNameWithoutExtension(path);

            if (name != "MainMenu")
            {
                sceneNames.Add(name);
            }
        }

        sceneDropdown.AddOptions(sceneNames);
    }

    /**
    * number (int) : 0,1 ou 2 - Indique le numéro de la sauvegarde
    * Charge la sauvegarde {number}
    */
    public void Saves(int number)
    {
        SaveManager.SetSlot(number);
        SaveData data = SaveManager.SlotData(number);
        if (data == null)
        {
            SaveManager.InitSave("Save n°" + (number + 1), Level1SceneName);
        }
        SaveManager.Load();
    }

    string GetSelectedScene()
    {
        if (sceneDropdown == null || sceneNames.Count == 0)
        {
            return Level1SceneName;
        }

        return sceneNames[sceneDropdown.value];
    }

    /**
    * Charge directement la scène sélectionnée
    */
    public void PlaySceneDirect()
    {
        string scene = GetSelectedScene();
        SaveManager.SetSlot(2);
        SaveData data = SaveManager.SlotData(2);
        if (data != null)
        {
            SaveManager.DeleteSave(2);
        }
        SaveManager.InitSave("Save n°" + 3, scene);
        SaveManager.Load();
    }

    /**
    * Charge la page des paramètres
    */
    public void Parameters()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
