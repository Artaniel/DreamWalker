using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public CharacterController characterController;
    public Transform playerTransform;
    public Transform cameraTransform;
    public Rigidbody playerRigidbody;
    public bool movementEnabled = true;
    public float sensitivityX = 0.1f;
    public float sensitivityY = 0.1f;
    private float verticalRotation = 0f;
    private Vector2 moveInput;
    private Vector3 velocity;

    public float speed = 10f;
    public float terminalFallSpeed = 10f;

    public Transform cameraDefaultPivot;
    private float inMoveTime = 0f;

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
        velocity = Time.deltaTime * speed * (moveInput.y * playerTransform.forward + moveInput.x * playerTransform.right);
        characterController.Move(velocity);
    }

    private void CamraShift()
    {
        float stepTime = 0.5f;
        float cameraShakeAmp = 0.5f;
        float phase = inMoveTime / stepTime;
        phase -= (int)phase;
        Vector3 cameraTargetPosition = cameraDefaultPivot.position + Vector3.up * cameraShakeAmp * phase;
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, cameraTargetPosition, 0.05f);
    }

    private bool GroundCheck() {
        return Physics.CheckSphere(playerTransform.position + Vector3.down * 1.5f, 0.5f);        
    }

    private void GrivityMove() {
        velocity += Vector3.down * 10f * Time.deltaTime;
        if (velocity.y < -terminalFallSpeed)
            velocity = new Vector3(velocity.x, terminalFallSpeed, velocity.z);
        characterController.Move(velocity);
    }
}
