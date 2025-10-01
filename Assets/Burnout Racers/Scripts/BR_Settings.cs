//----------------------------------------------
//			Burnout Racers
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Stores all general shared BR settings here.
/// </summary>
[System.Serializable]
public class BR_Settings : ScriptableObject {

    #region singleton
    private static BR_Settings instance;

    /// <summary>
    /// Instance of BR_Settings. Loads the settings from the Resources folder if not already loaded.
    /// </summary>
    public static BR_Settings Instance {
        get {
            if (instance == null)
                instance = Resources.Load("BR_Settings") as BR_Settings;

            return instance;
        }
    }
    #endregion

    /// <summary>
    /// Global time scale for the game.
    /// </summary>
    public float timeScale = 1f;

    [Header("Startup Cash")]
    /// <summary>
    /// Initial money given to the player at the start of the game.
    /// </summary>
    public int startMoney = 10000;

    [Header("Soundtracks")]
    /// <summary>
    /// Soundtrack played in the main menu.
    /// </summary>
    public AudioClip mainMenuSoundtrack;

    /// <summary>
    /// Soundtrack played during gameplay.
    /// </summary>
    public AudioClip gameplaySoundtrack;

    [Header("Default Values")]
    /// <summary>
    /// Default draw distance for the camera.
    /// </summary>
    public int defaultDrawDistance = 350;

    /// <summary>
    /// Default volume level for audio.
    /// </summary>
    public float defaultAudioVolume = 1f;

    /// <summary>
    /// Default volume level for music.
    /// </summary>
    public float defaultMusicVolume = 1f;

    /// <summary>
    /// Default setting for post-processing effects.
    /// </summary>
    public bool defaultPP = false;

    /// <summary>
    /// Default setting for shadow rendering.
    /// </summary>
    public bool defaultShadows = false;

    /// <summary>
    /// Determines if the leaderboard functionality should be used. Hidden in the Inspector.
    /// </summary>
    public bool useLeaderboard = false;

}
