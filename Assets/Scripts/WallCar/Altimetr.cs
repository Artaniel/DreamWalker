using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Altimetr : MonoBehaviour
{
    public WallCar car;
    public TextMeshProUGUI altiText;
    public bool isEnabled = true;

    private void Update()
    {
        if (isEnabled && car && altiText)
            altiText.text = $"Altitude {((int)car.transform.position.y).ToString()} m";
        else if (altiText)
            altiText.text = "";
        if (car && car.transform.position.y > 1000)
            NarrativeManager.Over1000mReached();
    }

}
