using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WallCar : MonoBehaviour
{
    public Transform[] groundCheckPoints;

    [HideInInspector] public bool isOnSurface = false;
    private float speed = 0f;
    private float strafeSpeed = 0f;
    public float maxSpeed = 5f;
    public float maxSpeedBoosted = 20f;
    public float acceleration = 2f;
    public float angularSpeed = 90f;
    public float airAngularSpeed = 90f;
    public float slowDownFactor = 0.9f;
    public Transform cameraHolder;
    [HideInInspector] public Rigidbody carRigidbody;
    private Vector3 normalSumm;

    public Transform hoverPoint;
    public float hoverHight = 0.5f;
    private Vector3 lastGroundPoint = Vector3.zero;
    public float maxDragDistance = 3f;
    private Vector3 legsPosSumm;
    private int legsInContact;

    private float airTimer = 0f;
    public float airTime = 0.6f;
    [HideInInspector] public bool airBlocksSurfacecheck = false;
    public float airMovementAcceleration = 0.1f;

    [HideInInspector] public float jumpPower = 0;
    public float maxJumpPower = 200f;
    public float jumpPowerAccumulationSpeed = 30f;
    public float fastJumpPower = 10f;
    private bool isRisingJumpPower = false;
    private CameraMovement cameraRotation;

    public SpiderLeg[] legs;
    [HideInInspector] public int legSyncPhase = 0;
    private float legSyncTimer = 0f;
    public float legSyncPeriod = 0.1f;
    public float legSyncPeriodBoosted = 0.1f;

    [HideInInspector] public Vector2 moveInput = Vector2.zero;
    [HideInInspector] public bool boostIsPressed = false;
    [HideInInspector] public bool focusIsPressed = false;
    [HideInInspector] public bool jumpIsPressed = false;

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
            LegSurfaceCheck();
            if (isOnSurface)
            {
                Move();
                JumpCheck();
                Hover();
            }
            else
            {
                AirMovement();
                DragBackCheck();
            }
        }
        SurfaceRotate();
        LegSyncUpdate();
    }

    private void Move()
    {
        float currentMaxSpeed = boostIsPressed ? maxSpeedBoosted : maxSpeed;
        if (moveInput.y != 0)
            speed = Mathf.Clamp(speed + moveInput.y * acceleration * Time.deltaTime, -currentMaxSpeed, currentMaxSpeed);
        else
            speed = slowDownFactor * speed; //slow down from forward and back
        if (moveInput.x != 0)
            strafeSpeed = Mathf.Clamp(strafeSpeed + moveInput.x * acceleration * Time.deltaTime, -currentMaxSpeed, currentMaxSpeed);
        else
            strafeSpeed = slowDownFactor * strafeSpeed;

        carRigidbody.velocity = transform.forward * speed + transform.right * strafeSpeed;
    }

    private void SurfaceRotate() {
        normalSumm = normalSumm.normalized;
        Debug.DrawRay(transform.position, normalSumm);
        Vector3 currentUpDirection = transform.up;
        Debug.DrawRay(transform.position, currentUpDirection, Color.black);
        float angle = Vector3.Angle(currentUpDirection, normalSumm);
        angle = Mathf.Min(angle, (isOnSurface ? angularSpeed : airAngularSpeed) * Time.fixedDeltaTime);
        Vector3 axis = Vector3.Cross(currentUpDirection, normalSumm);
        Debug.DrawRay(transform.position, axis, Color.magenta);
        transform.Rotate(axis, angle, Space.World);
    }

    private void Hover()
    {
        Vector3 groundPoint;
        if (legsInContact == 0)
            groundPoint = lastGroundPoint;
        else
            groundPoint = legsPosSumm / legsInContact;
        float distance = Vector3.Distance(groundPoint, transform.position);

        if (distance <= maxDragDistance)
            transform.position += (hoverHight - Vector3.Dot(transform.position - groundPoint, transform.up)) * 0.1f * transform.up;
        Debug.DrawLine(hoverPoint.position, groundPoint, Color.yellow);
        lastGroundPoint = groundPoint;
    }

    private void DragBackCheck()
    {
        if (Vector3.Distance(transform.position, lastGroundPoint) <= maxDragDistance && !boostIsPressed)
        {
            carRigidbody.velocity += (transform.position - lastGroundPoint) * -1f;
            Debug.DrawLine(hoverPoint.position, lastGroundPoint, Color.yellow);
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
        { if (isOnSurface)
            {
                if (focusIsPressed)
                {
                    isRisingJumpPower = true;
                    jumpPower = 0;
                }
                else {
                    if (jumpIsPressed)
                    {
                        jumpPower = fastJumpPower;
                        Jump();
                    }
                }
            }
        }
        else {
            jumpPower += jumpPowerAccumulationSpeed * Time.deltaTime;
            if (jumpPower >= maxJumpPower)
                jumpPower = maxJumpPower;

            if (jumpIsPressed)
            {
                isRisingJumpPower = false;
                Jump();
                jumpPower = 0;
            }
            else if (!focusIsPressed) { //RMB released without jump
                isRisingJumpPower = false;
                jumpPower = 0;
            }
        }
    }

    private void Jump() {
        if (cameraRotation.freeMode)
            carRigidbody.velocity += cameraRotation.cameraTargetTransform.forward * jumpPower;
        else
            carRigidbody.velocity += transform.up * jumpPower;
        airTimer = 0f;
        airBlocksSurfacecheck = true;
        lastGroundPoint = Vector3.zero;

        foreach (SpiderLeg leg in legs)
            leg.Disconnect();
        lastGroundPoint = Vector3.zero;
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
        float currentLegSyncPeriod = boostIsPressed ? legSyncPeriodBoosted : legSyncPeriod;
        legSyncTimer += Time.deltaTime;
        if (legSyncTimer > currentLegSyncPeriod)
        {
            legSyncTimer -= currentLegSyncPeriod;
            legSyncPhase = (legSyncPhase + 1) % 2;
            foreach (SpiderLeg leg in legs)
                leg.SyncStep();
        }
    }

    private void InputUpdate() {
        // dont forget some controls in CameraMovement script
        moveInput = Vector2.zero;
        if (!MouseLock.settingsIsOpen)
        {
            if (Keyboard.current.wKey.isPressed)
                moveInput += Vector2.up;
            if (Keyboard.current.sKey.isPressed)
                moveInput += Vector2.down;
            if (Keyboard.current.aKey.isPressed)
                moveInput += Vector2.left;
            if (Keyboard.current.dKey.isPressed)
                moveInput += Vector2.right;
            if (moveInput != Vector2.zero)
                moveInput.Normalize();
            boostIsPressed = Keyboard.current.shiftKey.isPressed;
            focusIsPressed = Mouse.current.rightButton.isPressed;
            jumpIsPressed = Mouse.current.leftButton.isPressed || Keyboard.current.spaceKey.isPressed;
        }
    }

    public Vector3 GetInputForvardPosition() {
        if (boostIsPressed)
            return Vector3.ProjectOnPlane(carRigidbody.velocity * 0.09f, transform.up);
        else
            return Vector3.ProjectOnPlane(carRigidbody.velocity * 0.18f, transform.up);
    }

    private void AirMovement()
    {
        speed = Vector3.Dot(carRigidbody.velocity, transform.forward);
        speed += moveInput.y * airMovementAcceleration * Time.deltaTime;
        strafeSpeed = Vector3.Dot(carRigidbody.velocity, transform.right);
        strafeSpeed += moveInput.x * airMovementAcceleration * Time.deltaTime;

        carRigidbody.velocity = transform.forward * speed + transform.right * strafeSpeed + Vector3.Dot(carRigidbody.velocity, transform.up) * transform.up;
        carRigidbody.velocity += Vector3.down * Time.deltaTime * 9.8f;
        normalSumm = Vector3.up;
    }
}
