using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UiInputManager : MonoBehaviour {

    public VrButton backButton;
    public VrButton nextButton;
    public VrButton launchBackButton;
    public VrButton launchNextButton;
    private bool hasDescended;

    private PhotoLoader photoLoader;

    private VrButton activeButton = null;

    [Header("UI Controls")]
    public Color disabledColour;
    public Color disabledActiveColour;
    public Color enabledColour;
    public Color enabledActiveColour;
    public Color clickedColour;

    public Text heading;
    public Text body;

    private string[] headings = { "Flying Safari", "Photography Controls", "Enter the Machine", "Flying - Turning", "Flying - Pitching", "Motion Sickness" };
    private string[] bodies = {
        "As an eccentric inventor you have been constructing a flying machine from the plans of Leonardo da Vinci. " +
            "Now that your machine is complete, you can experience the joys of flight while also pursuing your passion for photography." +
            "\n\n" +
            "There are nine different types of animals to find and photograph in the varied environments of the safari park. " +
            "You'll have to explore thoroughly and keep a sharp eye out to find them all before you land!",
        "Press down on the touchpad of the left controller to deploy your head-mounted camera. " +
            "Time will slow to allow you to take the best picture by pointing your head. Press down on the right controller touchpad  to take a photo." +
            "\n\n" +
            "Press the left controller touchpad again to put the camera away. You'll get to see all your photos once you land on the other side of the park.",
        "Explaining how to fly your amazing contraption will be best done from inside the machine itself." +
            "\n\n" +
            "But how do you get out of this room? There's no door! Please stand in the centre of the grey square, and press the GO button to find out what will happen next...",
        "The flying machine is controlled by the wheel in front of you. As the wheel is turned, the flying machine will start to roll. " +
            "The more it the flying machine is rolled, the faster it will turn left or right.\n\n" +
            "When the wheel is level and the arrows line up, the current roll and rate of turn will be maintained. " +
            "To level the flying machine out to horizontal again you will need to correct by turning the other way.",
        "To pitch the flying machine up, pull the wheel towards you. To angle the machine downwards, push the steering wheel away from you. ",
        "The flying controls are sensitive to give you lots of control. Start with very small turns of the wheel until you get the hang of turning. " +
            "\n\n" +
            "Shallow turns will also help reduce your chances of motion sickness." +
            "\n\n" +
            "That's all there is to it - press LAUNCH when you're ready to go!"
    };

    private ControllerInputManager inputManager;

    private int uiState = 0;

    public void Start()
    {
        inputManager = FindObjectOfType<ControllerInputManager>();
        photoLoader = FindObjectOfType<PhotoLoader>();
        if (photoLoader != null)
            photoLoader.OnPhotosLoaded += OnPhotosLoaded;
        SetState(backButton, false);
        SetState(nextButton, true);
        SetState(launchBackButton, true);
        SetState(launchNextButton, true);
        UpdateText();
    }

    public void BtnTriggerEnter(VrButton btn, Collider other)
    {
        activeButton = btn;
    }
    public void BtnTriggerExit(VrButton btn, Collider other)
    {
        if (activeButton == btn)
        {
            activeButton = null;
            EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
        }
    }

    // Called when a click is registered
    public void Click()
    {
        if (activeButton != null)
        {
            activeButton.ClickBtn();
        }
    }

    private void OnPhotosLoaded(int totalPages)
    {
        if (totalPages == 1)
        {
            isLastPage = true;
            nextButton.SetText("Play Again");
        }
    }

    private bool isLastPage;
    public void NextPhotoClick()
    {
        if (isLastPage)
        {
            FindObjectOfType<LevelManager>().LoadLevel("Start");
        } else
        {
            isLastPage = photoLoader.NextPage();
            if (isLastPage)
            {
                nextButton.SetText("Play Again");
            } else
            {
                nextButton.SetText("Next");
            }
        }
        SetState(backButton, true);
    }

    public void BackPhotoClick()
    {
        if (photoLoader.BackPage())
        {
            SetState(backButton, false);
        }
        else
        {
            SetState(backButton, true);
        }
        nextButton.SetText("Next");
        isLastPage = false;
    }

    public void NextButtonClick()
    {
        uiState++;
        HandleUiState();
        
        Debug.Log("Next button clicked");
    }

    public void BackButtonClick()
    {
        uiState--;
        HandleUiState();
        
        Debug.Log("Back button clicked");
    }

    private void HandleUiState()
    {
        if (uiState == 0)
        {
            SetState(backButton, false);
            UpdateText();
        }
        else if (uiState == 1)
        {
            SetState(backButton, true);
            nextButton.SetText("Next");
            UpdateText();
        }
        else if (uiState == 2)
        {
            if (!hasDescended) 
                nextButton.SetText("Go");
            UpdateText();
        }
        else if (uiState == 3)
        {
            if (!hasDescended)
            {
                inputManager.StartDescent();
                hasDescended = true;
                Invoke("UpdateText", 10f);
                nextButton = launchNextButton;
                backButton = launchBackButton;
                nextButton.SetText("Next");
            }
            else
                UpdateText();
        }
        else if (uiState == 4)
        {
            nextButton.SetText("Next");
            UpdateText();
        }
        else if (uiState == 5)
        {
            nextButton.SetText("Launch!");
            UpdateText();
        }
        else if (uiState == 6)
        {
            inputManager.StartLaunch();
            nextButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(false);
        }
    }

    private void UpdateText()
    {
        heading.text = headings[uiState];
        body.text = bodies[uiState];

    }

    private void SetState(VrButton button, bool enabled)
    {
        if (enabled)
        {
            button.SetEnabled(true);
            button.SetColours(enabledColour, enabledActiveColour, clickedColour);
        } else
        {
            button.SetEnabled(false);
            button.SetColours(disabledColour, disabledActiveColour, disabledActiveColour);
        }
    }
}
