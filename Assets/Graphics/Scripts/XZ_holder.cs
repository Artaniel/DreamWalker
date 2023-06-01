using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XZ_holder : MonoBehaviour
{

    public Transform trackingTransform;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(trackingTransform.position.x, 0f, trackingTransform.position.z);
    }
}
