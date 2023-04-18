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

    public float acceleration = 10f;
    public float maxGroundSpeed = 2f;
    private Vector2 lastmoveInput = Vector2.zero;

    public Transform cameraDefaultPivot;

    private void Update()
    {
        if (movementEnabled)
        {
            playerTransform.Rotate(Vector3.up, Mouse.current.delta.value.x * sensitivityX);
            verticalRotation = Mathf.Clamp(verticalRotation - Mouse.current.delta.value.y * sensitivityY, -90f, 90f);
            cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

            Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            Vector3 velocity = (moveInput.y * playerTransform.forward + moveInput.x * playerTransform.right) * Time.deltaTime * acceleration;
            characterController.Move(velocity);
        }
    }
}
