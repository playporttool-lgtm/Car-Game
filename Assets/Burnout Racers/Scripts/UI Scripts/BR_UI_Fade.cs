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

/// <summary>
/// UI script for fading the panel on enable.
/// </summary>
public class BR_UI_Fade : MonoBehaviour {

    /// <summary>
    /// CanvasGroup component used for controlling UI transparency.
    /// </summary>
    public CanvasGroup canvasGroup;

    private void Awake() {

        //  If no CanvasGroup is assigned, try to get it from the current GameObject.
        if (!canvasGroup)
            canvasGroup = GetComponent<CanvasGroup>();

    }

    private void OnEnable() {

        //  Start the fade-in effect when the GameObject is enabled.
        StartCoroutine(Fade());

    }

    /// <summary>
    /// Coroutine to handle the fade-in effect.
    /// </summary>
    private IEnumerator Fade() {

        if (canvasGroup)
            canvasGroup.alpha = 0f; //  Start with the canvas being fully transparent.

        float timer = 1f; //  Duration for the fade-in effect.

        while (timer > 0) {

            timer -= Time.unscaledDeltaTime;

            if (canvasGroup)
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1f, Time.unscaledDeltaTime * 8f); //  Gradually increase the alpha to make the UI visible.

            yield return null;

        }

        canvasGroup.alpha = 1f; //  Ensure the canvas is fully visible at the end.

    }

    private void OnDisable() {

        //  Stop all coroutines when the GameObject is disabled.
        StopAllCoroutines();

    }

    private void Reset() {

        //  Reset to reassign the CanvasGroup component.
        canvasGroup = GetComponent<CanvasGroup>();

    }

}
