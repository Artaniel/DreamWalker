using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BreathInput : MonoBehaviour
{
    //doing быстрое дыхание (определить время цикла, от вдоха, до следующего вдоха)
    // bug не полный цикл тоже работает, надо включить и детектирование выдоха
    //todo медленное дыхание

    public enum InputMode { scroll, RBMonly, RBMnLBM, mouseY }
    public InputMode inputMode = InputMode.scroll;
    public enum BreathMode { inhale, exhale, lowHold, highHold }
    public BreathMode breathMode;

    private float lastInhaleStartTimestump;
    public float fastBreathTimeTreshhold = 1f;
    private int fastBreathCounter = 0;
    private PlayerController player;
    private UIManager ui;
    private float stayInSameModeTimer = 0f;
    public float stayInSameModeTreshhold = 2f;


    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        ui = GameObject.FindWithTag("Canvas").GetComponent<UIManager>();
    }

    private void Update()
    {
        switch (inputMode) {
            case InputMode.scroll: ScrollInputUpdate(); break;
            case InputMode.RBMonly: RMBInput(); break;
            case InputMode.RBMnLBM: RBMnLBMInput(); break;
            case InputMode.mouseY: RMBInput(); break;
        }
    }

    private void DefineBreathMode(float breathSpeed) {
        BreathMode lastBreathMode = breathMode;
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

        if (breathMode != lastBreathMode)
        {
            stayInSameModeTimer = 0;
            Debug.Log(breathMode.ToString());
            if (breathMode == BreathMode.inhale && (lastBreathMode == BreathMode.lowHold || lastBreathMode == BreathMode.exhale)) // if changed to inhale
            {
                if (Time.time - lastInhaleStartTimestump < fastBreathTimeTreshhold)
                    fastBreathCounter++;
                else
                    fastBreathCounter = fastBreathCounter / 2;
                FastBreathRefresh();
                lastInhaleStartTimestump = Time.time;
            }
        }
        else {
            stayInSameModeTimer += Time.deltaTime;
            if (stayInSameModeTimer > stayInSameModeTreshhold) {
                fastBreathCounter = fastBreathCounter / 2;
                FastBreathRefresh();
            }
        }
    }

    private void FastBreathRefresh() {
        player.breathSpeedModifier = 1 + (fastBreathCounter * 0.2f);
        ui.RefreshSpeedModifier(player.breathSpeedModifier);
    }

    #region InputModes
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

    private void RMBInput() {
        float breathSpeed = 0;
        if (Mouse.current.rightButton.value > 0)
            breathSpeed = 1;
        else breathSpeed = -1;
        DefineBreathMode(breathSpeed);
    }

    private void RBMnLBMInput()
    {
        float breathSpeed = 0;
        if (Mouse.current.rightButton.value > 0)
            breathSpeed = 1;
        else if (Mouse.current.rightButton.value < 0)
            breathSpeed = -1;
        else
            breathSpeed = 0;
        DefineBreathMode(breathSpeed);
    }

    private void MouseYInput() { 
    
    }
    #endregion
}
