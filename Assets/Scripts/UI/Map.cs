using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public Transform tracker;
    public RectTransform cross, vertBase, vert;
    public float isoAngle, isoMult;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        cross.anchoredPosition = XZconvert(new Vector2(tracker.position.x, tracker.position.z)*isoMult);
        vertBase.anchoredPosition = cross.anchoredPosition;
        vert.anchoredPosition = new Vector2(vert.anchoredPosition.x, tracker.position.y*isoMult);
    }

    Vector2 XZconvert(Vector2 XZ)
    {
        float xIso = (XZ.x - XZ.y) * Mathf.Cos(Mathf.Deg2Rad * isoAngle);
        float yIso = (XZ.x + XZ.y) * Mathf.Sin(Mathf.Deg2Rad * isoAngle);
        return new Vector2(xIso, yIso);
    }
}
