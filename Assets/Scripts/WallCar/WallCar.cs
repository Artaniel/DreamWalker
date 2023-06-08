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
    public float slowDownFactor = 0.9f;
    public Transform cameraHolder;
    [HideInInspector] public Rigidbody carRigidbody;
    private Vector3 normalSumm;

    public Transform hoverPoint;
    public float hoverHight = 0.5f;
    private Vector3 lastGroundPoint = Vector3.zero;
    public float maxDragDistance = 5f;
    private Vector3 legsPosSumm;
    private int legsInContact;

    private bool isFlying = false;
    private float airTimer = 0f;
    private float airTime = 0.25f;
    [HideInInspector] public bool airBlocksSurfacecheck = false;

    private float jumpPower = 0;
    public float maxJumpPower = 200f;
    public float jumpPowerAccumulationSpeed = 30f;
    private bool isRisingJumpPower = false;
    private CameraMovement cameraRotation;

    public SpiderLeg[] legs;
    [HideInInspector] public int legSyncPhase = 0;
    private float legSyncTimer = 0f;
    public float legSyncPeriod = 0.1f;

    [HideInInspector] public Vector2 moveInput = Vector2.zero;    

    private void Awake()
    {
        carRigidbody = GetComponent<Rigidbody>();
        lastGroundPoint = transform.position - Vector3.up * 5;
        cameraRotation = GetComponent<CameraMovement>();
    }

    private void FixedUpdate()
    {
        InputUpdate();
        FlyCheck();
        if (!airBlocksSurfacecheck)
        {
            //SurfaceCheck();
            LegSurfaceCheck();
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
        LegSyncUpdate();
    }

    private void SurfaceCheck()
    {
        bool touchPointFound = false;
        normalSumm = Vector3.zero;

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
                normalSumm += foundNormal;
                Debug.DrawRay(groundCheckPoint.position, groundCheckPoint.forward, Color.green);
            }
            else
            {
                Debug.DrawRay(groundCheckPoint.position, groundCheckPoint.forward, Color.red);
            }
        }

        isOnSurface = touchPointFound;
    }

    private void Move()
    {
        if (moveInput.y != 0)
            speed = Mathf.Clamp(speed + moveInput.y * acceleration * Time.deltaTime, -maxSpeed, maxSpeed);
        else
            speed = slowDownFactor * speed; //slow down from forward and back
        if (moveInput.x != 0)
            strafeSpeed = Mathf.Clamp(strafeSpeed + moveInput.x * acceleration * Time.deltaTime, -maxSpeed, maxSpeed);
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
        angle = Mathf.Min(angle, angularSpeed * Time.deltaTime);
        Vector3 axis = Vector3.Cross(currentUpDirection, normalSumm);
        Debug.DrawRay(transform.position, axis, Color.magenta);
        transform.Rotate(axis, angle, Space.World);
    }

    private void Hover()
    {
        float raycastLength = 5f;
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

        if (legsInContact > 0)
        {
            groundPoint = legsPosSumm / legsInContact;
            float distance = Vector3.Distance(groundPoint, transform.position);
            //Debug.Log(distance);
            groundPoint = transform.position - transform.up * distance;
            if (distance < 0.2f)
                transform.position += transform.up * 0.2f;
            if (distance <= maxDragDistance)
                carRigidbody.velocity += -100f * ((distance - hoverHight) / hoverHight) * Time.deltaTime * (transform.position - groundPoint).normalized;
            else
                isFlying = true;
            Debug.DrawLine(hoverPoint.position, groundPoint, Color.yellow);
            //Debug.DrawRay(groundPoint, hoverPoint.forward * (raycastLength - distance), Color.black);
        }        
    }

    private void FlyCheck()
    {
        if (airBlocksSurfacecheck)
        {
            airTimer += Time.deltaTime;
            if (airTimer >= airTime)
                airBlocksSurfacecheck = false;
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
        airBlocksSurfacecheck = true;
        lastGroundPoint = Vector3.zero;

        foreach (SpiderLeg leg in legs)
            leg.Disconnect();
    }

    private void LegSurfaceCheck()
    {
        legsPosSumm = Vector3.zero;
        legsInContact = 0;
        normalSumm = Vector3.zero;
        foreach (SpiderLeg leg in legs)
        {
            if (leg.torchingGround)
            {
                normalSumm += leg.currentNormal;
                legsInContact++;
                legsPosSumm += leg.legTransform.position;
            }
        }        
        isOnSurface = (normalSumm != Vector3.zero);
    }

    public void LegSyncUpdate() {
        legSyncTimer += Time.deltaTime;
        if (legSyncTimer > legSyncPeriod)
        {
            legSyncTimer -= legSyncPeriod;
            legSyncPhase  = (legSyncPhase+1)% 2;
            foreach (SpiderLeg leg in legs) 
                leg.SyncStep();            
        }
    }

    private void InputUpdate() {
        moveInput = Vector2.zero;
        if (Keyboard.current.wKey.isPressed)
            moveInput += Vector2.up;
        if (Keyboard.current.sKey.isPressed)
            moveInput += Vector2.down;
        if (Keyboard.current.aKey.isPressed)
            moveInput += Vector2.left;
        if (Keyboard.current.dKey.isPressed)
            moveInput += Vector2.right;
    }

    public Vector3 GetInputForvardPosition() {
        //return (moveInput.y * transform.forward + moveInput.x * transform.right) * maxSpeed / 4f;
        return Vector3.ProjectOnPlane(carRigidbody.velocity * 0.2f, transform.up);
    }
}
