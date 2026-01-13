using UnityEngine;
using UnityEngine.UI;

public class LightSlider : MonoBehaviour
{
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Camera mainCamera;
    
    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        
        // Charger la valeur sauvegardée
        float savedBrightness = PlayerPrefs.GetFloat("Brightness", 1f);
        brightnessSlider.value = savedBrightness;
        SetBrightness(savedBrightness);
        
        brightnessSlider.onValueChanged.AddListener(SetBrightness);
    }
    
    public void SetBrightness(float value)
    {
        // Ajuster la couleur de fond de la caméra
        // value entre 0 et 1
        mainCamera.backgroundColor = new Color(value, value, value, 1f);
        
        PlayerPrefs.SetFloat("Brightness", value);
    }
}
