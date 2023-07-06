using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowsManager : MonoBehaviour
{
    public Window radVis, radVisError, radVisContent, radVisMessage, radVisCalculating, radVisSolution;


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

        radVisMessage.OpenWindow();
        yield return new WaitForSeconds(3f);

        radVisMessage.CloseWindow();
        yield return new WaitForSeconds(1f);

        radVisContent.OpenWindow();
        radVisContent.StartWorkSequence();
        radVisError.OpenWindow();
        yield return new WaitForSeconds(5f);

        radVisError.CloseWindow();
        radVisCalculating.OpenWindow();
        radVisCalculating.FlickeringOn();
        yield return new WaitForSeconds(7f);

        radVisCalculating.CloseWindow();
        radVisContent.CloseWindow();
        radVisSolution.OpenWindow();
        yield return new WaitForSeconds(10f);

        

        radVis.gameObject.SetActive(false);
        radVisError.gameObject.SetActive(false);
        radVisContent.gameObject.SetActive(false);
        radVisMessage.gameObject.SetActive(false);
        radVisCalculating.gameObject.SetActive(false);
        radVisSolution.gameObject.SetActive(false);


        //        radVisError.FlickeringOn();
    }
}
