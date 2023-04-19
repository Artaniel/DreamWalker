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

    public float speed = 10f;

    public Transform cameraDefaultPivot;
    private float inMoveTime = 0f;

    private void Update()
    {
        if (movementEnabled)
        {
            playerTransform.Rotate(Vector3.up, Mouse.current.delta.value.x * sensitivityX * Time.deltaTime);
            verticalRotation = Mathf.Clamp(verticalRotation - Mouse.current.delta.value.y * sensitivityY * Time.deltaTime, -90f, 90f);
            cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

            Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            Vector3 velocity = (moveInput.y * playerTransform.forward + moveInput.x * playerTransform.right) * Time.deltaTime * speed;
            characterController.Move(velocity);


            if (moveInput != Vector2.zero)
                inMoveTime += Time.deltaTime;
            else inMoveTime = 0;
            CamraShift();
        }
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
}
