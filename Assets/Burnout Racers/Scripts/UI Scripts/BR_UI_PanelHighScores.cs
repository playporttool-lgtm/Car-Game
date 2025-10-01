//----------------------------------------------
//			Burnout Racers
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays high scores for the selected scene on the main menu's scene selection panel.
/// </summary>
public class BR_UI_PanelHighScores : MonoBehaviour {

    /// <summary>
    /// The save name associated with the scene's high scores.
    /// </summary>
    public string saveName = "";

    /// <summary>
    /// Text element for displaying the best lap time.
    /// </summary>
    public TextMeshProUGUI lapText;

    /// <summary>
    /// Text element for displaying the best total time.
    /// </summary>
    public TextMeshProUGUI totalText;

    /// <summary>
    /// Called when the panel is enabled. Updates the displayed best lap and total times.
    /// </summary>
    private void OnEnable() {

        // Set the total time text if a best total time exists; otherwise, display a placeholder.
        if (BR_API.GetBestTimeTotal(saveName) != -1)
            totalText.text = BR_TimerToText.TimerText(BR_API.GetBestTimeTotal(saveName));
        else
            totalText.text = "-:--:--";

        // Set the lap time text if a best lap time exists; otherwise, display a placeholder.
        if (BR_API.GetBestLapTime(saveName) != -1)
            lapText.text = BR_TimerToText.TimerText(BR_API.GetBestLapTime(saveName));
        else
            lapText.text = "-:--:--";

    }

}
