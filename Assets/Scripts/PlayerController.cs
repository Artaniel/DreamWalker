using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
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

    private void Update()
    {        
        playerTransform.Rotate(Vector3.up, Mouse.current.delta.value.x * sensitivityX);
        verticalRotation = Mathf.Clamp(verticalRotation - Mouse.current.delta.value.y * sensitivityY, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (movementEnabled)
        {
            if (moveInput.magnitude < lastmoveInput.magnitude && Vector3.Angle(moveInput, lastmoveInput) < 1f)
            {
                lastmoveInput = moveInput;
                moveInput = Vector2.zero;
            }else
                lastmoveInput = moveInput;
            if (moveInput != Vector2.zero)
            {
                Vector3 velocity = playerRigidbody.velocity;
                velocity += (moveInput.y * playerTransform.forward + moveInput.x * playerTransform.right) * Time.deltaTime * acceleration;
                if (velocity.magnitude >= maxGroundSpeed)
                    velocity = velocity.normalized * maxGroundSpeed;
                playerRigidbody.velocity = velocity;
            }
            else {
                playerRigidbody.velocity = Vector3.zero;
            }
        }
    }
}
