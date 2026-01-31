using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.IO;
using UnityEngine.EventSystems;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    private const string Level1SceneName = "Level0";

    [SerializeField] private TMP_Dropdown sceneDropdown;
    [SerializeField] private GameObject firstButtonAutoSelected;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject parametersMenu;

    private List<string> sceneNames = new List<string>();

    void Start()
    {
        Time.timeScale = 1;
        InitSceneDropdown();

        if (sceneDropdown.options.Count > 0)
        {
            sceneDropdown.value = 0;
            StartCoroutine(SelectFirstButton());
        }
    }

    private IEnumerator SelectFirstButton()
    {
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(firstButtonAutoSelected);
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
     * number (int) : 0, 1 ou 2 - Indique le numéro de la sauvegarde
     * Charge la sauvegarde {number}
     */
    public void Saves(int number)
    {
        SaveData data = SaveManager.SlotData(number);
        if (data == null)
        {
            SaveManager.InitSave("Save n°" + (number + 1), Level1SceneName, number);
        }
        SaveManager.Load(number);
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
        SaveData data = SaveManager.SlotData(2);
        if (data != null)
        {
            SaveManager.DeleteSave(2);
        }
        SaveManager.InitSave("Save n°" + 3, scene, 2);
        SaveManager.Load(2);
    }

    /**
     * Charge la page des paramètres
     */
    public void Parameters()
    {
        mainMenu.SetActive(false);
        parametersMenu.SetActive(true);
    }

    /**
     * Retourne au menu principal depuis les paramètres
     */
    public void BackToMainMenu()
    {
        parametersMenu.SetActive(false);
        mainMenu.SetActive(true);
        StartCoroutine(SelectFirstButton());
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
