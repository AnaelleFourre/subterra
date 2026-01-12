using UnityEngine;
using UnityEngine.Audio;

public class MixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public void SetMasterVolume(float level) {
        audioMixer.setFloat("masterVolume", level)
    }

    public void SetMusicVolume(float level) {
        audioMixer.setFloat("musicVolume", level)
    }

    public void SetSFXVolume(float level) {
        audioMixer.setFloat("SFXVolume", level)
    }
}
