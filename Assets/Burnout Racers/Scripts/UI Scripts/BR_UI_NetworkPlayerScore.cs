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
#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
#endif
using TMPro;

/// <summary>
/// UI component to display the score of a networked player.
/// </summary>
public class BR_UI_NetworkPlayerScore : MonoBehaviour {

    /// <summary>
    /// Reference to the player's manager.
    /// </summary>
    public BR_PlayerManager player;

    /// <summary>
    /// UI text to display the player's position.
    /// </summary>
    public TextMeshProUGUI playerPlace;

    /// <summary>
    /// UI text to display the player's name.
    /// </summary>
    public TextMeshProUGUI playerName;

    /// <summary>
    /// Background image for the player's score display.
    /// </summary>
    public Image background;

    /// <summary>
    /// Background image for the current player.
    /// </summary>
    public Image youBackground;

#if PHOTON_UNITY_NETWORKING
    private float timeout = 0f;
#endif

    private void Update() {

#if !PHOTON_UNITY_NETWORKING

        // If Photon networking is not enabled, deactivate the game object.
        gameObject.SetActive(false);
        return;

#else

        // Adjust text colors based on whether the background for the current player is enabled.
        if (youBackground.enabled) {

            playerPlace.color = Color.black;
            playerName.color = Color.black;

        } else {

            playerPlace.color = Color.white;
            playerName.color = Color.white;

        }

        // If the player is not set, start a timeout counter and deactivate the game object after 3 seconds.
        if (!player) {

            timeout += Time.deltaTime;
            background.color = Color.red;
            youBackground.enabled = false;

            if (timeout > 3f)
                gameObject.SetActive(false);

            return;

        }

        // Reset the timeout if the player is present.
        timeout = 0f;

        // Update the player's name.
        playerName.text = player.nickName;

        // Return if the gameplay manager is not found.
        if (BR_GameplayManager.Instance == null)
            return;

        // Get the player's race position.
        int index = player.RacePositioner.racePosition;

        // Set the sibling index in the hierarchy based on the player's position.
        transform.SetSiblingIndex(index);
        playerPlace.text = (index).ToString();

        // Highlight the background if this is the current player's car.
        if (Equals(player, BR_GameplayManager.Instance.currentPlayerCar))
            youBackground.enabled = true;
        else
            youBackground.enabled = false;

#endif

    }

}
