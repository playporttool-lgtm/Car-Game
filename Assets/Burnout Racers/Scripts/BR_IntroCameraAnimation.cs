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
/// Intro camera animation before the countdown. Optional.
/// </summary>
public class BR_IntroCameraAnimation : MonoBehaviour {

    /// <summary>
    /// Animation camera.
    /// </summary>
    public Camera animationCamera;

    /// <summary>
    /// Animation component.
    /// </summary>
    public Animator animationComponent;

    /// <summary>
    /// Animates the camera related to the vehicle position.
    /// </summary>
    /// <param name="state"></param>
    /// <param name="vehiclePosition"></param>
    public void AnimateCamera(bool state, Transform vehiclePosition) {

        animationCamera.gameObject.SetActive(state);

        transform.position = vehiclePosition.position;
        transform.rotation = vehiclePosition.rotation;

        if (state)
            animationComponent.Play(0);
        else
            animationComponent.StopPlayback();

    }

}
