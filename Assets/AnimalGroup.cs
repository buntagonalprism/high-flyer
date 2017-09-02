using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalGroup : MonoBehaviour {

    public Waypoint currentWaypoint;
    public bool doesStandStill;
    public float standMinTime;
    public float standMaxTime;

    private bool isIdle;
    private float nextMoveTime;

    private List<Waypoint> waypoints = new List<Waypoint>();
    private List<NavMeshAgent> animals = new List<NavMeshAgent>();

	// Use this for initialization
	void Start () {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Waypoint>())
                waypoints.Add(child.GetComponent<Waypoint>());
            else if (child.GetComponent<NavMeshAgent>())
                animals.Add(child.GetComponent<NavMeshAgent>());
        }
        isIdle = true;
        nextMoveTime = Time.time + Random.Range(1, 3);
        if (waypoints.Count > 0)
            currentWaypoint = waypoints[0];
	}
	
	// Update is called once per frame
	void Update () {
        // Start moving to next waypoint once we've been idle long enough
        if (isIdle && Time.time > nextMoveTime)
        {
            if (currentWaypoint.ValidTargets.Count > 0)
            {
                currentWaypoint = currentWaypoint.ValidTargets[Random.Range(0, currentWaypoint.ValidTargets.Count)];
                List<NavMeshAgent> updated = new List<NavMeshAgent>();
                foreach (NavMeshAgent animal in animals)
                {
                    Vector3 location;
                    do
                    {
                        location = new Vector3(currentWaypoint.transform.position.x + Random.Range(-10, 10), currentWaypoint.transform.position.y, currentWaypoint.transform.position.z + Random.Range(-10, 10));
                    } while (doesTargetOverlap(location, animal.radius, updated));
                    animal.SetDestination(location);
                    updated.Add(animal);

                }
                isIdle = false;
            }
        }
        // Detecting all agents in the group reaching their target 
        else if (!isIdle)
        {
            bool isIdleTemp = true;
            foreach (NavMeshAgent animal in animals)
            {
                if (animal.remainingDistance > 0.5f)
                {
                    isIdleTemp = false;
                    break;
                }
                if (isIdleTemp)
                {
                    isIdle = true;
                    nextMoveTime = Time.time + Random.Range(10, 30);
                }
            }
        }
	}

    private void OnDrawGizmosSelected()
    {
        List<Waypoint> editorWaypoints = new List<Waypoint>();
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Waypoint>())
                editorWaypoints.Add(child.GetComponent<Waypoint>());
        }

        foreach (Waypoint waypoint in editorWaypoints)
        {
            Gizmos.DrawWireSphere(waypoint.transform.position, 1);
            foreach (Waypoint target in waypoint.ValidTargets)
            {
                Gizmos.DrawLine(waypoint.transform.position, target.transform.position);
                Gizmos.DrawWireCube(Vector3.Lerp(waypoint.transform.position, target.transform.position, 0.1f), Vector3.one);
            }
        }
    }



    private bool doesTargetOverlap(Vector3 testLocation, float testRadius, List<NavMeshAgent> referenceTargets)
    {
        foreach (NavMeshAgent reference in referenceTargets)
        {
            if (Vector3.Distance(testLocation, reference.destination) < ((testRadius + reference.radius) * 1.5f))
                return true;
        }
        return false;
    }
}
