using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Terminal : MonoBehaviour
{
    private UIManager ui;
    private GameObject player;
    private Transform cameraTransform;
    private bool playerIsInside = false;
    private bool isFocused = false;

    public Transform cameraPivot;
    public GameObject terminalContentPanel;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        ui = GameObject.FindWithTag("Canvas").GetComponent<UIManager>();
        player = GameObject.FindWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            playerIsInside = true;
            if (Vector3.Angle(other.transform.forward, transform.position - other.transform.position) < 90f)
            {
                ui.ShowPressE(this);
            }
        }
    }   

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            playerIsInside = false;
        }
    }

    public bool IsLookingAt() {
        return Vector3.Angle(player.transform.forward, transform.position - player.transform.position) < 90f && playerIsInside;
    }

    private void Update()
    {
        if (playerIsInside && Keyboard.current.eKey.IsPressed()&& !isFocused)
            FocusOnTerminal();
    }

    private void FocusOnTerminal() {
        ui.HidePressE();
        isFocused = true;
        player.GetComponent<PlayerController>().movementEnabled = false;
        StartCoroutine(CameraTransition());
    }

    IEnumerator CameraTransition() {
        Transform start = player.GetComponent<PlayerController>().cameraDefaultPivot;
        Quaternion startRotation = cameraTransform.rotation;
        Transform end = cameraPivot;
        float timer = 0f;
        float transtiotnTime = 1f;
        while (timer < transtiotnTime) {
            timer += Time.deltaTime;
            cameraTransform.position = Vector3.Lerp(start.position, end.position, timer / transtiotnTime);
            cameraTransform.rotation = Quaternion.Lerp(startRotation, end.rotation, timer / transtiotnTime);
            yield return null;
        }
        cameraTransform.position = end.position;
        cameraTransform.rotation = end.rotation;

        terminalContentPanel.SetActive(true);
    }

    public void CloseUI() { 
    
    }

}
