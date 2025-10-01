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
/// Visual props with rigidbody.
/// </summary>
public class BR_Prop : MonoBehaviour {

    /// <summary>
    /// Reference to the Rigidbody component attached to this prop.
    /// </summary>
    private Rigidbody rigid;

    private void Start() {

        // Initialize the Rigidbody component.
        rigid = GetComponent<Rigidbody>();

        // Put the Rigidbody to sleep to prevent it from being affected by physics until needed.
        rigid.Sleep();

    }

}
