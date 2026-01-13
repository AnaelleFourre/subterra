using UnityEngine;
using UnityEngine.UI;

public class LightSlider : MonoBehaviour
{
    [SerializeField] private Slider brightnessSlider;
     private Image brightnessPanel;
    
    void Start()
    {

        GameObject panelObject = GameObject.Find("BrightnessPanel"); // Change le nom selon ton panneau
        
        if (panelObject != null)
        {
            brightnessPanel = panelObject.GetComponent<Image>();
        }

        PlayerPrefs.DeleteKey("Brightness");
    
        float savedBrightness = PlayerPrefs.GetFloat("Brightness", 1f - brightnessPanel.color.a);
        brightnessSlider.value = savedBrightness;
        SetBrightness(savedBrightness);
        
        brightnessSlider.onValueChanged.AddListener(SetBrightness);
    }
    
    public void SetBrightness(float value)
    {
        Color panelColor = brightnessPanel.color;
        panelColor.a = 1f - value;
        brightnessPanel.color = panelColor;
        
        PlayerPrefs.SetFloat("Brightness", value);
    }
}
