using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredictorMarkerBillboard : MonoBehaviour
{
    private Transform cameraTransform;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        transform.LookAt(cameraTransform);
    }
}
