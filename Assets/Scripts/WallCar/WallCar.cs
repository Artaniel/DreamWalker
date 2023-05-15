using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WallCar : MonoBehaviour
{
    public Transform[] groundCheckPoints;

    private bool isOnSurface = false;
    private float speed = 0f;
    private float strafeSpeed = 0f;
    public float maxSpeed = 5f;
    public float acceleration = 2f;
    public float angularSpeed = 90f;
    private Rigidbody carRigidbody;
    [HideInInspector] public Vector3 connectionNormalSumm;

    public Transform hoverPoint;
    public float hoverHight = 0.5f;
    private Vector3 lastGroundPoint = Vector3.zero;
    public float maxDragDistance = 5f;

    private bool isFlying = false;
    private float airTimer = 0f;
    private float airTime = 0.25f;
    private bool airBlocksSurfacecheck = false;

    private CarCamera carCamera; 

    private void Awake()
    {
        carRigidbody = GetComponent<Rigidbody>();
        lastGroundPoint = transform.position - Vector3.up * 5;
        carCamera = GetComponent<CarCamera>();
    }

    private void Update()
    {
        FlyCheck();
        if (!airBlocksSurfacecheck)
        {
            SurfaceCheck();
            if (isOnSurface)
            {
                isFlying = false;
                Move();
                JumpCheck();
            }
            else
                Fall();
            if (!isFlying)
                Hover();
        }
        SurfaceRotate();
    }

    private void SurfaceCheck()
    {
        bool touchPointFound = false;
        connectionNormalSumm = Vector3.zero;

        foreach (Transform groundCheckPoint in groundCheckPoints)
        {
            bool thisIsTorching = false;
            Vector3 foundNormal = Vector3.zero;
            foreach (RaycastHit hit in Physics.RaycastAll(groundCheckPoint.position, groundCheckPoint.forward, 1f))
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
                Debug.DrawRay(groundCheckPoint.position, groundCheckPoint.forward, Color.green);
            }
            else
            {
                Debug.DrawRay(groundCheckPoint.position, groundCheckPoint.forward, Color.red);
            }
        }

        isOnSurface = touchPointFound;
    }

    private void Move() {
        Vector3 mouseForward = Vector3.ProjectOnPlane(carCamera.cameraHolder.forward, connectionNormalSumm).normalized;
        Vector3 mouseRight = - Vector3.Cross(mouseForward, connectionNormalSumm).normalized;

        if (Keyboard.current.wKey.isPressed)
            speed = Mathf.Clamp(speed + acceleration * Time.deltaTime, -maxSpeed, maxSpeed);
        else if (Keyboard.current.sKey.isPressed)
            speed = Mathf.Clamp(speed - acceleration * Time.deltaTime, -maxSpeed, maxSpeed);
        else
            speed = 0.98f * speed; //slow down from forward and back

        if (Keyboard.current.dKey.isPressed)
            strafeSpeed = Mathf.Clamp(strafeSpeed + acceleration * Time.deltaTime, -maxSpeed, maxSpeed);
        else if (Keyboard.current.aKey.isPressed)
            strafeSpeed = Mathf.Clamp(strafeSpeed - acceleration * Time.deltaTime, -maxSpeed, maxSpeed);
        else
            strafeSpeed = 0.98f * strafeSpeed;

        carRigidbody.velocity = mouseForward * speed + mouseRight * strafeSpeed;
    }

    private void Fall() {
        speed = Vector3.Dot(carRigidbody.velocity, transform.forward);
        strafeSpeed = Vector3.Dot(carRigidbody.velocity, transform.right);
        carRigidbody.velocity += Vector3.down * Time.deltaTime * 9.8f;
    }

    private void SurfaceRotate() {
        connectionNormalSumm = connectionNormalSumm.normalized;
        Debug.DrawRay(transform.position, connectionNormalSumm);
        Vector3 currentUpDirection = transform.up;
        Debug.DrawRay(transform.position, currentUpDirection, Color.black);
        float angle = Vector3.Angle(currentUpDirection, connectionNormalSumm);
        angle = Mathf.Min(angle, angularSpeed * Time.deltaTime);
        Vector3 axis = Vector3.Cross(currentUpDirection, connectionNormalSumm);
        Debug.DrawRay(transform.position, axis, Color.magenta);
        transform.Rotate(axis, angle, Space.World);

        Vector3 targetDirection = Vector3.ProjectOnPlane(carRigidbody.velocity, connectionNormalSumm).normalized;
        angle = Vector3.Angle(transform.forward, targetDirection);
        angle = Mathf.Min(angle, angularSpeed * Time.deltaTime);
        axis = Vector3.Cross(transform.forward, targetDirection);
        transform.Rotate(axis, angle, Space.World);

        Debug.DrawRay(transform.position, carRigidbody.velocity, Color.yellow);


    }

    private void Hover()
    {
        float raycastLength = 2f;
        Vector3 groundPoint = Vector3.zero;
        bool found = false;
        foreach (RaycastHit hit in Physics.RaycastAll(hoverPoint.position, hoverPoint.forward, raycastLength))
        {
            if (hit.collider.tag != "Player")
            {
                groundPoint = hit.point;
                found = true;
                break;
            }
        }
        if (!found)
            groundPoint = lastGroundPoint;
        else
            lastGroundPoint = groundPoint;

        if (lastGroundPoint != Vector3.zero) // case of point lost, to prevent pull to point saved before long jump
        {
            float distance = Vector3.Distance(groundPoint, hoverPoint.position);
            if (distance <= maxDragDistance)
                carRigidbody.velocity += -100f * ((distance - hoverHight) / hoverHight) * Time.deltaTime * (transform.position - groundPoint).normalized;
            else
                isFlying = true;
            Debug.DrawLine(hoverPoint.position, groundPoint, Color.yellow);
            Debug.DrawRay(groundPoint, hoverPoint.forward * (raycastLength - distance), Color.black);
        }        
    }

    private void JumpCheck() { 
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        { 
            isFlying = true;
            carRigidbody.velocity += transform.up * 10f;
            airTimer = 0f;
            airBlocksSurfacecheck = true;
            lastGroundPoint = Vector3.zero;
        }
    }

    private void FlyCheck() {
        if (airBlocksSurfacecheck)
        {
            airTimer += Time.deltaTime;
            if (airTimer >= airTime)
                airBlocksSurfacecheck = false;
        }

    }

}
