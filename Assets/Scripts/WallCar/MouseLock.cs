using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLock : MonoBehaviour
{
    public static bool settingsIsOpen = false;
    public Menu menu;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        settingsIsOpen = false;
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame) {
            settingsIsOpen = !settingsIsOpen;
            if (settingsIsOpen)
            {
                menu.OpenIngameSettings();
                Cursor.lockState = CursorLockMode.None;
            }
            else {
                menu.CloseSettings();
                Cursor.lockState = CursorLockMode.Locked;
            }

        }
    }

    public void SettingsClosed() {
        settingsIsOpen = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

}
