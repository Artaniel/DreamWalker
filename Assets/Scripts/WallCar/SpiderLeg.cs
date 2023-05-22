using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderLeg : MonoBehaviour
{
    public WallCar wallCar;
    public Transform legTransform;
    public Transform spiderBody;
    public Transform[] surfaceFindingPathNeutral;
    public Transform[] surfaceFindingPathForward;
    [HideInInspector] public Vector3 currentNormal;
    public float minStepLenght = 1f;
    public float maxStepLenght = 5f;
    public bool torchingGround = false;

    private void Awake()
    {
        
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        if (CheckPath(surfaceFindingPathNeutral, out hit))
        {
            if (Vector3.Distance(hit.point, legTransform.position) > minStepLenght || !torchingGround) {
                currentNormal = hit.normal;
                legTransform.position = hit.point;
                torchingGround = true;
                //rotation? vfx? sfx?
            } else if (Vector3.Distance(wallCar.transform.position, transform.position) > maxStepLenght) {
                currentNormal = Vector3.zero;
                legTransform.position = wallCar.transform.position;
            }
        }
    }

    private bool CheckPath(Transform[] path, out RaycastHit foundHit) {
        RaycastHit[] hits;      
        for (int i = 0; i < path.Length-1; i++) {
            hits = Physics.RaycastAll(path[i].position, path[i + 1].position - path[i].position, 
                Vector3.Distance(path[i].position, path[i + 1].position));
            if (GetHitFromArray(hits, out foundHit))
            {
                Debug.DrawRay(foundHit.point, foundHit.normal, Color.cyan);
                return true;
            }
        }
        foundHit = new RaycastHit(); //empty
        return false; // if not found
    }

    private bool GetHitFromArray(RaycastHit[] hits, out RaycastHit foundHit) {
        foreach (RaycastHit hit in hits)
            if (hit.collider.tag != "Player")
            {
                foundHit = hit;
                return true;
            }
        foundHit = new RaycastHit();
        return false;
    }
}
