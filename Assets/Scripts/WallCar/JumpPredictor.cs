using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPredictor : MonoBehaviour
{
    public WallCar car;
    public Transform cameraTransform;
    public float stepTime = 0.5f;
    public float maxTime = 10f;
    private GameObject[] markers;
    public GameObject markerPrefab;
    public GameObject wrapping;
    private bool isShown = false;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        markers = new GameObject[(int)(maxTime / stepTime)+1];
        for (int i = 0; i < (maxTime / stepTime); i++) {
            markers[i] = Instantiate(markerPrefab);
            markers[i].SetActive(false);
            markers[i].transform.parent = wrapping.transform;
        }
    }

    private void Update()
    {
        if (!isShown && car.focusIsPressed && car.isOnSurface)
            Show();
        if (isShown && (!car.focusIsPressed || !car.isOnSurface))
            Hide();
        if (isShown)
            PredictorUpdate();
    }

    private void PredictorUpdate() {

        if (markers.Length < maxTime / stepTime)// only for case of changing in runtime
            Init(); 
        Vector3 startongPosition = car.transform.position;
        Vector3 startingVelocity = car.carRigidbody.velocity + cameraTransform.forward * car.jumpPower;
        Vector3 acceleration = 9.8f * Vector3.down;
        Vector3 currentPosition; float time;
        for (int i = 0; i < (maxTime / stepTime); i++)
        {
            time = i * stepTime;
            //r = r0 + v0*t + a*t*t/2
            currentPosition = startongPosition + time * startingVelocity + (time * time / 2f) * acceleration;
            markers[i].transform.position = currentPosition;
        }
    }

    private void Show()
    {
        isShown = true;
        foreach (GameObject marker in markers)
            marker?.SetActive(true);
    }

    private void Hide() {
        isShown = false;
        foreach (GameObject marker in markers)
            marker?.SetActive(false);
    }
}
