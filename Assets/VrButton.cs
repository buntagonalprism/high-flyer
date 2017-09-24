using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class VrButton : MonoBehaviour {

    [HideInInspector]
    private Button btn;
    [HideInInspector]
    private Image img;

    private bool btnEnabled = true;

    //public delegate void HighlightHandler(VrButton btn, Collider collider);
    //public event HighlightHandler TriggerEnter;
    //public event HighlightHandler TriggerExit;

    private UiInputManager uiInputManager;


    public void SetColours(Color baseColour, Color activeColour, Color clickedColour)
    {
        ColorBlock cb = btn.colors;
        cb.normalColor = baseColour;
        cb.highlightedColor = activeColour;
        cb.pressedColor = clickedColour;
        btn.colors = cb;
    }

    public void SetEnabled(bool enabled)
    {
        btnEnabled = enabled;
    }

    public void SetText(string text)
    {
        btn.GetComponentInChildren<Text>().text = text;
    }

    public void ClickBtn()
    {
        var pointer = new PointerEventData(EventSystem.current);
        if (btnEnabled)
            //btn.Invoke("Click", 0.001f);
            //btn.onClick.Invoke();
            ExecuteEvents.Execute(btn.gameObject, pointer, ExecuteEvents.submitHandler);
    }

    private void Start()
    {
        btn = GetComponent<Button>();
        img = GetComponent<Image>();
        uiInputManager = FindObjectOfType<UiInputManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        btn.Select();
        uiInputManager.BtnTriggerEnter(this, other);
    }

    private void OnTriggerExit(Collider other)
    {
        uiInputManager.BtnTriggerExit(this, other);
    }
}
