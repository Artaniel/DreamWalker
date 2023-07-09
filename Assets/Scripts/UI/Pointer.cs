using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pointer : MonoBehaviour
{
    public Image image;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        image.rectTransform.anchoredPosition = Input.mousePosition;
    }

    void Hide()
    {
        image.enabled = false;
    }

    void Show()
    {
        image.enabled = true;
    }
}
