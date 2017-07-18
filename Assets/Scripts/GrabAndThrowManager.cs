using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabAndThrowManager : MonoBehaviour {

    [Header("SteamVR Camera Rig Components")]
    public ControllerInputDetector leftHand;
    public ControllerInputDetector rightHand;
    public GameObject playerHead;

    [Header("Throwing Speeds")]
    public float throwBoost = 1.4f;

    // Object grab tracking
    private GameObject placingObj = null;
    private List<GameObject> holdingObjs = new List<GameObject>();

    public void GrabObjectLeft(Collider collider)
    {
        GrabObject(collider, leftHand);
    }

    public void GrabObjectRight(Collider collider)
    {
        GrabObject(collider, rightHand);
    }

    void GrabObject(Collider collider, ControllerInputDetector hand)
    {
        if (collider.gameObject.CompareTag("Throwable") || collider.gameObject.CompareTag("Placeable") || collider.gameObject.CompareTag("PlaceableChild"))
        {
            // Grab hold of throwable or placeable objects when trigger held
            if (hand.IsTriggerDown())
            {
                bool keepWorldPosition = true;
                // Used when sub-colliders are used to build parent object shape. 
                if (collider.gameObject.CompareTag("PlaceableChild"))
                {
                    collider.transform.parent.transform.SetParent(hand.transform, keepWorldPosition);
                    holdingObjs.Add(collider.transform.parent.gameObject);
                    // Disable child colliders to prevent child trigger events being passed up to the controller
                    Collider[] childColliders = collider.transform.parent.GetComponentsInChildren<Collider>();
                    foreach (Collider childCollider in childColliders)
                    {
                        childCollider.enabled = false;
                    }
                }
                // Throwable and placeable objects
                else
                {
                    collider.transform.SetParent(hand.transform, keepWorldPosition);
                    collider.GetComponent<Rigidbody>().isKinematic = true;
                    holdingObjs.Add(collider.gameObject);
                }
                hand.HapticPulse(2000);

            }
        }
    }



    public void ReleaseObjectsRight()
    {
        ReleaseObjects(rightHand);
    }

    public void ReleaseObjectsLeft()
    {
        ReleaseObjects(leftHand);
    }

    void ReleaseObjects(ControllerInputDetector hand)
    {
        foreach (GameObject heldObj in holdingObjs)
        {
            // Throw by imparting controller velocity to held object
            if (heldObj.CompareTag("Throwable"))
            {
                heldObj.transform.SetParent(null);
                Rigidbody rigidBody = heldObj.GetComponent<Rigidbody>();
                rigidBody.isKinematic = false;
                rigidBody.velocity = hand.GetVelocity() * throwBoost;
                rigidBody.angularVelocity = hand.GetAngularVelocity();
            }
            // Place by removing parent transform relationship
            else
            {
                heldObj.transform.SetParent(null);
                Collider[] childColliders = heldObj.GetComponentsInChildren<Collider>();
                foreach (Collider childCollider in childColliders)
                {
                    childCollider.enabled = true;
                }
            }
        }
    }
}
