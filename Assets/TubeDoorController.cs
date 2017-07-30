using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeDoorController : MonoBehaviour
{

    public float triggerDuration = 1.5f;
    public float launchSpeed = 20f;
    public float launchDuration = 1f;
    public GameObject playerHead;

    private float startTime = -1f;
    private bool triggered = false;
    private GameObject player;
    private Rigidbody playerRb;

    private Animator anim;
    private RuntimeAnimatorController animController;
    private Vector3 delta;
    private Quaternion angleDelta;
    private bool isMovingPlayer = false;

    private float launchStartTime = 0;
    private Vector3 launchVelocity;
    private bool isLaunchingPlayer = false;
    private AudioSource audio;

    private void Start()
    {
        player = FindObjectOfType<PlayerPlane>().gameObject;
        playerRb = player.GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        animController = anim.runtimeAnimatorController;
        audio = GetComponent<AudioSource>();
    }

    private void Update()
    {

        // When we start sliding, move the player with the tube door
        if (anim.IsInTransition(0) && anim.GetNextAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("TubeSlide"))
        {
            delta = player.transform.position - transform.position;
            angleDelta = Quaternion.Inverse(transform.rotation) * player.transform.rotation;
            isMovingPlayer = true;
            //audio.Play();
            
        }
        // When we are ready  to luanch
        if(anim.IsInTransition(0) && anim.GetNextAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("TubeLaunchReady"))
        {
            isMovingPlayer = false;
            launchVelocity = player.transform.forward * launchSpeed;
            launchStartTime = Time.time;
            isLaunchingPlayer = true;
        }


        if (isMovingPlayer)
        {
            player.transform.position = transform.position + delta;
            player.transform.rotation = transform.rotation * angleDelta;
        }

        if (isLaunchingPlayer)
        {
            float ratio = (Time.time - launchStartTime) / launchDuration;
            Vector3 newVelocity = Vector3.Lerp(Vector3.zero, launchVelocity, ratio);
            playerRb.velocity = newVelocity;
            if (ratio >= 1)
            {
                isLaunchingPlayer = false;
                playerRb.centerOfMass = new Vector3(playerHead.transform.parent.transform.localPosition.x, 0f, playerHead.transform.parent.transform.localPosition.z);
                playerRb.isKinematic = false;
                playerRb.useGravity = true;
            }
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
