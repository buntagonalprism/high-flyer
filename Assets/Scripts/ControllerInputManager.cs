using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControllerInputManager : MonoBehaviour {

    [Header("SteamVR Camera Rig Components")]
    public ControllerInputDetector leftHand;
    public ControllerInputDetector rightHand;
    public GameObject playerHead;

    // Object grab tracking
    private MovementAndUiManager movementManager;
    private GrabAndThrowManager grabAndThrowManager;

    // Use this for initialization
    void Start()
    {
        movementManager = GetComponent<MovementAndUiManager>();
        grabAndThrowManager = GetComponent<GrabAndThrowManager>();
        
        leftHand.OnGripPress += movementManager.WalkForward;
        leftHand.OnTouchpad += movementManager.ShowTeleportBeamLeft;
        leftHand.OnTouchpadUp += movementManager.StartDashing;
        leftHand.OnTouchpadPressDown += movementManager.ClickUiButton;
        leftHand.OnColliderTriggerStay += new ControllerInputDetector.TriggerHandler(grabAndThrowManager.GrabObjectLeft);
        leftHand.OnTriggerPressUp += grabAndThrowManager.ReleaseObjectsLeft;
        rightHand.OnColliderTriggerStay += new ControllerInputDetector.TriggerHandler(grabAndThrowManager.GrabObjectRight);
        rightHand.OnTriggerPressUp += grabAndThrowManager.ReleaseObjectsRight;
  
    }



 


}
