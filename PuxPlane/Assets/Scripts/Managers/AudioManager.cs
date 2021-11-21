using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    // Constants
    private const string BACKGROUND_VOLUME = "BackgroundVolume";
    private const string SFX_VOLUME = "SFXVolume";

    private const string MIXER_PATH = "AudioMixers/MainMixer";

    // References
    private AudioMixer _mixer;


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnInit()
    {
        if (!Exists)
        {
            //new GameObject("AudioManager", typeof(AudioManager));
            Instantiate(Resources.Load("Managers/AudioManager", typeof(AudioManager))).name = "AudioManager";
        }
    }

    protected override void OnSingletonAwake()
    {
        // AudioManager should exist for the lifetime of the game.
        isPersistent = true;

        _mixer = Resources.Load<AudioMixer>(MIXER_PATH);
        if (_mixer == null)
        {
            Debug.LogError($"AudioManager: MainMixer not found. Ensure \"MainMixer\" is located in Resources/{MIXER_PATH}.");
            return;
        }        
    }

    private void Start()
    {
        // Set starting volume from GameSettings.
        // I don't know why but the Mixer doesn't update when this is run in OnSingletonAwake(), even though the correct values are saved in GameSettings.
        // I'm guessing some time needs to pass after loading the Mixer in OnSingletonAwake, before the mixer updates/responds correctly?
        SetBackgroundVolume(this.GetGameSettings().backgroundVolume);
        SetSFXVolume(this.GetGameSettings().sfxVolume);
    }

    public void SetBackgroundVolume(float volume)
    {
        // Clamp and set volume
        float dB = Mathf.Clamp(GetDecibelVolume(volume), -80, 20f);
        _mixer.SetFloat(BACKGROUND_VOLUME, dB);

        // Save new volume in GameSettings
        this.GetGameSettings().backgroundVolume = dB;
    }

    public float GetBackgroundVolume()
    {
        _mixer.GetFloat(BACKGROUND_VOLUME, out float volume);
        return GetLinearVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        // Clamp and set volume
        float dB = Mathf.Clamp(GetDecibelVolume(volume), -80, 20f);
        _mixer.SetFloat(SFX_VOLUME, dB);

        // Save new volume in GameSettings
        this.GetGameSettings().sfxVolume = dB;
    }

    public float GetSFXVolume()
    {
        _mixer.GetFloat(SFX_VOLUME, out float volume);
        return GetLinearVolume(volume);
    }

    private float GetDecibelVolume(float linearVolume)
    {
        return linearVolume <= 0f ? 0.0001f : 20f * Mathf.Log10(linearVolume);
    }

    private float GetLinearVolume(float decibelVolume)
    {
        return Mathf.Pow(10, decibelVolume / 20f);
    }
}
