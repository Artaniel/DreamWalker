using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrativeTrigger : MonoBehaviour
{
    public NarrativeManager.NarrativeState state;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") 
            NarrativeManager.TriggerActivated(state);
    }
}
