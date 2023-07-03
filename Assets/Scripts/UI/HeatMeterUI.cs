using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeatMeterUI : MonoBehaviour
{
    public Slider slider;
    public static HeatMeterUI instance;

    private void Awake()
    {
        if (!instance)
            instance = this;
    }

    public static void RefreshHeatMeter(float value) {        
        if (instance && instance.slider)
            instance.slider.value = value;
    }
}
