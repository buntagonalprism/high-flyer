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
    public GameObject player;

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
        //leftHand.OnColliderTriggerStay += new ControllerInputDetector.TriggerHandler(grabAndThrowManager.GrabObjectLeft);
        //leftHand.OnTriggerPressUp += grabAndThrowManager.ReleaseObjectsLeft;
        rightHand.OnColliderTriggerStay += new ControllerInputDetector.TriggerHandler(grabAndThrowManager.GrabObjectRight);
        rightHand.OnTriggerPressUp += grabAndThrowManager.ReleaseObjectsRight;
        leftHand.OnTriggerPressDown += LeftHand_OnTriggerPressDown;
  
    }

    private void LeftHand_OnTriggerPressDown()
    {
        Debug.Log("###### ABSOLUTE #######");
        Debug.Log("Left pos" + leftHand.transform.position.ToString("F4"));
        Debug.Log("Left rot" + leftHand.transform.rotation.eulerAngles.ToString("F4"));
        Debug.Log("Right pos" + rightHand.transform.position.ToString("F4"));
        Debug.Log("Right rot" + rightHand.transform.rotation.eulerAngles.ToString("F4"));
        Debug.Log("Head pos" + playerHead.transform.position.ToString("F4"));
        Debug.Log("Head rot" + playerHead.transform.rotation.eulerAngles.ToString("F4"));

        Debug.Log("###### RELATIVE #######");
        Debug.Log("Left pos" + leftHand.transform.localPosition.ToString("F4"));
        Debug.Log("Left rot" + leftHand.transform.localEulerAngles.ToString("F4"));
        Debug.Log("Right pos" + rightHand.transform.localPosition.ToString("F4"));
        Debug.Log("Right rot" + rightHand.transform.localEulerAngles.ToString("F4"));
        Debug.Log("Head pos" + playerHead.transform.parent.transform.localPosition.ToString("F4"));
        Debug.Log("Head rot" + playerHead.transform.parent.transform.localEulerAngles.ToString("F4"));
    }
}
