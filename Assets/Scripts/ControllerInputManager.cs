using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

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
    public LinkController linkController;
    public GameObject steeringWheel;
    public GameObject elevatorController;
    private float cos25 = Mathf.Cos(25 * Mathf.Deg2Rad);

    private Wings wings;

    public bool flightControlsActive = false;

    // UI Controls
    private LevelManager levelManager;
    private UiInputManager uiInputManager;
    private AnimalPhotoManager photoManager;

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
        //leftHand.OnTouchpadPressDown += movementManager.ClickUiButton;
        //leftHand.OnColliderTriggerStay += new ControllerInputDetector.TriggerHandler(grabAndThrowManager.GrabObjectLeft);
        //leftHand.OnTriggerPressUp += grabAndThrowManager.ReleaseObjectsLeft;
        //rightHand.OnColliderTriggerStay += new ControllerInputDetector.TriggerHandler(grabAndThrowManager.GrabObjectRight);
        //rightHand.OnTriggerPressUp += grabAndThrowManager.ReleaseObjectsRight;

        uiInputManager = FindObjectOfType<UiInputManager>();
        rightHand.OnTriggerPressDown += HitUiButton;
        leftHand.OnTriggerPressDown += HitUiButton;

        leftHand.OnTriggerPressDown += LeftHand_OnTriggerPressDown;

        photoManager = FindObjectOfType<AnimalPhotoManager>();
        leftHand.OnTouchpadPressDown += ToggleCameraLens;
        rightHand.OnTouchpadPressDown += InitiatePhotoGrab;

        levelManager = FindObjectOfType<LevelManager>();

        string sceneName = SceneManager.GetActiveScene().name;
        if (SceneManager.GetActiveScene().name == "Start")
        {
            GetAnimatedComponents();
        }

        wings = FindObjectOfType<Wings>();

        //StartDescent();
        //rightHand.OnTriggerPressDown += StartDescent;


    }

    private void Update()
    {
        //leftDevice = SteamVR_Controller.Input((int)leftTrackedObj.index);
        //rightDevice = SteamVR_Controller.Input((int)rightTrackedObj.index);

        /* Flight controls */
        if (flightControlsActive)
        {
            // Control the wings with the steering angle
            leftLocalPos = leftHand.transform.localPosition;
            rightLocalPos = rightHand.transform.localPosition;
            steeringVector = leftLocalPos - rightLocalPos;
            float steeringAngle = Mathf.Atan2(steeringVector.y, Mathf.Abs(steeringVector.x)) * Mathf.Rad2Deg;
            //Debug.Log("Steering:" + steeringAngle.ToString("F5"));
            leftWing.transform.localEulerAngles = new Vector3(-90f + (angleSensitivity * steeringAngle), -90f, 90f);
            rightWing.transform.localEulerAngles = new Vector3(-90f + (angleSensitivity * steeringAngle), 90f, -90f);
            wings.SetSteeringAngle(angleSensitivity * steeringAngle);
            
            // Control the elevators with forward/back displacement
            centrePosition.x = (leftLocalPos.z + rightLocalPos.z) * 0.5f;
            centrePosition.y = (leftLocalPos.y + rightLocalPos.y) * 0.5f;
            //headPosition.x = playerHead.transform.parent.transform.localPosition.x;
            // headPosition.y = playerHead.transform.parent.transform.localPosition.z;
            headPosition = new Vector2(0.4f, 1.1f);
            //float joystickDisplacment = (centrePosition - headPosition).magnitude - referenceJoystick;
            //float joystickDisplacment = (centrePosition - headPosition).magnitude * (centrePosition.x > headPosition.x ? 1f : -1f);
            float joystickDisplacment = ((leftLocalPos.z + rightLocalPos.z) * 0.5f) - 0.4f;
            elevators.transform.localEulerAngles = new Vector3(-90f + (joystickSensitivity * joystickDisplacment), 90f, -90f);
            wings.SetPitchAngle(joystickSensitivity * joystickDisplacment);

            // Move the wheel and other links
            steeringWheel.transform.localPosition = new Vector3(joystickDisplacment / cos25, 0f, 0f);
            steeringWheel.transform.localEulerAngles = new Vector3(-steeringAngle, 0f, 0f);
            linkController.steeringAngle = -steeringAngle;
            elevatorController.transform.localPosition = new Vector3(joystickDisplacment / cos25, 0f, 0f);

            
        }
        //leftWing.transform.localEulerAngles = new Vector3(heightSensitivity * ( leftHand.transform.localPosition.y - referenceHeight), 0, 0);
        //rightWing.transform.localEulerAngles = new Vector3(heightSensitivity * ( rightHand.transform.localPosition.y - referenceHeight), 0, 0);


        //if (isDescending)
        //{
        //    Descend();
        //}

    }

    public void InitiatePhotoGrab(float x, float y)
    {
        photoManager.InitiatePhotoGrab(x,y);
    }

    public void ToggleCameraLens(float x, float y)
    {
        photoManager.ToggleCameraLens(x,y);
    }

    private void HitUiButton()
    {
        uiInputManager.Click();
        //levelManager.LoadLevel("Main");
    }

    private GameObject elevator;
    private GameObject plane;
    private AudioSource planeAudio;
    private GameObject whiteboard;
    private void GetAnimatedComponents()
    {
        elevator = GameObject.Find("ElevatorPlatform");
        plane = GameObject.Find("PlaneDelivery");
        planeAudio = plane.GetComponent<AudioSource>();
        whiteboard = GameObject.Find("StandingWhiteboard");
    }

    public void StartDescent()
    {
        elevator.GetComponent<Animator>().SetTrigger("startDescent");
        elevator.GetComponent<AudioSource>().Play();
        plane.GetComponent<Animator>().SetTrigger("startReveal");
        whiteboard.GetComponent<Animator>().SetTrigger("startDescent");

        transform.parent.GetComponent<Animator>().SetTrigger("startDescent");
        Invoke("DescentComplete", 20f);
    }

    public void StartLaunch()
    {
        plane.GetComponent<Animator>().SetTrigger("startLaunch");
        whiteboard.GetComponent<Animator>().SetTrigger("startLaunch");
        transform.parent.GetComponent<Animator>().SetTrigger("startLaunch");
        plane.GetComponent<AudioSource>().Play();
        Invoke("LaunchComplete", 20f);
    }

    private void DescentComplete()
    {
        elevator.GetComponent<AudioSource>().Stop();
        flightControlsActive = true;
        //StartLaunch();
    }

    private void LaunchComplete()
    {
        // Change to scene "Main" 
        levelManager.LoadLevel("Main");
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
