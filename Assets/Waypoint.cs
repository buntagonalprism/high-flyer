using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{

    public List<Waypoint> ValidTargets;



    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, 1);
        foreach (Waypoint target in ValidTargets)
        {
            Gizmos.DrawLine(transform.position, target.transform.position);
            Gizmos.DrawWireCube(Vector3.Lerp(transform.position, target.transform.position, 0.1f), Vector3.one);
        }
        if (transform.parent.GetComponent<AnimalGroup>() != null)
        {
            transform.parent.GetComponent<AnimalGroup>().OnDrawGizmosFromChild();
        }
    }
}
