//----------------------------------------------
//			Burnout Racers
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// UI Button click invoker with the old input system.
/// </summary>
public class BR_UI_KeyEnter : MonoBehaviour {

    /// <summary>
    /// Name of the input mapped to the button click action.
    /// </summary>
    public string inputName = "Submit";

    /// <summary>
    /// Button component to invoke the click action.
    /// </summary>
    private Button button;

    private void Start() {

        //  Get the Button component attached to this GameObject.
        button = GetComponent<Button>();

    }

    private void Update() {

        //  Ensure the button component is assigned.
        if (!button)
            button = GetComponent<Button>();

        //  If button component is still not found, disable this script.
        if (!button) {

            Debug.LogError("Button not found on this " + transform.name + ". Disabling it.");
            enabled = false;
            return;

        }

        //  If input name is not set, do nothing.
        if (inputName == "")
            return;

        //  If the input button is pressed, invoke the button's onClick event.
        if (Input.GetButtonDown(inputName))
            button.onClick.Invoke();

    }

}
