using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public AudioMixer mixer;
    static public SoundManager instance;
    public float maxMusicVolume = 1f;
    public float maxSFXVolume = 1f;
    public float SFXVolume;
    public float musicVolume;


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
        SFXVolume = PlayerPrefs.GetFloat("SFX");        
        musicVolume = PlayerPrefs.GetFloat("Music");
        mixer.SetFloat("SFX", SFXVolume);
        mixer.SetFloat("Music", musicVolume);
    }

    public static void ChangeVolumeSFX(float value)
    {
        if (instance)
        {
            instance.SFXVolume = value;
            instance.mixer.SetFloat("SFX", value);
        }
        PlayerPrefs.SetFloat("SFX", value);
    }

    public static void ChangeVolumeMusic(float value)
    {
        if (instance)
        {
            instance.musicVolume = value;
            instance.mixer.SetFloat("Music", value);
        }
        PlayerPrefs.SetFloat("Music", value);
    }
}
