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
/// UI networked player nametag displayer.
/// </summary>
public class BR_PlayerNameTag : MonoBehaviour {

    /// <summary>
    /// Reference to the player manager script.
    /// </summary>
    private BR_PlayerManager player;

    /// <summary>
    /// Offset for positioning the nametag relative to the player.
    /// </summary>
    public Vector3 offset = new Vector3();

    /// <summary>
    /// CanvasGroup component to control the transparency of the nametag.
    /// </summary>
    public CanvasGroup cGroup;

    /// <summary>
    /// Text component to display the player's name.
    /// </summary>
    public TextMeshProUGUI playerName;

    /// <summary>
    /// Initializes the nametag with the player manager reference.
    /// </summary>
    /// <param name="_player">The player manager to associate with this nametag.</param>
    public void Initialize(BR_PlayerManager _player) {

        player = _player;

    }

    private void LateUpdate() {

        // Return if the player reference is null.
        if (!player)
            return;

        // Update the nametag text to show the player's nickname.
        playerName.text = player.nickName;

        // Calculate the target position on the screen.
        Vector3 targetPos = Camera.main.WorldToScreenPoint(player.transform.position + offset);

        // If the target position is in front of the camera, update the nametag position. Otherwise, move it offscreen.
        if (targetPos.z > 0)
            transform.position = targetPos;
        else
            transform.position = new Vector3(0f, -5000f, 0f);

        // Adjust the transparency of the nametag based on its distance from the camera.
        cGroup.alpha = Mathf.Lerp(5f, 0f, targetPos.z / 50f);

    }

}
