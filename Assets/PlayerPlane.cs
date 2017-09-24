using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPlane : MonoBehaviour {

    public float launchSpeed = 20f;
    public float launchDuration = 1f;
    public GameObject playerHead;

    private Rigidbody rb;
    private bool deployedWings = false;
    private Wings wings;
    private bool isLaunchingPlayer = false;
    private Vector3 launchVelocity;
    private float launchStartTime;


    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        wings = GetComponent<Wings>();

        Camera camera = FindObjectOfType<Camera>();
        float[] distances = new float[32];
        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = 1000;
        }
        distances[9] = 3600;
        distances[10] = 2000;
        camera.layerCullDistances = distances;

        Animator anim = GetComponent<Animator>();
        //anim.dis
        if (SceneManager.GetActiveScene().name == "Main")
            Invoke("Launch", anim.GetCurrentAnimatorClipInfo(0)[0].clip.length + 3f);
        //Launch();
    }

    private void Launch()
    {
        // When we are ready  to luanch
        GetComponent<Animator>().enabled = false;
        launchVelocity = transform.forward * launchSpeed;
        launchStartTime = Time.time;
        isLaunchingPlayer = true;
        rb.isKinematic = false;
    }

    private void Update()
    {
        if (isLaunchingPlayer)
        {
            float ratio = (Time.time - launchStartTime) / launchDuration;
            Vector3 newVelocity = Vector3.Lerp(Vector3.zero, launchVelocity, ratio);
            rb.velocity = newVelocity;
            if (ratio >= 1)
            {
                isLaunchingPlayer = false;
                rb.centerOfMass = new Vector3(playerHead.transform.parent.transform.localPosition.x, 0f, playerHead.transform.parent.transform.localPosition.z);
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate () {
        Vector3 vel = rb.velocity;
        if (!deployedWings)
        {
            if (vel.magnitude > 5f && vel.y < 0.1)
            {
                deployedWings = true;
                wings.setWingsDeployed(true);
                rb.useGravity = false;
            }
            if (vel.magnitude > 5f)
            {
                rb.MoveRotation(Quaternion.LookRotation(vel, Quaternion.Euler(-90f, 0, 0) * vel));
            }
        }
        // Keep velocity constant in case of collisions slowing us down
        else
        {
            rb.velocity = launchSpeed * rb.velocity.normalized;
        }
	}
}
