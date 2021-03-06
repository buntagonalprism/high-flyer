﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wings : MonoBehaviour {

    //public float liftCoefficient = 4.91f;
    //public float wingAngleCoefficient = 0.2f;
    [Header("Flight control configuration")]
    public float pitchCoefficient = 0.01f;
    public float rollCoefficient = 0.05f;
    public float yawCoefficient = 0.01f;
    public float levelingRate = 0.1f;

    [Header("Wing Components")]
    public GameObject rightWing;
    public GameObject leftWing;
    public GameObject elevators;


    [Header("Control Wire Components")]
    public GameObject leftWingWirePos;
    public GameObject leftWingBasePos;
    public GameObject rightWingWirePos;
    public GameObject rightWingBasePos;
    public GameObject leftTailWirePos;
    public GameObject leftTailBasePos;
    public GameObject rightTailWirePos;
    public GameObject rightTailBasePos;

    public GameObject lineRendererPrefab;

    [Header("Landing Controls")]
    public GameObject landingZone;
    public float landingSpeed;

    private float landingStartTime;
    private float landingDuration;
    private Vector3 landingStartPosition;
    private Quaternion landingStartRotation;
    private Quaternion levelFlight;

    private List<ControlWire> controlWires = new List<ControlWire>();
    private Rigidbody playerRb;
    private bool wingsDeployed = false;
    private bool isLanding = false;

    private float steeringAngle = 0.0f;
    private float pitchAngle = 0.0f;

    public void SetSteeringAngle(float angle)
    {
        steeringAngle = angle;
    }

    public void SetPitchAngle(float angle)
    {
        pitchAngle = angle;
    }

    class ControlWire
    {
        public ControlWire(LineRenderer renderer, GameObject start, GameObject finish)
        {
            this.wireRenderer = renderer;
            this.start = start;
            this.finish = finish;
        }
        public LineRenderer wireRenderer;
        public GameObject start;
        public GameObject finish;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Lander"))
        {
            isLanding = true;
            wingsDeployed = false;
            float distance = Vector3.Distance(transform.position, landingZone.transform.position);
            levelFlight = Quaternion.LookRotation(new Vector3(landingZone.transform.position.x - transform.position.x, 0f, landingZone.transform.position.z - transform.position.z), Vector3.up);
            landingStartPosition = transform.position;
            landingStartRotation = transform.rotation;
            landingStartTime = Time.time;
            landingDuration = distance / landingSpeed;
            playerRb.isKinematic = true;
        }
    }

    // Use this for initialization
    void Start () {
        playerRb = FindObjectOfType<PlayerPlane>().gameObject.GetComponent<Rigidbody>();

        // Instantiate the line renderers for the control wires
        GameObject wire = Instantiate(lineRendererPrefab, transform);
        LineRenderer renderer = wire.GetComponent<LineRenderer>();
        controlWires.Add(new ControlWire(renderer, leftWingWirePos, leftWingBasePos));

        wire = Instantiate(lineRendererPrefab, transform);
        renderer = wire.GetComponent<LineRenderer>();
        controlWires.Add(new ControlWire(renderer, rightWingWirePos, rightWingBasePos));

        wire = Instantiate(lineRendererPrefab, transform);
        renderer = wire.GetComponent<LineRenderer>();
        controlWires.Add(new ControlWire(renderer, leftTailWirePos, leftTailBasePos));

        wire = Instantiate(lineRendererPrefab, transform);
        renderer = wire.GetComponent<LineRenderer>();
        controlWires.Add(new ControlWire(renderer, rightTailWirePos, rightTailBasePos));

    }
	
    public void setWingsDeployed(bool wingsDeployed)
    {
        this.wingsDeployed = wingsDeployed;
    }



    private void Update()
    {
        // Update line renderers to show the control wires 
        foreach (ControlWire controlWire in controlWires)
        {
            controlWire.wireRenderer.SetPosition(0, controlWire.start.transform.position);
            controlWire.wireRenderer.SetPosition(1, controlWire.finish.transform.position);
        }
    }

    // Update is called once per frame
    void FixedUpdate () {
        //Vector3 velocity = playerRb.velocity;
        // Only deploy wings when we reach crusing angle
        if (wingsDeployed)
        {
            //Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            //float liftReduction = wingAngleCoefficient * (1 - localVelocity.y / localVelocity.magnitude);
            //Vector3 lift = -playerRb.transform.forward * playerRb.mass * liftCoefficient * (1f - liftReduction);
            //Vector3 roll = playerRb.transform.forward * playerRb.mass * rollCoefficient * localVelocity.z / localVelocity.magnitude;
            //playerRb.AddForce(lift);
            // Debug.Log(lift.ToString("F4"));
            //playerRb.AddForceAtPosition(roll, transform.position);

            // Old wing-driven pitch
            //float rPitch = -pitchCoefficient * wrapAngle180(rightWing.transform.localEulerAngles.x);
            //float lPitch = -pitchCoefficient * wrapAngle180(leftWing.transform.localEulerAngles.x);
            //Debug.Log("RPitch:" + rPitch.ToString("F8"));
            //Debug.Log("LPitch:" + lPitch.ToString("F8"));

            /** Approach 2 **/
            /*
            float pitch = pitchCoefficient * wrapAngle180(elevators.transform.localEulerAngles.x);

            // Roll direction depends on which side of the body the wing is
            float rRoll = rollCoefficient * (rightWing.transform.localPosition.x > 0 ? 1 : -1) * wrapAngle180(rightWing.transform.localEulerAngles.x);
            float lRoll = rollCoefficient * (leftWing.transform.localPosition.x > 0 ? 1 : -1) * wrapAngle180(leftWing.transform.localEulerAngles.x);
            //Debug.Log("RRoll:" + rRoll.ToString("F8"));
            //Debug.Log("LRoll:" + lRoll.ToString("F8"));
            //float yaw = -0.8f * rollCoefficient * wrapAngle180(transform.eulerAngles.z);
            float yaw = -rollCoefficient * wrapAngle180(rightWing.transform.localEulerAngles.x);
            //Debug.Log("Yaw:" + yaw.ToString("F8"));
            //Quaternion levelFlight = Quaternion.LookRotation(new Vector3(playerRb.velocity.x, 0f, playerRb.velocity.z), Vector3.up);
            Quaternion newRotation = playerRb.rotation * Quaternion.AngleAxis(pitch, transform.right) * Quaternion.AngleAxis(yaw, transform.up);
            //float currentRoll = wrapAngle180(newRotation.eulerAngles.z); //AngleAxis(rRoll + lRoll, transform.forward)
            //float targetRoll = wrapAngle180(rightWing.transform.localEulerAngles.x);
            //newRotation = newRotation * Quaternion.AngleAxis(targetRoll - currentRoll, transform.forward);
            //float angle = Quaternion.Angle(levelFlight, newRotation);
            //playerRb.rotation = Quaternion.Slerp(newRotation, levelFlight, levelingRate / angle);
            */


            /** Approach 3: **/
            //float roll = rollCoefficient * wrapAngle180(rightWing.transform.localEulerAngles.x + 90f);
            //float pitch = pitchCoefficient * wrapAngle180(elevators.transform.localEulerAngles.x + 90f);
            float roll = rollCoefficient * -steeringAngle;
            float pitch = pitchCoefficient * pitchAngle;
            playerRb.transform.Rotate(pitch, 0f, roll, Space.Self);
            float yaw = -yawCoefficient * getRollToHorizontal(transform);
            playerRb.transform.Rotate(0f, yaw, 0f, Space.World);

            //playerRb.transform.localEulerAngles = new Vector3(playerRb.transform.localEulerAngles.x, playerRb.transform.localEulerAngles.y, targetRoll);
            //playerRb.rotation = newRotation;
            playerRb.velocity = playerRb.transform.forward * playerRb.velocity.magnitude;
            //playerRb.angularVelocity = Vector3.zero;
            //playerRb.AddRelativeTorque(pitch, 0, roll);
            // Velocity is always in the forwards direction
            //transform.
            //
        }
        else if (isLanding)
        {
            float ratio = (Time.time - landingStartTime) / landingDuration;
            if (ratio < 1)
            {
                transform.rotation = Quaternion.Slerp(landingStartRotation, levelFlight, ratio);
                transform.position = Vector3.Lerp(landingStartPosition, landingZone.transform.position, ratio);
            } else
            {
                isLanding = false;
                FindObjectOfType<LevelManager>().LoadLevel("End");
            }
        }
	}



    private float getRollToHorizontal(Transform t)
    {
        Vector3 vf = t.forward;
        Vector3 vr = t.right;
        float xrh = Mathf.Sqrt(1f / (1f + ((vf.x * vf.x) / (vf.z * vf.z))));
        xrh = xrh * (vr.x >= 0f ? 1f : -1f) * (t.up.y >= 0f ? 1f : -1f);
        float zrh = -1f * vf.x * xrh / vf.z;
        float roll = Vector3.Angle(new Vector3(xrh, 0f, zrh), vr) * (vr.y >= 0 ? 1 : -1);
        //Debug.Log("Roll:" + roll.ToString("F4"));
        return roll;
    }

    private float wrapAngle180(float angle)
    {
        if (angle >= 360f || angle <= -360f)
            angle = angle % 360f;
        if (angle >= 180f)
            angle -= 360f;
        else if (angle < -180f)
            angle += 360f;
        return angle;
    }
}
