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
/// Stored all resources of the BR here.
/// </summary>
[System.Serializable]
public class BR_Resources : ScriptableObject {

    #region singleton
    private static BR_Resources instance;
    public static BR_Resources Instance { get { if (instance == null) instance = Resources.Load("BR_Resources") as BR_Resources; return instance; } }
    #endregion

    [Header("Demo Scenes")]
    public Object mainMenuScene;
    public Object gameplay1Scene;
    public Object gameplay2Scene;

}
