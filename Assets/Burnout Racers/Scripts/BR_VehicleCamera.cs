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
/// Used in the main menu scene to render the player vehicle on the UI canvas. It will track the vehicle location. Must be attached to the camera.
/// </summary>
public class BR_VehicleCamera : MonoBehaviour {

    /// <summary>
    /// Reference to the MainMenuManager. Retrieves the instance if not already set.
    /// </summary>
    public static BR_MainMenuManager MainMenuManager {
        get {
            if (_MainMenuManager == null)
                _MainMenuManager = BR_MainMenuManager.Instance;

            return _MainMenuManager;
        }
    }

    private static BR_MainMenuManager _MainMenuManager;

    /// <summary>
    /// Offset position relative to the player's vehicle.
    /// </summary>
    public Vector3 positionOffset = Vector3.zero;

    /// <summary>
    /// Offset rotation relative to the player's vehicle.
    /// </summary>
    public Quaternion rotationOffset = Quaternion.identity;

    private void LateUpdate() {

        // If MainMenuManager is not available, exit the function.
        if (!MainMenuManager)
            return;

        // Get the current player's vehicle.
        BR_PlayerManager player = MainMenuManager.currentPlayerCar;

        // If the player is not available, exit the function.
        if (!player)
            return;

        // Set the camera's position to the player's vehicle position.
        transform.position = player.transform.position;

        // Adjust the camera's position to keep it at ground level (y = 0).
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

        // Reset the camera's rotation to the identity quaternion (no rotation).
        transform.rotation = Quaternion.identity;

        // Apply the position and rotation offsets to the camera.
        transform.position += positionOffset;
        transform.rotation *= rotationOffset;

    }

}
