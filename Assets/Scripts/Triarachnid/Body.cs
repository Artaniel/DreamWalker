using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    private Rigidbody myRigidbody;
    public Triarachnid triarachnid;

    void Start()
    {
        myRigidbody = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (IsAnyFootConnected())
            myRigidbody.AddForce(Vector3.up*2f*myRigidbody.mass);
        MaxDistCheck();
    }

    public void SendForce(Vector3 externalForce)
    { 
        if (!IsAnyFootConnected())
            myRigidbody.AddForce(externalForce);
        Debug.DrawRay(transform.position, externalForce * 0.15f, Color.green);
    }

    private void MaxDistCheck() {
        foreach (Foot foot in triarachnid.foots) {
            if (Vector3.Distance(transform.position, foot.transform.position) > triarachnid.legLeangth) {
                Vector3 relativeVelocity = foot.GetComponent<Rigidbody>().velocity - myRigidbody.velocity;
                Vector3 relativeVelocityRadial = Vector3.Project(relativeVelocity, foot.transform.position - transform.position);
                myRigidbody.velocity+= relativeVelocityRadial/2f;
                foot.myRigidbody.velocity += -relativeVelocityRadial/2f * (foot.myRigidbody.mass/myRigidbody.mass);
            }
        }
    }

    private bool IsAnyFootConnected() {
        bool result = false;
        foreach (Foot foot in triarachnid.foots) {
            result = result || foot.isConnected();
        }
        return result;
    }

}
