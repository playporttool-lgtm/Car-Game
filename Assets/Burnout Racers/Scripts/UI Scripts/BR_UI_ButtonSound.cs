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
using UnityEngine.EventSystems;

/// <summary>
/// UI button press sound effect.
/// </summary>
public class BR_UI_ButtonSound : MonoBehaviour, IPointerClickHandler {

    /// <summary>
    /// Audio clip to play when the button is clicked.
    /// </summary>
    public AudioClip clip;

    /// <summary>
    /// Volume of the sound effect.
    /// </summary>
    [Range(0f, 1f)] public float volume = 1f;

    /// <summary>
    /// Called when the button is clicked.
    /// </summary>
    /// <param name="eventData">Event data associated with the pointer click.</param>
    public void OnPointerClick(PointerEventData eventData) {

        // Try to find an existing GameObject named "ButtonSFX".
        GameObject buttonSoundSFXParent = GameObject.Find("ButtonSFX");

        // If it doesn't exist, create a new GameObject named "ButtonSFX".
        if (!buttonSoundSFXParent)
            buttonSoundSFXParent = new GameObject("ButtonSFX");

        // Create and play the audio source for the button click sound.
        BR_CreateAudioSource.NewAudioSource(buttonSoundSFXParent, clip.name, 0f, 0f, volume, clip, false, true, true);

    }

}
