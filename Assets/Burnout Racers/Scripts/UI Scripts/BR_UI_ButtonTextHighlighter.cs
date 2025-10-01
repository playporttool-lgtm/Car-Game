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
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// UI text highlighter when mouse hovers.
/// </summary>
public class BR_UI_ButtonTextHighlighter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    /// <summary>
    /// TextMeshProUGUI component to be highlighted.
    /// </summary>
    public TextMeshProUGUI text;

    /// <summary>
    /// Default text color.
    /// </summary>
    public Color defaultTextColor = Color.white;

    /// <summary>
    /// Color to change to when the mouse hovers over the text.
    /// </summary>
    public Color targetTextColor = Color.black;

    /// <summary>
    /// Whether the mouse is hovering over the text.
    /// </summary>
    public bool hovering = false;

    /// <summary>
    /// Initializes the text color when the object is enabled.
    /// </summary>
    private void OnEnable() {

        hovering = false;
        text.color = defaultTextColor;

    }

    /// <summary>
    /// Resets the text color when the object is disabled.
    /// </summary>
    private void OnDisable() {

        hovering = false;
        text.color = defaultTextColor;

    }

    /// <summary>
    /// Handles the event when the mouse pointer enters the text area.
    /// </summary>
    /// <param name="eventData">Event data associated with the pointer enter event.</param>
    public void OnPointerEnter(PointerEventData eventData) {

        hovering = true;

    }

    /// <summary>
    /// Handles the event when the mouse pointer exits the text area.
    /// </summary>
    /// <param name="eventData">Event data associated with the pointer exit event.</param>
    public void OnPointerExit(PointerEventData eventData) {

        hovering = false;

    }

    /// <summary>
    /// Initializes the text component and default text color.
    /// </summary>
    private void Awake() {

        if (!text)
            text = GetComponentInChildren<TextMeshProUGUI>();

        defaultTextColor = text.color;

    }

    /// <summary>
    /// Updates the text color based on whether the mouse is hovering over it.
    /// </summary>
    private void Update() {

        if (hovering)
            text.color = Color.Lerp(text.color, targetTextColor, Time.deltaTime * 5f);
        else
            text.color = Color.Lerp(text.color, defaultTextColor, Time.deltaTime * 5f);

    }

}
