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

    [Header("Flight Controls")]
    public float heightSensitivity = 100f;
    public float referenceHeight = 1.25f;
    public float angleSensitivity = 1f;
    public float referenceJoystick = 0.4f;
    public float joystickSensitivity = 50f;
    public GameObject leftWing;
    public GameObject rightWing;
    public GameObject elevators;


    //private SteamVR_TrackedObject leftTrackedObj;
    //private SteamVR_Controller.Device leftDevice;
    //private SteamVR_TrackedObject rightTrackedObj;
    //private SteamVR_Controller.Device rightDevice;

    private Vector3 leftLocalPos;
    private Vector3 rightLocalPos;
    private Vector3 steeringVector;
    private Vector2 centrePosition = Vector2.zero;
    private Vector2 headPosition = Vector2.zero;

    // Object grab tracking
    private MovementAndUiManager movementManager;
    private GrabAndThrowManager grabAndThrowManager;

    // Use this for initialization
    void Start()
    {
        movementManager = GetComponent<MovementAndUiManager>();
        grabAndThrowManager = GetComponent<GrabAndThrowManager>();

        //leftTrackedObj = leftHand.GetComponent<SteamVR_TrackedObject>();
        //rightTrackedObj = rightHand.GetComponent<SteamVR_TrackedObject>();

        leftHand.OnGripPress += movementManager.WalkForward;
        //leftHand.OnTouchpad += movementManager.ShowTeleportBeamLeft;
        //leftHand.OnTouchpadUp += movementManager.StartDashing;
        leftHand.OnTouchpadPressDown += movementManager.ClickUiButton;
        //leftHand.OnColliderTriggerStay += new ControllerInputDetector.TriggerHandler(grabAndThrowManager.GrabObjectLeft);
        //leftHand.OnTriggerPressUp += grabAndThrowManager.ReleaseObjectsLeft;
        rightHand.OnColliderTriggerStay += new ControllerInputDetector.TriggerHandler(grabAndThrowManager.GrabObjectRight);
        rightHand.OnTriggerPressUp += grabAndThrowManager.ReleaseObjectsRight;
        leftHand.OnTriggerPressDown += LeftHand_OnTriggerPressDown;
  
    }

    private void Update()
    {
        //leftDevice = SteamVR_Controller.Input((int)leftTrackedObj.index);
        //rightDevice = SteamVR_Controller.Input((int)rightTrackedObj.index);
        leftLocalPos = leftHand.transform.localPosition;
        rightLocalPos = rightHand.transform.localPosition;
        steeringVector = leftLocalPos - rightLocalPos;
        float steeringAngle = Mathf.Atan2(steeringVector.y , Mathf.Abs(steeringVector.x)) * Mathf.Rad2Deg;
        //Debug.Log("Steering:" + steeringAngle.ToString("F5"));
        leftWing.transform.localEulerAngles = new Vector3(angleSensitivity * steeringAngle, 0, 0);
        rightWing.transform.localEulerAngles = new Vector3(-angleSensitivity * steeringAngle, 0, 0);

        // Control the elevators with forward/back displacement
        centrePosition.x = (leftLocalPos.x + rightLocalPos.x) * 0.5f;
        centrePosition.y = (leftLocalPos.z + rightLocalPos.z) * 0.5f;
        headPosition.x = playerHead.transform.parent.transform.localPosition.x;
        headPosition.y = playerHead.transform.parent.transform.localPosition.z;
        float joystickDisplacment = (centrePosition - headPosition).magnitude - referenceJoystick;
        elevators.transform.localEulerAngles = new Vector3(joystickSensitivity * joystickDisplacment, 0f, 0f);

        //leftWing.transform.localEulerAngles = new Vector3(heightSensitivity * ( leftHand.transform.localPosition.y - referenceHeight), 0, 0);
        //rightWing.transform.localEulerAngles = new Vector3(heightSensitivity * ( rightHand.transform.localPosition.y - referenceHeight), 0, 0);
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
