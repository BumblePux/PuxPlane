using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : HUD
{
    [Header("References")]
    [SerializeField]
    private Slider backgroundVolumeSlider;
    [SerializeField]
    private Slider sfxVolumeSlider;


    private void OnEnable()
    {
        backgroundVolumeSlider.value = AudioManager.Instance.GetBackgroundVolume();
        sfxVolumeSlider.value = AudioManager.Instance.GetSFXVolume();
    }

    public void OnBackgroundVolumeChanged(float volume)
    {
        AudioManager.Instance.SetBackgroundVolume(volume);
    }

    public void OnSFXVolumeChanged(float volume)
    {
        AudioManager.Instance.SetSFXVolume(volume);
    }
}
