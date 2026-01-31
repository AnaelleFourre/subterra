using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private const float MinDb = -50f;
    private const float MaxDb = 0f;

    private void OnEnable()
    {
        float music = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 1f);

        mainMixer.SetFloat("musicVolume", Mathf.Lerp(MinDb, MaxDb, music));
        mainMixer.SetFloat("SFXVolume", Mathf.Lerp(MinDb, MaxDb, sfx));

        musicSlider.SetValueWithoutNotify(music);
        sfxSlider.SetValueWithoutNotify(sfx);
    }

    public void SetMusicVolume(float value)
    {
        mainMixer.SetFloat("musicVolume", Mathf.Lerp(MinDb, MaxDb, value));
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        mainMixer.SetFloat("SFXVolume", Mathf.Lerp(MinDb, MaxDb, value));
        PlayerPrefs.SetFloat("SFXVolume", value);
    }
}