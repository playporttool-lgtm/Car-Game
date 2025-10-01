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
/// Pause game button to be used in the gameplay scene.
/// </summary>
public class BR_UI_PauseButton : MonoBehaviour {

    public void OnClick() {

        if (!BR_GameplayManager.Instance)
            return;

        BR_GameplayManager.Instance.Pause();

    }

}
