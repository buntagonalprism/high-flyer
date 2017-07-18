using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Captures input from HTC Vive Controllers and exposes event actions for them
/// </summary>
public class ControllerInputDetector : MonoBehaviour
{

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;

    public event Action OnTriggerPressDown;
    public event Action OnTriggerPress;
    public event Action OnTriggerPressUp;
    public event Action OnGripPress;
    public delegate void TouchpadHandler(float x, float y);
    public event TouchpadHandler OnTouchpadDown;
    public event TouchpadHandler OnTouchpad;
    public event TouchpadHandler OnTouchpadUp;
    public event TouchpadHandler OnTouchpadPressDown;
    public event TouchpadHandler OnTouchpadPressUp;
    public delegate void TriggerHandler(Collider collider);
    public event TriggerHandler OnColliderTriggerStay;

    //public delegate 

    // Use this for initialization
    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Update is called once per frame
    void Update()
    {
        device = SteamVR_Controller.Input((int)trackedObj.index);
        // Trigger button
        if (IsTriggerDown())
        {
            if (OnTriggerPressDown != null)
                OnTriggerPressDown();
        }
        else if (IsTriggerHeld())
        {
            if (OnTriggerPress != null)
                OnTriggerPress();
        }
        else if (IsTriggerUp())
        {
            if (OnTriggerPressUp != null)
                OnTriggerPressUp();
        }

        // Grip button
        if (device.GetPress(SteamVR_Controller.ButtonMask.Grip))
        {
            if (OnGripPress != null) 
                OnGripPress();
        }

        // Touchpad touch
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            if (OnTouchpadDown != null) {
                OnTouchpadDown(device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x, device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).y);
            }
        }
        else if (device.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
        {
            if (OnTouchpad != null)
                OnTouchpad(device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x, device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).y);
        }
        else if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            if (OnTouchpadUp != null)
                OnTouchpadUp(device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x, device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).y);
        }

        // Touchpad press
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            if (OnTouchpadPressDown != null)
                OnTouchpadPressDown(device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x, device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).y);
        }
        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            if (OnTouchpadPressUp != null)
                OnTouchpadPressUp(device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x, device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).y);
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (OnColliderTriggerStay != null)
            OnColliderTriggerStay(collider);
    }

    public bool IsTriggerHeld()
    {
        device = SteamVR_Controller.Input((int)trackedObj.index);
        return device.GetPress(SteamVR_Controller.ButtonMask.Trigger);
    }

    public bool IsTriggerUp()
    {
        device = SteamVR_Controller.Input((int)trackedObj.index);
        return device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger);
    }

    public bool IsTriggerDown()
    {
        device = SteamVR_Controller.Input((int)trackedObj.index);
        return device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger);
    }

    public void HapticPulse(ushort intensity)
    {
        device = SteamVR_Controller.Input((int)trackedObj.index);
        device.TriggerHapticPulse(intensity);
    }

    public Vector3 GetVelocity()
    {
        device = SteamVR_Controller.Input((int)trackedObj.index);
        return device.velocity;
    }

    public Vector3 GetAngularVelocity()
    {
        device = SteamVR_Controller.Input((int)trackedObj.index);
        return device.angularVelocity;
    }

}
