using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public WallCar car;
    public float reactionRadius = 5f;
    private bool isActive;
    public TextMeshProUGUI pressEText;
    public UnityEvent myEvent;

    private void Awake()
    {
        if (!car)
            Debug.LogError("no link to WallCar from Interactable");
        if (!pressEText)
            Debug.LogError("no link to TextMeshProUGUI from Interactable");

    }

    void Update()
    {
        if (!isActive && Vector3.Distance(transform.position, car.transform.position) <= reactionRadius)
        {
            pressEText.enabled = true;
            isActive = true;
        }
        else if (isActive && Vector3.Distance(transform.position, car.transform.position) > reactionRadius)
        {
            pressEText.enabled = false;
            isActive = false;
        }
        if (isActive && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("Interactable E pressed");
            myEvent.Invoke();
        }
    }
}
