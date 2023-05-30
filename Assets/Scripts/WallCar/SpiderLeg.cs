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

    public Transform sholder;
    public Transform legModel;
    private const float defaultLegModelLength = 0.8f;

    private Quaternion sholderDefaultRotation;
    private Vector3 legDefaultScale;

    public int syncIndex;
    private Vector3 visualLegPosition;

    public float aboveKneeLength = 2.5f;
    public float beloveKneeLength = 2.5f;
    public Transform kneeTransform;

    private void Awake()
    {
        maxStepLenght = aboveKneeLength + beloveKneeLength;
        sholderDefaultRotation = sholder.localRotation;
        legDefaultScale = legModel.localScale;
    }

    void FixedUpdate()
    {
        maxStepLenght = aboveKneeLength + beloveKneeLength; // for debug only
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
        sholder.LookAt(visualLegPosition);
        sholder.Rotate(transform.forward, -90f);
        legModel.localScale = new Vector3(1f, Vector3.Distance(sholder.position, visualLegPosition) * defaultLegModelLength , 1f);
    }

    public void SyncStep() {
        if (wallCar.legSyncPhase == syncIndex)
            visualLegPosition = legTransform.position;
    }

    public void UpdateKnee() {
        Vector3 legSurfaceNormal = Vector3.Cross(wallCar.transform.up, legModel.position - sholder.position).normalized;
        float sholderToFootDist = (legModel.position - sholder.position).magnitude;
        float kneeRisingAngleCos = (sholderToFootDist* sholderToFootDist + aboveKneeLength*aboveKneeLength - beloveKneeLength*beloveKneeLength)/
            (2* aboveKneeLength* sholderToFootDist);
        float kneeRisingAngleSin = Mathf.Sqrt(1 - (kneeRisingAngleCos * kneeRisingAngleCos));
        Quaternion rotation = new Quaternion(legSurfaceNormal.x * kneeRisingAngleSin, legSurfaceNormal.y * kneeRisingAngleSin, 
            legSurfaceNormal.z * kneeRisingAngleSin, kneeRisingAngleCos);
        Vector3 kneePosition = sholder.position + (rotation * (legModel.position - sholder.position).normalized)*beloveKneeLength;
        kneeTransform.position = kneePosition;
        kneeTransform.LookAt(legModel);
    }
}
