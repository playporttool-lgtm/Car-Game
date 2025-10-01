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
/// Transports the vehicle to the spawn position if it's out of bounds.
/// </summary>
public class BR_CheckOutOfBounds : MonoBehaviour {

    /// <summary>
    /// Called when another collider enters the trigger collider attached to this object.
    /// </summary>
    /// <param name="col">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider col) {

        // Finding the player car by getting the BR_PlayerManager component from the colliding object or its parents.
        BR_PlayerManager player = col.transform.GetComponentInParent<BR_PlayerManager>(true);

        // Transporting the player car back to the first spawn point defined in the gameplay manager.
        RCCP.Transport(player.CarController, BR_GameplayManager.Instance.spawnPoints[0].position, BR_GameplayManager.Instance.spawnPoints[0].rotation);

    }

}
