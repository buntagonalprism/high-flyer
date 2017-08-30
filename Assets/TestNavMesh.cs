using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestNavMesh : MonoBehaviour {

    private NavMeshAgent agent;
    private bool set = false;

	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > 3f && !set)
        {
            Vector3 newPosition = transform.position;
            newPosition.x = newPosition.x + 15f;
            agent.SetDestination(newPosition);
            set = true;
        }
	}
}
