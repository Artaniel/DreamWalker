using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowsManager : MonoBehaviour
{
    public Window radVis, radVisError, radVisContent;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            StartCoroutine(RadVisSequence());
        }

    }

    IEnumerator RadVisSequence()
    {
        radVis.OpenWindow();
        yield return new WaitForSeconds(1f);
        radVisContent.OpenWindow();
        radVisContent.StartWorkSequence();
        radVisError.OpenWindow();
        radVisError.FlickeringOn(1f);
    }
}
