using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    public Transform cameraTargetTransform;
    public Transform holder;
    public Transform freeCameraPivot;
    public Transform carTransform;
    [HideInInspector] public bool freeMode = false;
    private Vector3 savedLocalPosition;

    public float senetivity = 20f;
    private float verticalRotation = 0f;

    public Transform trueCameraTransform;
    public float LERPFactor = 0.1f;
    private float currentLERPFactor = 1f;

    public List<Renderer> carRenderers;

    private void Awake()
    {
        savedLocalPosition = cameraTargetTransform.localPosition;
        currentLERPFactor = LERPFactor;
    }

    void FixedUpdate()
    {
        if (Mouse.current.rightButton.IsPressed() && !freeMode)
            SwichToFreeMode();
        else if (!Mouse.current.rightButton.IsPressed() && freeMode)
            SwitchToNormalMode();

        if (freeMode)
            FreeModeUpdate();
        else
            NormalModeUpdate();

        trueCameraTransform.position = Vector3.Lerp(trueCameraTransform.position, cameraTargetTransform.position, currentLERPFactor);
        trueCameraTransform.rotation = Quaternion.Lerp(trueCameraTransform.rotation, cameraTargetTransform.rotation, LERPFactor);
    }

    private void SwichToFreeMode() {
        freeMode = true;
        cameraTargetTransform.position = freeCameraPivot.position;
        cameraTargetTransform.parent = transform;
    }

    private void SwitchToNormalMode()
    {
        currentLERPFactor = LERPFactor;
        freeMode = false;
        cameraTargetTransform.parent = holder;
        cameraTargetTransform.localPosition = savedLocalPosition;
        cameraTargetTransform.localRotation = Quaternion.identity;
        foreach (Renderer carRenderer in carRenderers)
            carRenderer.enabled = true;
    }

    private void FreeModeUpdate()
    {
        if (currentLERPFactor < 1)
            currentLERPFactor += 0.05f;
        else
        {
            currentLERPFactor = 1;
            foreach (Renderer carRenderer in carRenderers)
                carRenderer.enabled = false;
        }
        carTransform.Rotate(transform.up, Mouse.current.delta.value.x * senetivity * Time.deltaTime, Space.World);
        verticalRotation = Mathf.Clamp(verticalRotation - Mouse.current.delta.value.y * senetivity * Time.deltaTime, -90f, 90f);
        cameraTargetTransform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private void NormalModeUpdate()
    {
        carTransform.Rotate(transform.up, Mouse.current.delta.value.x * senetivity * Time.deltaTime, Space.World);
        verticalRotation = Mathf.Clamp(verticalRotation - Mouse.current.delta.value.y * senetivity * Time.deltaTime, 0f, 90f);
        holder.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }
}
