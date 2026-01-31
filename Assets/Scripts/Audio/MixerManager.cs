using UnityEngine;
using UnityEngine.Audio;

public class MixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public void SetMasterVolume(float level) => audioMixer.SetFloat("masterVolume", level);

    public void SetMusicVolume(float level) => audioMixer.SetFloat("musicVolume", level);

    public void SetSFXVolume(float level) => audioMixer.SetFloat("SFXVolume", level);
}
