using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WallCar : MonoBehaviour
{
    //bug задний ход стопорится в ноль и дрожит

    public Transform[] groundCheckPoints;
    private bool isOnSurface = false;
    private float speed = 0f;
    private float strafeSpeed = 0f;
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
        else
            Fall();
        MouseTurn();
    }

    private void SurfaceCheck() {
        bool touchPointFound = false;
        Vector3 connectionNormalSumm = Vector3.zero;
        foreach (Transform groundCheckPoint in groundCheckPoints)
        {
            bool thisIsTorching = false;
            Vector3 foundNormal = Vector3.zero;
            foreach (RaycastHit hit in Physics.RaycastAll(groundCheckPoint.position, -groundCheckPoint.up, 1f)) 
                if (hit.collider.tag != "Player")
                {
                    thisIsTorching = true;
                    foundNormal = hit.normal;
                    break;
                }
            if (thisIsTorching)
            {
                touchPointFound = true;
                connectionNormalSumm += foundNormal;
                Debug.DrawRay(groundCheckPoint.position, -groundCheckPoint.up, Color.green);
            }
            else
            {
                Debug.DrawRay(groundCheckPoint.position, -groundCheckPoint.up, Color.red);
            }

        }
        isOnSurface = touchPointFound;
        
    }

    private void Move() {
        if (Keyboard.current.wKey.isPressed)
            speed = Mathf.Clamp(speed + acceleration * Time.deltaTime, -maxSpeed, maxSpeed);
        else if (Keyboard.current.sKey.isPressed)
            speed = Mathf.Clamp(speed - acceleration * Time.deltaTime, -maxSpeed, maxSpeed);
        else
            speed = 0.98f * speed; 

        if (Keyboard.current.aKey.isPressed)
            strafeSpeed = Mathf.Clamp(strafeSpeed + acceleration * Time.deltaTime, -maxSpeed, maxSpeed);

        //transform.position += transform.forward * speed * Time.deltaTime;
        carRigidbody.velocity = transform.forward * speed;
    }

    private void Fall() {
        carRigidbody.velocity += Vector3.down * Time.deltaTime * 9.8f;
    }

    private void MouseTurn() {
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + Mouse.current.delta.value.x * senetivity * Time.deltaTime, 0);
        verticalRotation = Mathf.Clamp(verticalRotation - Mouse.current.delta.value.y * senetivity * Time.deltaTime, 0f, 90f);
        cameraHolder.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }
}
