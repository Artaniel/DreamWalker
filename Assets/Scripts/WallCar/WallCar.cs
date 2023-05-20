using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WallCar : MonoBehaviour
{
    public Transform[] groundCheckPoints;
    public Transform[] forwardRaycastSet;

    private bool isOnSurface = false;
    private float speed = 0f;
    private float strafeSpeed = 0f;
    public float maxSpeed = 5f;
    public float acceleration = 2f;
    public float angularSpeed = 90f;
    public float slowDownFactor = 0.9f;
    public Transform cameraHolder;
    private Rigidbody carRigidbody;
    private Vector3 normalSumm;
    public float criticalAngle = 20f;

    public Transform hoverPoint;
    public float hoverHight = 0.5f;
    private Vector3 lastGroundPoint = Vector3.zero;
    public float maxDragDistance = 5f;

    private bool isFlying = false;
    private float airTimer = 0f;
    private float airTime = 0.25f;
    private bool isJustJumped = false;

    private float jumpPower = 0;
    private float maxJumpPower = 200f;
    private float jumpPowerAccumulationSpeed = 30f;
    private bool isRisingJumpPower = false;
    private CameraMovement cameraRotation;


    private void Awake()
    {
        carRigidbody = GetComponent<Rigidbody>();
        lastGroundPoint = transform.position - Vector3.up * 5;
        cameraRotation = GetComponent<CameraMovement>();
    }

    private void FixedUpdate()
    {
        FlyCheck();
        if (!isJustJumped)
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
        isOnSurface = false;
        List<Transform> raycasts = new List<Transform>(groundCheckPoints);
        if (Keyboard.current.wKey.isPressed)
            foreach (Transform raycast in forwardRaycastSet)
                raycasts.Add(raycast);

        foreach (Transform raycast in raycasts)
        {
            Vector3 normal;
            if (IsHittingSurface(Physics.RaycastAll(raycast.position, raycast.forward, 1f), out normal))
            {
                isOnSurface = true;
                normalSumm += normal;
                Debug.DrawRay(raycast.position, raycast.forward, Color.green);
            }
            else
            {
                Debug.DrawRay(raycast.position, raycast.forward, Color.red);
            }
        }                
    }

    private bool IsHittingSurface(RaycastHit[] hits, out Vector3 normal) {
        foreach (RaycastHit hit in hits)
            if (hit.collider.tag != "Player")
            {
                normal = hit.normal;
                return true;
            }
        normal = Vector3.zero;
        return false;
    }

    private void SpherecastCheck() {  //now not used
        if (IsHittingSurface(Physics.SphereCastAll(transform.position + transform.up * 2, 2f, -transform.up, 4f), out Vector3 normal))
            normalSumm += normal;
    }

    private void Move() {
        if (Keyboard.current.wKey.isPressed)
            speed = Mathf.Clamp(speed + acceleration * Time.deltaTime, -maxSpeed, maxSpeed);
        else if (Keyboard.current.sKey.isPressed)
            speed = Mathf.Clamp(speed - acceleration * Time.deltaTime, -maxSpeed, maxSpeed);
        else
            speed = slowDownFactor * speed; //slow down from forward and back

        if (Keyboard.current.dKey.isPressed)
            strafeSpeed = Mathf.Clamp(strafeSpeed + acceleration * Time.deltaTime, -maxSpeed, maxSpeed);
        else if (Keyboard.current.aKey.isPressed)
            strafeSpeed = Mathf.Clamp(strafeSpeed - acceleration * Time.deltaTime, -maxSpeed, maxSpeed);
        else
            strafeSpeed = slowDownFactor * strafeSpeed;

        carRigidbody.velocity = transform.forward * speed + transform.right * strafeSpeed;
    }

    private void Fall() {
        speed = Vector3.Dot(carRigidbody.velocity, transform.forward);
        strafeSpeed = Vector3.Dot(carRigidbody.velocity, transform.right);
        carRigidbody.velocity += Vector3.down * Time.deltaTime * 9.8f;
    }

    private void SurfaceRotate() {
        normalSumm = normalSumm.normalized;
        Debug.DrawRay(transform.position, normalSumm);
        Vector3 currentUpDirection = transform.up;
        Debug.DrawRay(transform.position, currentUpDirection, Color.black);
        float angle = Vector3.Angle(currentUpDirection, normalSumm);
        if (angle<criticalAngle)
            angle = 0.01f * angle;
        else
            angle = Mathf.Min(angle, angularSpeed * Time.deltaTime);
        Vector3 axis = Vector3.Cross(currentUpDirection, normalSumm);
        Debug.DrawRay(transform.position, axis, Color.magenta);
        transform.Rotate(axis, angle, Space.World);
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

    private void FlyCheck()
    {
        if (isJustJumped)
        {
            airTimer += Time.deltaTime;
            if (airTimer >= airTime)
                isJustJumped = false;
        }
    }

    private void JumpCheck() {
        if (!isRisingJumpPower)
        {
            if (Keyboard.current.spaceKey.isPressed)
            {
                isRisingJumpPower = true;
                jumpPower = 0;
            }
        }
        else {
            jumpPower += jumpPowerAccumulationSpeed * Time.deltaTime;
            if (jumpPower >= maxJumpPower)
                jumpPower = maxJumpPower;

            if (!Keyboard.current.spaceKey.isPressed)
            {
                isRisingJumpPower = false;
                Jump();
            }
        }
    }

    private void Jump() {
        isFlying = true;
        if (cameraRotation.freeMode)
            carRigidbody.velocity += cameraRotation.cameraTargetTransform.forward * jumpPower;
        else
            carRigidbody.velocity += transform.up * jumpPower;
        airTimer = 0f;
        isJustJumped = true;
        lastGroundPoint = Vector3.zero;
    }
}
