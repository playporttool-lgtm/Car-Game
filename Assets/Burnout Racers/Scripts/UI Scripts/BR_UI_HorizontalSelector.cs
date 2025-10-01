//----------------------------------------------
//			Burnout Racers
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// UI horizontal selector.
/// </summary>
public class BR_UI_HorizontalSelector : MonoBehaviour {

    /// <summary>
    /// Text component to display the current selection.
    /// </summary>
    public TextMeshProUGUI selectorText;

    /// <summary>
    /// Current selected index.
    /// </summary>
    public int value = 0;

    /// <summary>
    /// Array of texts that can be selected.
    /// </summary>
    public string[] texts;

    /// <summary>
    /// Event triggered when the selector's value is updated.
    /// </summary>
    public UnityEvent<BR_UI_HorizontalSelector> onUpdate = new UnityEvent<BR_UI_HorizontalSelector>();

    private void Update() {

        //  Update the selector text to match the current value.
        selectorText.text = texts[value];

    }

    /// <summary>
    /// Move to the next value in the selector.
    /// </summary>
    public void Next() {

        value++;

        //  Wrap around if the value exceeds the array length.
        if (value >= texts.Length)
            value = 0;

        //  Trigger the onUpdate event.
        onUpdate.Invoke(this);

    }

    /// <summary>
    /// Move to the previous value in the selector.
    /// </summary>
    public void Prev() {

        value--;

        //  Wrap around if the value is below 0.
        if (value < 0)
            value = texts.Length - 1;

        //  Trigger the onUpdate event.
        onUpdate.Invoke(this);

    }

    /// <summary>
    /// Update the value of this selector based on another selector.
    /// </summary>
    /// <param name="otherSelector">The other selector to match the value with.</param>
    public void UpdateValue(BR_UI_HorizontalSelector otherSelector) {

        value = otherSelector.value;

    }

}
