using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovementAndUiManager : MonoBehaviour
{

    [Header("SteamVR Camera Rig Components")]
    public ControllerInputDetector leftHand;
    public ControllerInputDetector rightHand;
    public GameObject playerHead;

    [Header("Teleport Pointing")]
    public GameObject teleportTarget;
    public LineRenderer teleportBeamLine;
    public LayerMask teleportMask;
    public LayerMask uiMask;
    public int maxTeleportRange = 15;

    [Header("Teleport and Movement")]
    public float dashVelocity = 6f;
    public float walkVelocity = 1f;

    // Represents containing player object (i.e. the SteamVR camera rig)
    private GameObject player;

    // Teleport management
    private int maxRangeVertical;
    private Vector3 teleportLocation;
    private float targetHeightAdjust;

    // Teleport dash tracking
    private bool isMoving;
    private Vector3 moveDirection;
    private float totalTravelTime;
    private float elapsedTravelTime = 0f;

    // UI Menu tracking
    private int uiIdx = 0;
    private bool uiMenuShowing = true;
    private bool uiMenuHit = false;
    private Button uiMenuHighlighted = null;
    private bool hasClickedUi = false;  // Used to stop teleporting after clicking a UI button

    private void Start()
    {
        player = gameObject;
        targetHeightAdjust = 0.05f;
        maxRangeVertical = (int)Mathf.Round(maxTeleportRange * 1.2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            DashForward();
            return;
        }
    }


    public void ShowTeleportBeamLeft(float x, float y)
    {
        ShowTeleportBeam(leftHand);
    }

    public void ShowTeleportBeam(ControllerInputDetector hand)
    {

        if (hasClickedUi)
        {
            teleportTarget.gameObject.SetActive(false);
            teleportBeamLine.gameObject.SetActive(false);
            return;
        }
        teleportBeamLine.gameObject.SetActive(true);

        if (uiMenuShowing)
        {
            uiMenuHit = ShowMenuPointer(hand);
        }
        else
        {
            uiMenuHit = false;
        }
        if (!uiMenuHit)
        {
            ShowTeleportPointer(hand);
        }
    }

    public bool ShowMenuPointer(ControllerInputDetector hand)
    {
        RaycastHit hit;
        if (Physics.Raycast(hand.transform.position, hand.transform.forward, out hit, maxTeleportRange, uiMask))
        {
            // Selectable the button to hilight it - or we set colour manually on the image instead of the button
            //hit.transform.GetComponent<Button>().Select();
            teleportTarget.gameObject.SetActive(true);
            teleportBeamLine.startColor = new Color(0f, 0f, 1f);
            teleportBeamLine.endColor = new Color(0f, 0f, 1f);
            teleportBeamLine.SetPosition(0, hand.transform.position);
            teleportBeamLine.SetPosition(1, hit.point);

            teleportTarget.gameObject.SetActive(false);
            ApplyUiHighlight(hit);
            return true;
        }
        RemoveUiHighlight();
        uiMenuHit = false;
        return false;
    }


    public void ClickUiButton(float x, float y)
    {
        if (uiMenuHighlighted != null)
        {
            // Invoke the button click event
            uiMenuHighlighted.onClick.Invoke();
        }
    }

    public void ShowTeleportPointer(ControllerInputDetector hand)
    {
        RaycastHit hit;
        teleportTarget.gameObject.SetActive(true);

        // See if our controller ray hits something
        if (Physics.Raycast(hand.transform.position, hand.transform.forward, out hit, maxTeleportRange, teleportMask))
        {
            teleportLocation = hit.point;
        }
        // If it doesn't, take the point at maximum range and project straight down, see if that hits something
        else
        {
            Vector3 maxPosition = hand.transform.position + (maxTeleportRange * hand.transform.forward);
            RaycastHit maxHit;
            if (Physics.Raycast(maxPosition, Vector3.down, out maxHit, maxRangeVertical, teleportMask))
            {
                teleportLocation = maxHit.point;
            }
            // If nothing valid hit here, no else condition - instead we'll use the transform from last frame. 
        }

        // Draw the line to the teleport target, and move the teleport target. 
        teleportBeamLine.startColor = new Color(1f, 0f, 0f);
        teleportBeamLine.endColor = new Color(1f, 0f, 0f);
        teleportBeamLine.SetPosition(0, hand.transform.position);
        teleportBeamLine.SetPosition(1, teleportLocation);
        teleportTarget.transform.position = teleportLocation + new Vector3(0, targetHeightAdjust, 0);
    }

    public void StartDashing(float x, float y)
    {
        // Releasing the touchpad after clicking a UI button does nothing
        if (hasClickedUi)
        {
            hasClickedUi = false;
            return;
        }
        teleportBeamLine.gameObject.SetActive(false);
        teleportTarget.gameObject.SetActive(false);
        if (!uiMenuHit)
        {
            // Move the player head to be directly above the teleport location, and move the player camera rig to be exactly touching the teleport location ground
            Vector3 teleportVector = new Vector3(teleportLocation.x - playerHead.transform.position.x, teleportLocation.y - player.transform.position.y, teleportLocation.z - playerHead.transform.position.z);
            moveDirection = teleportVector / teleportVector.magnitude;
            totalTravelTime = teleportVector.magnitude / dashVelocity;
            isMoving = true;
        }
        else
        {
            RemoveUiHighlight();
        }
    }

    public void ApplyUiHighlight(RaycastHit hit)
    {
        if (hit.transform.GetComponent<Button>())
        {
            uiMenuHighlighted = hit.transform.GetComponent<Button>();
            uiMenuHighlighted.GetComponent<Image>().color = new Color(0f, 0.8f, 1f);
        }
        else
        {
            RemoveUiHighlight();
        }
    }

    public void RemoveUiHighlight()
    {
        if (uiMenuHighlighted != null)
        {
            uiMenuHighlighted.GetComponent<Image>().color = new Color(1f, 1f, 1f);
            uiMenuHighlighted = null;
        }
    }

    public void DashForward()
    {
        player.transform.position += Time.deltaTime * moveDirection * dashVelocity;
        elapsedTravelTime += Time.deltaTime;
        if (elapsedTravelTime >= totalTravelTime)
        {
            isMoving = false;
            elapsedTravelTime = 0;
        }
    }

    public void WalkForward()
    {
        // Project the viewing direction forward according to the walk velocity and frame time
        Vector3 viewPosition = playerHead.transform.position + (Time.deltaTime * walkVelocity * playerHead.transform.forward);
        RaycastHit currentGroundHit;
        if (Physics.Raycast(playerHead.transform.position, Vector3.down, out currentGroundHit, maxTeleportRange, teleportMask))
        {
            RaycastHit walkHit;
            // See if there is valid ground in front to walk onto
            if (Physics.Raycast(viewPosition, Vector3.down, out walkHit, maxTeleportRange, teleportMask))
            {
                // Adjust the camera rig by the difference
                player.transform.position += walkHit.point - currentGroundHit.point;
            }
        }
    }
}