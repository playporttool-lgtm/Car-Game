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
/// Rotates the gameobject with main camera. Used on 3D texts.
/// </summary>
public class BR_TextToCameraRotation : MonoBehaviour {

    private void Update() {

        if (!Camera.main)
            return;

        transform.rotation = Camera.main.transform.rotation;

    }

}
