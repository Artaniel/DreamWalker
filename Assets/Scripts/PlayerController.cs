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
    private float verticalRotetion = 0f;

    public InputActionAsset actions;

    private void Update()
    {        
        playerTransform.Rotate(Vector3.up, Mouse.current.delta.value.x * sensitivityX);
        verticalRotetion = Mathf.Clamp(verticalRotetion - Mouse.current.delta.value.y * sensitivityY, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotetion, 0, 0);

        if (movementEnabled)
        {
            Vector2 moveInput = actions.FindActionMap("Player").FindAction("move").ReadValue<Vector2>();
            Debug.Log(actions.FindActionMap("Player").FindAction("move"));
            playerRigidbody.AddForce(new Vector3(moveInput.y, moveInput.x, 0), ForceMode.VelocityChange);
        }
    }
}
