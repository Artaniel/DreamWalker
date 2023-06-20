using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public AudioMixer mixer;
    static public SoundManager instance;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }


    public static void ChangeVolumeSFX(float value)
    {
        instance?.mixer.SetFloat("SFX", value);
    }

    public static void ChangeVolumeMusic(float value)
    {
        instance?.mixer.SetFloat("Music", value);
    }
}
