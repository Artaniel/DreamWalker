using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WallCar : MonoBehaviour
{
    //bug задний ход стопорится в ноль и дрожит
    // проверить Debug.Ray 

    public Transform[] groundCheckPoints;
    private bool isOnSurface = false;
    private float speed = 0f;
    public float maxSpeed = 5f;
    public float acceleration = 2f;
    private float verticalRotation = 0f;
    public float senetivity = 1f;
    public Transform cameraHolder;
    private Rigidbody carRigidbody;

    private void Awake()
    {
        carRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        SurfaceCheck();
        if (isOnSurface)
            Move();
        MouseTurn();
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
               
            }
            else {
                Debug.DrawRay(groundCheckPoint.position, -groundCheckPoint.up, Color.red);
            }
        }
        isOnSurface = touchPointFound;
        
    }

    private void Move() {
        if (Keyboard.current.wKey.isPressed)
            speed = Mathf.Clamp(speed + acceleration * Time.deltaTime, -maxSpeed, maxSpeed);
        else if (Keyboard.current.sKey.isPressed)
            speed = Mathf.Clamp(- speed + acceleration * Time.deltaTime, -maxSpeed, maxSpeed);
        else
            speed = Mathf.Clamp(speed - acceleration * Time.deltaTime, 0, maxSpeed);
        //transform.position += transform.forward * speed * Time.deltaTime;
        carRigidbody.velocity = transform.forward * speed;
    }

    private void MouseTurn() {
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + Mouse.current.delta.value.x * senetivity * Time.deltaTime, 0);
        verticalRotation = Mathf.Clamp(verticalRotation - Mouse.current.delta.value.y * senetivity * Time.deltaTime, 0f, 90f);
        cameraHolder.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }
}
