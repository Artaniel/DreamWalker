using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class FogController : MonoBehaviour
{
    public float fogMaxDensity, fogMinDensity, maxAltitude, minAltitude;
    public Transform trackingPoint;

    void Update()
    {
        float currentY = Mathf.Clamp(trackingPoint.position.y, minAltitude, maxAltitude);
        float curentDensity = math.remap(minAltitude, maxAltitude, fogMaxDensity, fogMinDensity, currentY);
        RenderSettings.fogDensity = curentDensity;
    }
}
