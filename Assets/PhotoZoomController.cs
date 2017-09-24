using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoZoomController : MonoBehaviour {

    public int zoomLevel = 1;
    public int maxZoomLevel = 10;
    public int minFov;
    private Camera photoCamera;
    private float baseFov;

    // Use this for initialization
    void Start()
    {
        photoCamera = GetComponent<Camera>();
        baseFov = photoCamera.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        zoomLevel = Mathf.Clamp(zoomLevel, 1, maxZoomLevel);
        photoCamera.fieldOfView = Mathf.Lerp(baseFov, minFov, (float)((zoomLevel - 1) / maxZoomLevel));
    }
}
