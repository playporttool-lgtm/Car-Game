//----------------------------------------------
//			Burnout Racers
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;

/// <summary>
/// Stores all selectable player vehicles here.
/// </summary>
[System.Serializable]
public class BR_PlayerCars : ScriptableObject {

    #region singleton
    private static BR_PlayerCars instance;

    /// <summary>
    /// Singleton instance for accessing the player cars.
    /// </summary>
    public static BR_PlayerCars Instance {
        get {
            if (instance == null)
                instance = Resources.Load("BR_PlayerCars") as BR_PlayerCars;
            return instance;
        }
    }
    #endregion

    /// <summary>
    /// Class for defining selectable player cars with associated price.
    /// </summary>
    [System.Serializable]
    public class SelectablePlayerCars {

        /// <summary>
        /// The GameObject representing the player car.
        /// </summary>
        public GameObject car;

        /// <summary>
        /// The price of the car.
        /// </summary>
        public int price;

    }

    /// <summary>
    /// Array of selectable player cars.
    /// </summary>
    public SelectablePlayerCars[] playerCars;

}
