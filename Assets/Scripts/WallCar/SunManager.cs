using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunManager : MonoBehaviour
{
    public float azimut = 0f;
    public float ascention = 45f;

    public Transform lightSource;


    private void Update()
    {
        RefreshLight();
    }

    private void RefreshLight() {
        lightSource.transform.rotation = Quaternion.Euler(ascention, azimut, 0);
    }

    public bool IsUnderLight(Vector3 position) {
        RaycastHit[] hits = Physics.RaycastAll(position, -lightSource.forward);
        foreach (RaycastHit hit in hits)
            if (hit.transform.tag != "Player")
                return false;
        return true;
    }
}
