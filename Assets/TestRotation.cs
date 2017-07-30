using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotation : MonoBehaviour {

    public float x;
    public float y;
    public float z;
    public float dx = 0f;
    public float dy = 0f;
    public float dz = 0f;
	
	// Update is called once per frame
	void Update () {
        //transform.rotation = Quaternion.Euler(x, y, z);
        Vector3 vf = transform.forward;
        Vector3 vr = transform.right;
        float xrh = Mathf.Sqrt(1f / (1f + ((vf.x * vf.x) / (vf.z * vf.z))));
        xrh = xrh * (vr.x >= 0f ? 1f : -1f) * (transform.up.y >= 0f ? 1f : -1f);
        float zrh = -1f * vf.x * xrh / vf.z;
        float roll = Vector3.Angle(new Vector3(xrh, 0f, zrh), vr) * (vr.y >= 0 ? 1 : -1);
        Debug.Log("Roll:" + roll.ToString("F4"));
	}

    private void FixedUpdate()
    {
        transform.Rotate(dx, dy, dz, Space.Self);
    }
}
