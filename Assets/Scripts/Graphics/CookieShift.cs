using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CookieShift : MonoBehaviour
{
    public Vector2 shiftSpeed;
    UniversalAdditionalLightData lightData;
    // Start is called before the first frame update
    void Start()
    {
        lightData = GetComponent<UniversalAdditionalLightData>();
    }

    // Update is called once per frame
    void Update()
    {
        lightData.lightCookieOffset += shiftSpeed * Time.deltaTime;
    }
}
