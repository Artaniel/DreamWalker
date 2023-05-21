using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderLeg : MonoBehaviour
{
    public Transform legTransform;
    public Transform spiderBody;
    public Transform[] surfaceFindingPathNeutral;
    public Transform[] surfaceFindingPathForward;

    void Update()
    {
        FindContactPoint(surfaceFindingPathNeutral);
    }

    private Vector3 FindContactPoint(Transform[] path) {
        RaycastHit[] hits;
        RaycastHit hit;        
        for (int i = 0; i < path.Length-1; i++) {
            hits = Physics.RaycastAll(path[i].position, path[i + 1].position - path[i].position, 
                Vector3.Distance(path[i].position, path[i + 1].position));
            if (GetHitFromArray(hits, out hit))
                Debug.DrawRay(hit.point, hit.normal, Color.cyan);
        }

        return Vector3.zero; // if not found
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
