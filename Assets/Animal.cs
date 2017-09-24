using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour {

    [HideInInspector]
    public NavMeshAgent agent;
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public bool IsIdle = false;
    [HideInInspector]
    new public Renderer renderer;

    public string name;
    public float maxVisibleRange = 60f;
    public float baseAnimationSpeed = 1.0f;

	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        renderer = GetComponentInChildren<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
