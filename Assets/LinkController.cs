using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkController : MonoBehaviour {

    public float steeringAngle = 0;
    public float radius = 1.145f;
    public float wheelHeight = 2f;
    public GameObject steeringGear;
    public GameObject driveGear;
    private float wheelRotation;
    private Link[] links;

    private float piOn2 = Mathf.PI * 0.5f;
    private float piOn6 = Mathf.PI * 0.166667f;

	// Use this for initialization
	void Start () {
        links = GetComponentsInChildren<Link>();
        int linkId = 0;
        foreach (Link link in links)
        {
            link.LinkId = linkId;
            if (linkId == 0)
                linkId = 1;
            else if (linkId > 0)
                linkId *= -1;
            else {
                linkId = (linkId - 1) * -1;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        float anglePos;
        Vector3 position;
        Vector3 rotation;
        float x = steeringAngle;
        rotation = steeringGear.transform.localEulerAngles;
        rotation.z = -90f - steeringAngle;
        steeringGear.transform.localEulerAngles = rotation;
        rotation = driveGear.transform.localEulerAngles;
        rotation.z = -90f - steeringAngle;
        driveGear.transform.localEulerAngles = rotation;
        wheelRotation = -1 * (x > 180 ? x - 360: x) * Mathf.Deg2Rad;
		foreach (Link link in links)
        {
            anglePos = ((float) link.LinkId) * piOn6 + wheelRotation;
            position = link.transform.localPosition;
            rotation = link.transform.localEulerAngles;
            if (anglePos > piOn2)
            {
                position.y = radius;
                position.z = -radius * (anglePos - piOn2);
                rotation.x = -90;
            } else if (anglePos < -piOn2)
            {
                position.y = -radius;
                position.z = radius * (anglePos + piOn2);
                rotation.x = 90f;
            } else
            {
                position.y = radius * Mathf.Sin(anglePos);
                position.z = radius * Mathf.Cos(anglePos);
                rotation.x = -Mathf.Rad2Deg * anglePos;
            }
            if (position.z < -wheelHeight)
            {
                position.y = 0;
                position.z = -wheelHeight;
            }
       
            link.transform.localPosition = position;
            link.transform.localEulerAngles = rotation;
            
        }
	}
}
