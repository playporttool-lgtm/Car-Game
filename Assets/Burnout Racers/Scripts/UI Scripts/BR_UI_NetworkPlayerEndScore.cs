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
/// UI component to display the last score of a networked player.
/// </summary>
public class BR_UI_NetworkPlayerEndScore : MonoBehaviour {

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
    /// UI text to display the player's time.
    /// </summary>
    public TextMeshProUGUI playerTime;

    /// <summary>
    /// UI text to display the player's points.
    /// </summary>
    public TextMeshProUGUI playerPoints;

    /// <summary>
    /// Background image for the current player.
    /// </summary>
    public Image youBackground;

#if PHOTON_UNITY_NETWORKING
    private float timer = 1f;
#endif

    private void Update() {

#if !PHOTON_UNITY_NETWORKING

        // If Photon networking is not enabled, deactivate the game object.
        gameObject.SetActive(false);
        return;

#else

        // Increment timer by the time elapsed since the last frame.
        timer += Time.deltaTime;

        // Update the UI elements once every second.
        if (timer >= 1) {

            timer = 0f;

            // Adjust text colors based on whether the background for the current player is enabled.
            if (youBackground.enabled) {

                playerPlace.color = Color.black;
                playerName.color = Color.black;
                playerTime.color = Color.black;
                playerPoints.color = Color.black;

            } else {

                playerPlace.color = Color.white;
                playerName.color = Color.white;
                playerTime.color = Color.white;
                playerPoints.color = Color.white;

            }

            // If the player is not set, deactivate the game object.
            if (!player) {

                gameObject.SetActive(false);
                return;

            }

            // Get the gameplay manager instance.
            BR_GameplayManager gameplayManager = BR_GameplayManager.Instance;

            if (gameplayManager == null)
                return;

            int index = -1;

            // Find the player's position in the list of finished network players.
            if (gameplayManager.allNetworkPlayersFinished != null && gameplayManager.allNetworkPlayersFinished.Count >= 1) {

                for (int i = 0; i < gameplayManager.allNetworkPlayersFinished.Count; i++) {

                    if (Equals(player, gameplayManager.allNetworkPlayersFinished[i]))
                        index = i + 1;

                }

            }

            // Update the sibling index in the hierarchy if the player was found.
            if (index != -1)
                transform.SetSiblingIndex(index);

            // Update the player's position text.
            playerPlace.text = (index).ToString();

            // If the game is not in offline mode, calculate and display race points based on the player's position.
            if (!gameplayManager.offlineMode) {

                if (gameplayManager.gameType == BR_GameplayManager.GameType.Race) {

                    int racePoints = -1;

                    if (player.racePosition == 1)
                        racePoints = 2;
                    else if (player.racePosition == gameplayManager.allRacers.Count - 0)
                        racePoints = -1;
                    else
                        racePoints = 1;

                    playerPoints.text = (racePoints > 0 ? "+ " : "") + racePoints.ToString();

                } else {

                    playerPoints.text = "-";

                }

            } else {

                playerPoints.text = "-";

            }

            // Update the player's name and time.
            playerName.text = player.nickName;
            playerTime.text = BR_TimerToText.TimerText(player.raceTime);

            // Highlight the background if this is the current player's car.
            if (Equals(player, BR_GameplayManager.Instance.currentPlayerCar))
                youBackground.enabled = true;
            else
                youBackground.enabled = false;

        }

#endif

    }

}
