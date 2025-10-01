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
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Leaderboard content item representing a player.
/// </summary>
public class BR_UI_LeaderboardItem : MonoBehaviour {

    /// <summary>
    /// UI text element to display the player's place.
    /// </summary>
    public TextMeshProUGUI placeText;

    /// <summary>
    /// UI text element to display the player's name.
    /// </summary>
    public TextMeshProUGUI nameText;

    /// <summary>
    /// UI text element to display the player's points.
    /// </summary>
    public TextMeshProUGUI pointsText;

    /// <summary>
    /// GameObject indicating if this item represents the current player.
    /// </summary>
    public GameObject youIndication;

    /// <summary>
    /// Sets the leaderboard item with the provided data.
    /// </summary>
    /// <param name="place">The player's place on the leaderboard.</param>
    /// <param name="name">The player's name.</param>
    /// <param name="points">The player's points.</param>
    /// <param name="isMine">Whether this item represents the current player.</param>
    public void Set(int place, string name, int points, bool isMine) {

        //  Set the player's name.
        nameText.text = name;

        //  Set the player's points formatted as "points pnts".
        pointsText.text = points.ToString("F0") + " pnts";

        //  Set the place text with the appropriate suffix.
        if (place == 1)
            placeText.text = place.ToString() + "st";
        else if (place == 2)
            placeText.text = place.ToString() + "nd";
        else if (place == 3)
            placeText.text = place.ToString() + "rd";
        else
            placeText.text = place.ToString() + "th";

        //  If the youIndication GameObject is assigned, activate it if this is the current player's item.
        if (youIndication)
            youIndication.SetActive(isMine);

    }

}
