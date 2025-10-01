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
/// Stores all AI vehicles and related data.
/// </summary>
[System.Serializable]
public class BR_BotCars : ScriptableObject {

    #region singleton
    /// <summary>
    /// Singleton instance of BR_BotCars.
    /// </summary>
    private static BR_BotCars instance;

    /// <summary>
    /// Provides access to the singleton instance of BR_BotCars.
    /// Loads the instance from the Resources folder if it hasn't been loaded yet.
    /// </summary>
    public static BR_BotCars Instance {
        get {
            if (instance == null)
                instance = Resources.Load("BR_BotCars") as BR_BotCars;
            return instance;
        }
    }
    #endregion

    /// <summary>
    /// A class representing a selectable AI bot car.
    /// </summary>
    [System.Serializable]
    public class SelectableAIBotCars {

        /// <summary>
        /// The GameObject representing the AI bot car.
        /// </summary>
        public GameObject car;

    }

    /// <summary>
    /// Array of selectable AI bot cars.
    /// </summary>
    public SelectableAIBotCars[] AICars;

    /// <summary>
    /// Array of nicknames for the AI bots.
    /// </summary>
    public string[] botNicknames;

}
