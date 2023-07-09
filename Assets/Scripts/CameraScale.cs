using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScale : MonoBehaviour {

    // Set this to the in-world distance between the left & right edges of your scene.
    public float sceneWidth;

    private Camera mainCam;

    void Start() {
        mainCam = GetComponent<Camera>();
    }

    void Update() {
        float unitsPerPixel = sceneWidth / Screen.width;

        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;

        mainCam.orthographicSize = desiredHalfHeight;
    }
}