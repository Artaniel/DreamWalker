using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarCamera : MonoBehaviour
{
    public WallCar wallCar;
    public float senetivity = 1f;
    private float verticalRotation = 0f;
    public Transform cameraHolder;

    private void Awake()
    {
        if (!wallCar)
            wallCar = GetComponent<WallCar>();
    }

    private void Update()
    {
        RotateUp();
        MouseTurn();
        cameraHolder.transform.position = wallCar.transform.position;
    }

    private void MouseTurn()
    {
        cameraHolder.localRotation = Quaternion.Euler(verticalRotation, cameraHolder.localRotation.eulerAngles.y, cameraHolder.localRotation.eulerAngles.z);
        cameraHolder.Rotate(transform.up, Mouse.current.delta.value.x * senetivity * Time.deltaTime, Space.World);
        verticalRotation = Mathf.Clamp(verticalRotation - Mouse.current.delta.value.y * senetivity * Time.deltaTime, 0f, 90f);
    }

    private void RotateUp() {
        Vector3 targetDirection = wallCar.transform.up;
        float angle = Vector3.Angle(cameraHolder.up, targetDirection);
        Vector3 axis = Vector3.Cross(cameraHolder.up, targetDirection);
        cameraHolder.Rotate(axis, angle, Space.World);
    }
}
