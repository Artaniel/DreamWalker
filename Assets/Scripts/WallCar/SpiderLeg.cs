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
    public float maxStepLenght = 5f;
    public bool torchingGround = false;

    public Transform sholder;
    public Transform legModel;
    private const float defaultLegModelLength = 0.8f;

    private Quaternion sholderDefaultRotation;
    private Vector3 legDefaultScale;


    private void Awake()
    {
        sholderDefaultRotation = sholder.localRotation;
        legDefaultScale = legModel.localScale;
    }

    void FixedUpdate()
    {
        if (!wallCar.airBlocksSurfacecheck)
        {
            RaycastHit hit;
            if (CheckPath(surfaceFindingPathNeutral, out hit))
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

    private bool CheckPath(Transform[] path, out RaycastHit foundHit) {
        RaycastHit[] hits;      
        for (int i = 0; i < path.Length-1; i++) {
            hits = Physics.RaycastAll(path[i].position, path[i + 1].position - path[i].position, 
                Vector3.Distance(path[i].position, path[i + 1].position));
            if (GetHitFromArray(hits, out foundHit))
            {
                Debug.DrawLine(path[i].position, path[i + 1].position, Color.green);
                Debug.DrawRay(legTransform.position, currentNormal, Color.cyan);
                return true;
            }
            else
            {
                Debug.DrawLine(path[i].position, path[i + 1].position, Color.red);
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
        sholder.LookAt(legTransform.position);
        sholder.Rotate(transform.forward, -90f);
        legModel.localScale = new Vector3(1f, Vector3.Distance(sholder.position, legTransform.position) * defaultLegModelLength , 1f);
    }
}
