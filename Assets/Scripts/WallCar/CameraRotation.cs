using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotation : MonoBehaviour
{
    public Transform cameraTransform;
    public Transform holder;
    public Transform freeCameraPivot;
    public Transform carTransform;
    private bool freeMode = false;
    private Vector3 savedLocalPosition;

    public float senetivity = 20f;
    private float verticalRotation = 0f;

    private void Awake()
    {
        savedLocalPosition = cameraTransform.localPosition;
    }

    void Update()
    {
        if (Mouse.current.rightButton.IsPressed() && !freeMode)
            SwichToFreeMode();
        else if (!Mouse.current.rightButton.IsPressed() && freeMode)
            SwitchToNormalMode();

        if (freeMode)
            FreeModeUpdate();
        else
            NormalModeUpdate();
    }

    private void SwichToFreeMode() {
        freeMode = true;
        cameraTransform.position = freeCameraPivot.position;
        cameraTransform.parent = transform;
    }

    private void SwitchToNormalMode()
    {
        freeMode = false;
        cameraTransform.parent = holder;
        cameraTransform.localPosition = savedLocalPosition;
        cameraTransform.localRotation = Quaternion.identity;
    }

    private void FreeModeUpdate()
    {
        carTransform.Rotate(transform.up, Mouse.current.delta.value.x * senetivity * Time.deltaTime, Space.World);
        verticalRotation = Mathf.Clamp(verticalRotation - Mouse.current.delta.value.y * senetivity * Time.deltaTime, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private void NormalModeUpdate()
    {
        carTransform.Rotate(transform.up, Mouse.current.delta.value.x * senetivity * Time.deltaTime, Space.World);
        verticalRotation = Mathf.Clamp(verticalRotation - Mouse.current.delta.value.y * senetivity * Time.deltaTime, -90f, 90f);
        holder.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }
}
