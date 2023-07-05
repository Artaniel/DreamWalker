using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Window : MonoBehaviour
{
    Image image;
    public bool useWorkSequence = false, randomWorkSequence = false;
    public float workFrequency = 3f;
    public Sprite window;
    public Sprite[] openSequence, workSequence;
    public float openSequenceDuration = 1f;
    public bool isOpen = false, isFlickering = false, isMuted = false;

 
    public float frequency = 3f;

    Color colMute = new Color(0, 0, 0, 0), colUnmute = new Color(255, 255, 255, 1);

    void Start()
    {
        image = GetComponent<Image>();
        image.sprite = window;
        Mute();

    }



    IEnumerator WindowFlickerCoroutine(float frequency)
    {
        isFlickering = true;
        while (isFlickering)
        {
            Mute();
            yield return new WaitForSeconds(1f/frequency);
            Unmute();
            yield return new WaitForSeconds(1f / frequency);
        }
    }

    IEnumerator OpenWindowCoroutine()
    {
        isOpen = true;
        Unmute();
        for (int i=0; i<openSequence.Length; i++)
        {
            image.sprite = openSequence[i];
            yield return new WaitForSeconds(openSequenceDuration/openSequence.Length);
        }
        image.sprite = window;
    }

    IEnumerator CloseWindowCoroutine()
    {
        for (int i = openSequence.Length-1; i > -1; i--)
        {
            image.sprite = openSequence[i];
            yield return new WaitForSeconds(openSequenceDuration / openSequence.Length);
        }
        Mute();
        isOpen = false;
    }

    IEnumerator StartWorkSequence(float frequency)
    {
        int i = 0;
        while (isOpen)
        {
            image.sprite = workSequence[i];
            i++;
            if (i == workSequence.Length) { i = 0; }
            yield return new WaitForSeconds(1 / frequency);
        }
    }

    IEnumerator StartRandomWorkSequence(float frequency)
    {
        int i = 0;
        int ip = 0;
        while (isOpen)
        {
            i = Random.Range(0, workSequence.Length);
            if (i == ip) { i++; }
            if (i >= workSequence.Length) { i = 0; }
            ip = i;
            image.sprite = workSequence[i];
            yield return new WaitForSeconds(1 / frequency);
        }
    }

    public void StartWorkSequence()
    {
        if (useWorkSequence)
        {
            if (randomWorkSequence)
            {
                StartCoroutine(StartRandomWorkSequence(workFrequency));
            }
            else
            {
                StartCoroutine(StartWorkSequence(workFrequency));
            }
        }
    }
    public void OpenWindow()
    {
        StartCoroutine(OpenWindowCoroutine());
    }
    public void CloseWindow()
    {
        StartCoroutine(CloseWindowCoroutine());
    }

    public void FlickeringOn(float frequency)
    {
        StartCoroutine(WindowFlickerCoroutine(frequency));
    }
    public void FlickeringOff()
    {
        isFlickering = false;
    }

    public void Mute()
    {
        isMuted = true;
        //image.color = colMute;
        image.enabled = false;
    }

    public void Unmute()
    {
        isMuted = false;
        //image.color = colUnmute;
        image.enabled = true;
    }

}
