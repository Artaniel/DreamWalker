using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Foot : MonoBehaviour
{
    private enum LegStatus { free, connected, controlled }
    private LegStatus status = LegStatus.free;
    [HideInInspector] public Rigidbody myRigidbody;
    private Vector3 connectedPosition;
    public Body body;

    void Start()
    {
        myRigidbody = gameObject.GetComponent<Rigidbody>();
    }

    private void OnMouseDrag()
    {
        if (status == LegStatus.connected || status == LegStatus.free)
        {
            status = LegStatus.controlled;
        }
    }

    void Update()
    {
        if (status == LegStatus.free)
        {
         //   FreeUpdate();
        }
        else if (status == LegStatus.connected)
        {
            ConnectedUpdate();
        }
        else if (status == LegStatus.controlled)
        {
            if (Input.GetMouseButtonUp(0))
            {
                status = LegStatus.free;
            }
            else
            {
                ControledUpdate();
            }
        }
    }

    void ControledUpdate()
    {        
        Vector3 targetPosition = GetTargetPosition();
        Vector3 manualForce = myRigidbody.mass * (targetPosition - transform.position);

        Vector3 brakingForce = myRigidbody.velocity * (-200f) * (1 / (targetPosition - transform.position).magnitude);
        Vector3 supportForce = myRigidbody.mass * Physics.gravity * -0.3f;
        Vector3 reverseForce;
        if (Vector3.Angle(manualForce, myRigidbody.velocity) > 90)
        {
            reverseForce = myRigidbody.velocity * -1f;
            Debug.DrawRay(transform.position, reverseForce, Color.red);
        }
        else reverseForce = Vector3.zero;
        body.SendForce(-1f * (manualForce + brakingForce + reverseForce + supportForce));
        myRigidbody.AddForce(manualForce + brakingForce + reverseForce + supportForce);
        Debug.DrawRay(transform.position, manualForce * 0.15f);
        Debug.DrawRay(transform.position, brakingForce * 0.15f, Color.green);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (status == LegStatus.free)
        {
            if (collision.gameObject.tag != "Player")
                status = LegStatus.connected;
            connectedPosition = transform.position;
        }
    }

    void ConnectedUpdate()
    {
        transform.position = connectedPosition;
    }

    private Vector3 GetTargetPosition() { // потом добавить фильтрацию hit-ов, чтобы отсеять своего персонажа
        Vector3 result;
        RaycastHit hit;
        Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.value), out hit);
        result = hit.point;
        if (Vector3.Distance(transform.position, result) > body.triarachnid.legLeangth)
            result = transform.position + (result - transform.position).normalized * body.triarachnid.legLeangth;
        return result;
        
    }

    public bool isConnected() { return status == LegStatus.connected; }
}
