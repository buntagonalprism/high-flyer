using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalGroup : MonoBehaviour {

    public Waypoint currentWaypoint;
    public bool doesStandStill;
    public float standMinTime;
    public float standMaxTime;

    // Represents whether the group motion is idle
    private bool isGroupIdle;
    private float nextMoveTime;

    private List<Waypoint> waypoints = new List<Waypoint>();
    private List<Animal> animals = new List<Animal>();

	// Use this for initialization
	void Start () {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Waypoint>())
                waypoints.Add(child.GetComponent<Waypoint>());
            else if (child.GetComponent<Animal>())
            {
                // Randomise clip start position to avoid eerir synchronised movement
                Animal animal = child.GetComponent<Animal>();
                // Animals which can stand still have an idle clip
                if (doesStandStill)
                {
                    animal.animator.Play("Idle", 0, Random.Range(0f, 1f));
                }

                // Other animals we need to get a reference to the current clip, which could have a different name (swim, fly etc)
                else
                {
                    //AnimationClip clip = animal.animator.GetCurrentAnimatorClipInfo(0)[0].clip;
                    animal.animator.Play(animal.animator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, Random.Range(0f, 1f));
                }
                animals.Add(animal);
            }
        }
        isGroupIdle = true;
        nextMoveTime = Time.time + Random.Range(standMinTime, standMaxTime);
        if (waypoints.Count > 0 && currentWaypoint == null)
            currentWaypoint = waypoints[0];
        if (!doesStandStill)
            updateGroupDestinations();
	}
	
	// Update is called once per frame
	void Update () {
        // Start moving an idle to next waypoint once we've been idle long enough
        if (doesStandStill && isGroupIdle && Time.time > nextMoveTime)
        {
            updateGroupDestinations();
        }
        // Detecting all agents in the group reaching their target 
        else if (doesStandStill && !isGroupIdle)
        {
            bool allAnimalsIdle = true;
            foreach (Animal animal in animals)
            {
                if (!animal.IsIdle && animal.agent.remainingDistance < 0.5f)
                {
                    animal.IsIdle = true;
                    animal.animator.SetBool("isWalking", false);
                    animal.animator.Play("Idle", 0, Random.Range(0f, 1f));
                } else if (!animal.IsIdle)
                {
                    allAnimalsIdle = false;
                }
            }
            if (allAnimalsIdle)
            {
                isGroupIdle = true;
                nextMoveTime = Time.time + Random.Range(standMinTime, standMaxTime);
            }
        }
        // Detect any agent reaching target for groups that don't stop moving
        else if (!doesStandStill)
        {
         
            foreach (Animal animal in animals)
            {
                if (animal.agent.remainingDistance < 0.2f )
                {
                    updateGroupDestinations();                
                }
            }

        }

        // Update animation speed
        foreach (Animal animal in animals)
        {
            float animationSpeed = animal.baseAnimationSpeed * Mathf.Clamp(animal.agent.velocity.magnitude / animal.agent.speed, 0.2f, 1f);
            animal.animator.SetFloat("animSpeedScale", animationSpeed);
        }
	}

    public void OnDrawGizmosFromChild() {
        OnDrawGizmosSelected();
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

    private void updateGroupDestinations()
    {
        if (currentWaypoint.ValidTargets.Count > 0)
        {
            currentWaypoint = currentWaypoint.ValidTargets[Random.Range(0, currentWaypoint.ValidTargets.Count)];
            List<Animal> updated = new List<Animal>();
            foreach (Animal animal in animals)
            {
                Vector3 location;
                int loops = 0;
                float animalRadius = animal.agent.radius * animal.transform.localScale.x;
                do
                {
                    loops++;
                    if (loops % 1000 == 0)
                        throw new System.Exception("excessive loops required to place animals");
                    location = new Vector3(currentWaypoint.transform.position.x + Random.Range(-5 * animalRadius, 5*animalRadius), currentWaypoint.transform.position.y, currentWaypoint.transform.position.z + Random.Range(-5 * animalRadius, 5 * animalRadius));
                } while (doesTargetOverlap(location, animalRadius, updated));
                //Debug.Log("loops required: " + loops);
                animal.agent.SetDestination(location);
                
                if (doesStandStill)
                {
                    animal.IsIdle = false;
                    animal.animator.SetBool("isWalking", true);
                    //animal.animator.no
                    //animal.animator.Play("Walk", 0, Random.Range(0f, 1f));
                }
                updated.Add(animal);

            }
            if (doesStandStill)
            {
                isGroupIdle = false;
            }
        }
    }

    private bool doesTargetOverlap(Vector3 testLocation, float testRadius, List<Animal> referenceTargets)
    {
        foreach (Animal reference in referenceTargets)
        {
            if (Vector3.Distance(testLocation, reference.agent.destination) < (testRadius * 2.2f))
                return true;
        }
        return false;
    }
}
