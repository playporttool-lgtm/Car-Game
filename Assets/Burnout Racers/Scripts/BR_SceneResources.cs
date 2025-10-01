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
/// Resources of the levels.
/// </summary>
[System.Serializable]
public class BR_SceneResources : ScriptableObject {

    #region singleton
    private static BR_SceneResources instance;
    public static BR_SceneResources Instance { get { if (instance == null) instance = Resources.Load("BR_SceneResources") as BR_SceneResources; return instance; } }
    #endregion

    public GameObject mainMenu;
    public GameObject gameplay;

}
