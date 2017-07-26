using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wing : MonoBehaviour {

    public float liftCoefficient = 4.91f;
    public float wingAngleCoefficient = 0.2f;
    public float rollCoefficient = 0.0001f;
    private Rigidbody playerRb;
    private bool wingsDeployed = false;

	// Use this for initialization
	void Start () {
        playerRb = FindObjectOfType<SteamVR_ControllerManager>().gameObject.GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector3 velocity = playerRb.velocity;
        if (velocity.z > 0.995 * velocity.magnitude)
        {
            wingsDeployed = true;
        }
        // Only deploy wings when we reach crusing angle
        if (wingsDeployed)
        {
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float liftReduction = wingAngleCoefficient * (1 - localVelocity.y / localVelocity.magnitude);
            Vector3 lift = playerRb.transform.forward * playerRb.mass * liftCoefficient * (1f - liftReduction);
            Vector3 roll = -playerRb.transform.forward * playerRb.mass * rollCoefficient * localVelocity.z / localVelocity.magnitude;
            playerRb.AddForce(lift);
            Debug.Log(lift.ToString("F4"));
            playerRb.AddForceAtPosition(roll, transform.position);
            
        } else
        {
            playerRb.MoveRotation(Quaternion.LookRotation(Quaternion.Euler(-90f, 0, 0) * velocity, velocity));
        }
        //transform.
        //transform.to
        //playerRb.Addf
	}
}
