using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public AudioMixer mixer;
    static public SoundManager instance;
    private float maxMusicVolume = 10f;
    private float maxSFXVolume = 10f;


    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            Init();
        }
        else
            Destroy(gameObject);
    }


    private void Init()
    {
        DontDestroyOnLoad(gameObject);        
        mixer.GetFloat("Music", out maxMusicVolume);
        mixer.GetFloat("SFX", out maxSFXVolume);
        float _SFXvolume = PlayerPrefs.GetFloat("SFX");
        float _musicvolume = PlayerPrefs.GetFloat("Music");
        mixer.SetFloat("SFX", _SFXvolume);
        mixer.SetFloat("Music", _musicvolume);
    }

    public static void ChangeVolumeSFX(float value)
    {
        instance?.mixer.SetFloat("SFX", value); 
        PlayerPrefs.SetFloat("SFX", value);
    }

    public static void ChangeVolumeMusic(float value)
    {
        instance?.mixer.SetFloat("Music", value);
        PlayerPrefs.SetFloat("Music", value);
    }
}
