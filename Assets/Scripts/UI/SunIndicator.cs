using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SunIndicator : MonoBehaviour
{
    public Image image;
    public SpiderHealth spiderHealth;


    void Update()
    {
        image.enabled = spiderHealth.isUnderSun;
    }
}
