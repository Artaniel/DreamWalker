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
    private Quaternion savedLocalRotation;

    public float senetivity = 20f;
    private float verticalRotation = 0f;

    public Transform trueCameraTransform;
    public float LERPFactorLinar = 0.1f;
    public float LERPFactorRotation = 0.1f;
    private float currentLERPFactor = 1f;

    public float minRenderDist = 2f;
    public List<Renderer> carRenderers;
    private bool lastFrameVisible = true;

    private void Awake()
    {
        savedLocalPosition = cameraTargetTransform.localPosition;
        savedLocalRotation = cameraTargetTransform.localRotation;
        currentLERPFactor = LERPFactorLinar;
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
        trueCameraTransform.rotation = Quaternion.Lerp(trueCameraTransform.rotation, cameraTargetTransform.rotation, LERPFactorRotation);
        ModelVisibilityCheck();
    }

    private void SwichToFreeMode() {
        freeMode = true;
        cameraTargetTransform.position = freeCameraPivot.position;
        cameraTargetTransform.parent = transform;
    }

    private void SwitchToNormalMode()
    {
        currentLERPFactor = LERPFactorLinar;
        freeMode = false;
        cameraTargetTransform.parent = holder;
        cameraTargetTransform.localPosition = savedLocalPosition;
        cameraTargetTransform.localRotation = savedLocalRotation;
    }

    private void FreeModeUpdate()
    {
        if (currentLERPFactor < 1)
            currentLERPFactor += 0.05f;
        else
        {
            currentLERPFactor = 1;
        }
        if (!MouseLock.settingsIsOpen)
        {
            carTransform.Rotate(transform.up, Mouse.current.delta.value.x * senetivity * Time.deltaTime, Space.World);
            verticalRotation = Mathf.Clamp(verticalRotation - Mouse.current.delta.value.y * senetivity * Time.deltaTime, -90f, 90f);
            cameraTargetTransform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        }
    }

    private void NormalModeUpdate()
    {
        if (!MouseLock.settingsIsOpen)
        {
            carTransform.Rotate(transform.up, Mouse.current.delta.value.x * senetivity * Time.deltaTime, Space.World);
            verticalRotation = Mathf.Clamp(verticalRotation - Mouse.current.delta.value.y * senetivity * Time.deltaTime, -90f, 90f);
            holder.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        }
        RaycastHit[] hits = Physics.RaycastAll(carTransform.position, cameraTargetTransform.position - carTransform.position, -savedLocalPosition.z);
        float minDist = Mathf.Infinity;
        RaycastHit result = new RaycastHit();
        foreach (RaycastHit hit in hits) {
            if (hit.transform.tag != "Player" && Vector3.Distance(hit.point, carTransform.position) < minDist)
            {
                result = hit;
                minDist = Vector3.Distance(hit.point, carTransform.position);
            }
        }
        if (minDist < Mathf.Infinity)
            cameraTargetTransform.localPosition = new Vector3(cameraTargetTransform.localPosition.x, cameraTargetTransform.localPosition.y,
                -Vector3.Distance(result.point, carTransform.position));
        else
            cameraTargetTransform.localPosition = savedLocalPosition;
    }

    private void ModelVisibilityCheck()
    {
        if (lastFrameVisible && Vector3.Distance(carTransform.position, trueCameraTransform.position) < minRenderDist)
        {
            lastFrameVisible = false;
            foreach (Renderer carRenderer in carRenderers)
                carRenderer.enabled = false;
        }
        else if (!lastFrameVisible && Vector3.Distance(carTransform.position, trueCameraTransform.position) > minRenderDist)
        {
            lastFrameVisible = true;
            foreach (Renderer carRenderer in carRenderers)
                carRenderer.enabled = true;
        }

    }
}
