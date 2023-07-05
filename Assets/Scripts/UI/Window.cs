using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Window : MonoBehaviour
{
    Image image;
    public Sprite window;
    public Sprite[] openSequence;
    public float openStagesTime=.1f;
    public bool windowIsOpen = false;

    void Start()
    {
        image = GetComponent<Image>();
        image.sprite = window;
        image.color = new Color(0, 0, 0, 0);

    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (!windowIsOpen) { OpenWindow(); windowIsOpen = true; }
            else { CloseWindow(); windowIsOpen = false; }
            
        }
    }

    IEnumerator OpenWindowCoroutine()
    {
        image.color = new Color(255,255,255,1);
        for (int i=0; i<openSequence.Length; i++)
        {
            image.sprite = openSequence[i];
            yield return new WaitForSeconds(openStagesTime);
        }
        image.sprite = window;
    }

    IEnumerator CloseWindowCoroutine()
    {

        for (int i = openSequence.Length-1; i > -1; i--)
        {
            image.sprite = openSequence[i];
            yield return new WaitForSeconds(openStagesTime);
        }
        image.color = new Color(0, 0, 0, 0);
    }

    void OpenWindow()
    {
        StartCoroutine(OpenWindowCoroutine());
    }
    void CloseWindow()
    {
        StartCoroutine(CloseWindowCoroutine());
    }
}
