using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Radar : MonoBehaviour
{
    public WallCar car;
    public TextMeshProUGUI text;
    public bool isEnabled = true;
    public GameObject source;

    private void Update()
    {
        if (isEnabled && car && text && source)
            text.text = $"Distance to signal source {((int)Vector3.Distance(car.transform.position, source.transform.position)).ToString()} m";
        else if (text)
            text.text = "";
    }
}
