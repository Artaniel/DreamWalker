using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneCamera : MonoBehaviour
{
    public float cameraLerpFactor, mouseSensitivity;
    public Transform zeroPoint;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void FixedUpdate()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 screenDimensions = new Vector2(Screen.width, Screen.height);
        Vector2 mouseRelativePos = mousePos - screenDimensions / 2;
        Vector3 mouseAdjustedPos = new Vector3(mouseRelativePos.x, mouseRelativePos.y, 0) * mouseSensitivity;
        Vector3 currentTarget = mouseAdjustedPos + zeroPoint.position;
        Vector3 newCameraPosition = new Vector3(Mathf.Lerp(transform.position.x, currentTarget.x, cameraLerpFactor), Mathf.Lerp(transform.position.y, currentTarget.y, cameraLerpFactor), transform.position.z);
        transform.position = newCameraPosition;
    }

}
