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
/// Enables the content when bots are enabled.
/// </summary>
public class BR_UI_EnableWhenBotsActive : MonoBehaviour {

    public GameObject content;

    private void Update() {

        content.SetActive(BR_API.GetBots());

    }

}
