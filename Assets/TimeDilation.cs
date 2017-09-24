using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeDilation : MonoBehaviour {

    [Range(0,9)]
    public int dialationFactor;
    private float baseFixedDeltaTime;

	// Use this for initialization
	void Start () {
        baseFixedDeltaTime = Time.fixedDeltaTime;
	}
	
	// Update is called once per frame
	void Update () {
        float dialationRatio = (10f - dialationFactor) / 10f;
        Time.timeScale = dialationRatio;
        Time.fixedDeltaTime = baseFixedDeltaTime * dialationRatio;
	}
}
