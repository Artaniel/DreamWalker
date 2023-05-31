using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderLeg : MonoBehaviour
{
    public WallCar wallCar;
    public Transform legTransform;
    public Transform spiderBody;
    public Transform[] surfaceFindingPathNeutral;
    [HideInInspector] public Vector3 currentNormal;
    public float minStepLenght = 1f;
    private float maxStepLenght = 5f;
    public bool torchingGround = false;

    private const float defaultLegModelLength = 0.8f;

    private Quaternion sholderDefaultRotation;
    private Vector3 legDefaultScale;

    public int syncIndex;
    private Vector3 visualLegPosition;

    public float aboveKneeLength = 2.5f;
    public float belowKneeLength = 2.5f;
    public Transform sholder;
    public Transform legModel;
    public Transform kneeTransform;
    public Transform legLowerModel;

    private void Awake()
    {
        maxStepLenght = aboveKneeLength + belowKneeLength;
        sholderDefaultRotation = sholder.localRotation;
        legDefaultScale = legModel.localScale;
    }

    void FixedUpdate()
    {
        maxStepLenght = aboveKneeLength + belowKneeLength; // for debug only
        if (!wallCar.airBlocksSurfacecheck)
        {
            RaycastHit hit;
            if (CheckPath(surfaceFindingPathNeutral, wallCar.GetInputForvardPosition(), out hit))
            {
                if (Vector3.Distance(hit.point, legTransform.position) > minStepLenght || !torchingGround)
                {
                    currentNormal = hit.normal;
                    legTransform.position = hit.point;
                    torchingGround = true;
                    //rotation? vfx? sfx?
                }
            }
            if (Vector3.Distance(wallCar.transform.position, legTransform.position) > maxStepLenght)
            {
                Disconnect();
            }
        }
        if (torchingGround)
            UpdateModelTransform();
    }

    public void Disconnect()
    {
        currentNormal = Vector3.zero;
        torchingGround = false;
        sholder.localRotation = sholderDefaultRotation;
        legModel.localScale = legDefaultScale;
    }

    private bool CheckPath(Transform[] path, Vector3 addedVector, out RaycastHit foundHit) {
        RaycastHit[] hits;      
        for (int i = 0; i < path.Length-1; i++) {
            hits = Physics.RaycastAll(path[i].position + addedVector, path[i + 1].position - path[i].position, 
                Vector3.Distance(path[i].position, path[i + 1].position));
            if (GetHitFromArray(hits, out foundHit))
            {
                Debug.DrawLine(path[i].position + addedVector, path[i + 1].position + addedVector, Color.green);
                Debug.DrawRay(legTransform.position, currentNormal, Color.cyan);
                return true;
            }
            else
            {
                Debug.DrawLine(path[i].position + addedVector, path[i + 1].position + addedVector, Color.red);
            }
        }
        foundHit = new RaycastHit(); //empty
        return false; // if not found
    }

    private bool GetHitFromArray(RaycastHit[] hits, out RaycastHit foundHit) {
        foreach (RaycastHit hit in hits)
            if (hit.collider.tag != "Player" && !hit.collider.isTrigger)
            {
                foundHit = hit;
                return true;
            }
        foundHit = new RaycastHit();
        return false;
    }

    private void UpdateModelTransform() {
        UpdateKnee();
        sholder.LookAt(kneeTransform.position);
        sholder.Rotate(sholder.forward, -90f);
        kneeTransform.LookAt(visualLegPosition);
        //kneeTransform.Rotate(transform.forward, -90f);
        legModel.localScale = new Vector3(1f, Vector3.Distance(sholder.position, kneeTransform.position) * defaultLegModelLength, 1f);
        legLowerModel.localScale = new Vector3(1f, Vector3.Distance(visualLegPosition, kneeTransform.position) * defaultLegModelLength, 1f);
    }
    public void UpdateKnee()
    {
        Vector3 legSurfaceNormal = Vector3.Cross(wallCar.transform.up, visualLegPosition - sholder.position).normalized;
        float sholderToFootDist = (visualLegPosition - sholder.position).magnitude;
        float kneeRisingAngleCos = (sholderToFootDist * sholderToFootDist + aboveKneeLength * aboveKneeLength - belowKneeLength * belowKneeLength) /
            (2 * aboveKneeLength * sholderToFootDist);
        float kneeRisingAngleSin = Mathf.Sqrt(1 - (kneeRisingAngleCos * kneeRisingAngleCos));
        Quaternion rotation = new Quaternion(legSurfaceNormal.x * kneeRisingAngleSin, legSurfaceNormal.y * kneeRisingAngleSin,
            legSurfaceNormal.z * kneeRisingAngleSin, kneeRisingAngleCos);
        Debug.Log(kneeRisingAngleCos);
        Vector3 kneePosition = rotation * (visualLegPosition - sholder.position).normalized;
        kneePosition *= belowKneeLength;
        kneePosition += sholder.position;
        kneeTransform.position = kneePosition;
    }

    public void SyncStep() {
        if (wallCar.legSyncPhase == syncIndex)
            visualLegPosition = legTransform.position;
    }

}
