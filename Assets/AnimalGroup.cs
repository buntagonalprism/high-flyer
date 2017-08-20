using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalGroup : MonoBehaviour {

    public Waypoint currentWaypoint;

    private bool isIdle;
    private float nextMoveTime;

    private List<Waypoint> waypoints;
    private List<GameObject> animals;

	// Use this for initialization
	void Start () {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Waypoint>())
                waypoints.Add(child.GetComponent<Waypoint>());
            else
                animals.Add(child.gameObject);
        }
        isIdle = true;
        nextMoveTime = Time.time + Random.Range(10, 30);
	}
	
	// Update is called once per frame
	void Update () {
        // Start movingto next waypoint once we've been idle long enough
		if (isIdle && Time.time > nextMoveTime)
        {
            if (currentWaypoint.ValidTargets.Count > 0)
            {
                Waypoint waypoint = currentWaypoint.ValidTargets[Random.Range(0, currentWaypoint.ValidTargets.Count)];

            }
        }
	}
}
