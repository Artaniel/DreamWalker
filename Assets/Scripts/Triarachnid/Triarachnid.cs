using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triarachnid : MonoBehaviour
{
    public Body body;
    public Foot[] foots;
    public float legLeangth = 50f;

    private void Awake()
    {
        body.triarachnid = this;
    }

}
