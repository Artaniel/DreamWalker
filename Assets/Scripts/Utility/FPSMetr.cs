using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSMetr : MonoBehaviour
{
    public TextMeshProUGUI FPSText;
    private int index = 0;
    private const int size = 50;
    private float[] lastRecords = new float[size];

    private void Update()
    {
        lastRecords[index] = 1 / Time.deltaTime;
        float summ = 0;
        for (int i = 0; i < size; i++)
            summ += lastRecords[i];

        FPSText.text = $"FPS {(int)(1 / Time.deltaTime)} / {(int)(summ / size)} / {(int)(1 / Time.fixedDeltaTime)}";

        index++;
        if (index >= size) index = 0;
    }
}
