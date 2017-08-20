using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlane : MonoBehaviour {

    private Rigidbody rb;
    private bool deployedWings = false;
    private Wings wings;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        wings = GetComponent<Wings>();

        Camera camera = FindObjectOfType<Camera>();
        float[] distances = new float[32];
        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = 1000;
        }
        distances[9] = 3600;
        distances[10] = 2000;
        camera.layerCullDistances = distances;
        
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector3 vel = rb.velocity;
        if (!deployedWings)
        {
            if (vel.magnitude > 5f && vel.y < 0.1)
            {
                deployedWings = true;
                wings.setWingsDeployed(true);
                rb.useGravity = false; 
            }
            if (vel.magnitude > 5f)
            {
                rb.MoveRotation(Quaternion.LookRotation(vel, Quaternion.Euler(-90f, 0, 0) * vel));
            }
        }
	}
}
