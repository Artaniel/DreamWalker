using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCar : MonoBehaviour
{
    public Transform[] groundCheckPoints;
    private bool isOnSurface = false;

    private void Awake()
    {
        
    }

    private void Update()
    {
        SurfaceCheck();
    }

    private void SurfaceCheck() {
        bool touchPointFound = false;
        Vector3 connectionNormalSumm = Vector3.zero;
        foreach (Transform groundCheckPoint in groundCheckPoints)
        {
            RaycastHit hit;
            if (Physics.SphereCast(groundCheckPoint.position, 0.1f, -groundCheckPoint.up, out hit))
            {
                touchPointFound = true;
                connectionNormalSumm += hit.normal;
                Debug.DrawRay(groundCheckPoint.position, - groundCheckPoint.up, Color.green);
            }
        }
        isOnSurface = touchPointFound;
        
    }
}
