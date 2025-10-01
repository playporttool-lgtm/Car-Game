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
using System.Collections.Generic;

/// <summary>
/// Receives float from UI Slider, and displays the value as a text.
/// </summary>
public class BR_UI_SliderTextReader : MonoBehaviour {

    public Slider slider;       //  UI Slider.
    public Text text;       //  UI Text.

    private void Awake() {

        if (!slider)
            slider = GetComponentInParent<Slider>();

        if (!text)
            text = GetComponentInChildren<Text>();

    }

    private void Update() {

        if (!slider || !text)
            return;

        text.text = slider.value.ToString("F1");

    }

}
