using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrativeManager : MonoBehaviour
{
    public static NarrativeManager instance;
    public enum NarrativeState {start, wrongTower, sunIsRotating, end}
    public NarrativeState state = NarrativeState.start;
    public SunManager sun;
    public bool energyIsOn = false;
    public int energyNeedlesInjected = 0;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            state = NarrativeState.start;
        }
        else
            Debug.LogWarning("Narrative manager duplicate?");
    }

    public static void TriggerActivated(NarrativeState gateState)
    {
        if (gateState == NarrativeState.wrongTower)
            WrongHighTower();
        else if (gateState == NarrativeState.end)
            EndTriggerReached();
    }

    public static void WrongHighTower() {
        if (instance.state == NarrativeState.start)
        {
            Debug.Log("Reached wrong tower");
            instance.state = NarrativeState.wrongTower;
        }
    }

    public static void Over1000mReached()
    {
        if (instance.state == NarrativeState.start || instance.state == NarrativeState.wrongTower) {
            Debug.Log("1000m reached");
            instance.sun.isRotating = true;
            instance.state = NarrativeState.sunIsRotating;
        }
    }

    public static void EndTriggerReached()
    {
        if (instance.state == NarrativeState.sunIsRotating)
        {
            Debug.Log("End.");
            instance.state = NarrativeState.end;
        }
    }

    public static void EnergyNeedleInject()
    {
        instance.energyNeedlesInjected++;
        if (instance.energyNeedlesInjected >= 3)
        {
            instance.energyIsOn = true;
        }
    }


}
