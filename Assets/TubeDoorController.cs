using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeDoorController : MonoBehaviour
{

    public float triggerDuration = 1.5f;
    private float startTime = -1f;
    private bool triggered = false;
    private GameObject player;
    private Animator anim;
    private RuntimeAnimatorController animController;
    private Vector3 delta;
    private Quaternion angleDelta;
    private bool isMovingPlayer = false;
    private AudioSource audio;


    private void Start()
    {
        player = FindObjectOfType<SteamVR_ControllerManager>().gameObject;
        anim = GetComponent<Animator>();
        animController = anim.runtimeAnimatorController;
        audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (anim.IsInTransition(0))
        {
            AnimatorStateInfo stateInfo = anim.GetNextAnimatorStateInfo(0);
            Debug.Log(stateInfo);
        }
        // When we start sliding, move the player with the tube door
        if (anim.IsInTransition(0) && anim.GetNextAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("TubeSlide"))
        {
            delta = player.transform.position - transform.position;
            angleDelta = Quaternion.Inverse(transform.rotation) * player.transform.rotation;
            isMovingPlayer = true;
        }
        if(anim.IsInTransition(0) && anim.GetNextAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("TubeLaunchReady"))
        {
            audio.Play();
            isMovingPlayer = false;
        }
        // Once we finish sliding, uncouple the player
        if (anim.IsInTransition(0) && anim.GetNextAnimatorStateInfo(0).IsName("Base.Idle"))
        {
            player.transform.SetParent(null);
        }
        if (isMovingPlayer)
        {
            player.transform.position = transform.position + delta;
            player.transform.rotation = transform.rotation * angleDelta;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered)
        {
            if (other.GetComponent<Camera>())
            {
                startTime = Time.time;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (startTime < 0 || triggered) return; // Catch condition in case trigger enter doesn't work properly
        float elapsed = Time.time - startTime;
        if (elapsed > triggerDuration)
        {
            triggered = true;
            anim.SetTrigger("StartRotate");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        startTime = -1f;
    }





}
