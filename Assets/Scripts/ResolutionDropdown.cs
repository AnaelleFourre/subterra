using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ResolutionDropdown : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown; // Ou Dropdown si pas TMPro
    
    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;
    
    void Start()
    {
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();
        
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        
        int currentResolutionIndex = 0;
        
        // Filtrer pour éviter les doublons (même résolution, différents refresh rates)
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            
            // Vérifier si cette résolution n'est pas déjà dans la liste
            bool isDuplicate = false;
            foreach (Resolution res in filteredResolutions)
            {
                if (res.width == resolutions[i].width && res.height == resolutions[i].height)
                {
                    isDuplicate = true;
                    break;
                }
            }
            
            if (!isDuplicate)
            {
                filteredResolutions.Add(resolutions[i]);
                options.Add(option);
                
                // Vérifier si c'est la résolution actuelle
                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = filteredResolutions.Count - 1;
                }
            }
        }
        
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }
    
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}