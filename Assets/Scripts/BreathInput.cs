using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BreathInput : MonoBehaviour
{
    public float lung;
    public enum InputMode { scroll, RBMonly, RBMnLBM, mouseY }
    public InputMode inputMode = InputMode.scroll;

    public enum BreathMode {inhale, exhale, lowHold, highHold};
    public BreathMode breathMode;

    private void Update()
    {
        if (inputMode == InputMode.scroll) {
            ScrollInputUpdate();
        }
    }

    private void DefineBreathMode(float breathSpeed) {
        if (breathSpeed > 0)
            breathMode = BreathMode.inhale;
        else if (breathSpeed < 0)
            breathMode = BreathMode.exhale;
        else if (breathSpeed == 0)
        {
            if (breathMode == BreathMode.inhale)
                breathMode = BreathMode.highHold;
            if (breathMode == BreathMode.exhale)
                breathMode = BreathMode.lowHold;
        }
        Debug.Log(breathMode.ToString());
    }

    #region ScrollInput
    private Dictionary<float, float> lastInputs = new Dictionary<float, float>();
    private float trackingDelay = 0.5f;

    private void ScrollInputUpdate() {
        if (Mouse.current.scroll.value.y != 0)
            lastInputs.Add(Time.time, Mouse.current.scroll.value.y);

        List<float> toRemove = new List<float>();
        float breathSpeed = 0;
        foreach (float timeStamp in lastInputs.Keys) { // calculating sum in last trackingDelay time, marks for remove every else
            if (Time.time - timeStamp > trackingDelay)
                toRemove.Add(timeStamp);
            else
                breathSpeed += lastInputs[timeStamp];
        }

        foreach (float timeStamp in toRemove) // remove marked
            lastInputs.Remove(timeStamp);

        DefineBreathMode(breathSpeed / 120f);
    }
    #endregion


}
