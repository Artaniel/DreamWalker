using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public CharacterController characterController;
    public Transform playerTransform;
    public Transform cameraTransform;
    public Transform playerFootTransform;
    public Rigidbody playerRigidbody;
    public Transform cameraDefaultPivot;
    public bool movementEnabled = true;
    private float verticalRotation = 0f;
    private Vector2 moveInput;
    private Vector3 velocity;
    private float inMoveTime = 0f;

    public float sensitivityX = 0.1f;
    public float sensitivityY = 0.1f;
    public float speed = 10f;
    public float terminalFallSpeed = 10f;
    public float gravityAcceleration = 10f;
    public float JumpSppedAmp = 1f;
    public float airAcceleration = 0.01f;

    private void Update()
    {
        if (movementEnabled)
        {
            playerTransform.Rotate(Vector3.up, Mouse.current.delta.value.x * sensitivityX * Time.deltaTime);
            verticalRotation = Mathf.Clamp(verticalRotation - Mouse.current.delta.value.y * sensitivityY * Time.deltaTime, -90f, 90f);
            cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

            moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (GroundCheck())
            {
                GroundMove();
                if (Keyboard.current.spaceKey.wasPressedThisFrame)
                    Jump();
                if (moveInput != Vector2.zero)
                    inMoveTime += Time.deltaTime;
                else inMoveTime = 0;
            }
            else
            {
                inMoveTime = 0;
                GrivityMove();
            }
            CamraShift();
        }
    }

    private void GroundMove() {
        velocity = Time.deltaTime * speed * (moveInput.y * playerTransform.forward + moveInput.x * playerTransform.right) + velocity.y* Vector3.up;
        characterController.Move(velocity);
    }

    private void CamraShift()
    {
        float stepTime = 0.5f;
        float cameraShakeAmp = 0.5f;
        float phase = inMoveTime / stepTime;
        phase -= (int)phase;
        Vector3 cameraTargetPosition = cameraDefaultPivot.position + Vector3.up * cameraShakeAmp * (phase-0.6f) ;
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, cameraTargetPosition, 0.05f);
    }

    private bool GroundCheck() {
        int layer = LayerMask.GetMask("Default");
        return Physics.CheckSphere(playerFootTransform.position, 0.5f, layer);
    }

    private void GrivityMove() {
        velocity += Time.deltaTime * airAcceleration * (moveInput.y * playerTransform.forward + moveInput.x * playerTransform.right);
        velocity += Vector3.down * gravityAcceleration * Time.deltaTime;
        if (velocity.y < -terminalFallSpeed)
            velocity = new Vector3(velocity.x, terminalFallSpeed, velocity.z);
        characterController.Move(velocity);
    }

    private void Jump() {
        velocity = new Vector3(velocity.x, JumpSppedAmp, velocity.z);
    }
}
