//----------------------------------------------
//			Burnout Racers
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Single waypoint for AI.
/// </summary>
public class BR_Waypoint : MonoBehaviour {

    /// <summary>
    /// Target speed for AI. 
    /// </summary>
    [Range(0f, 360f)] public float targetSpeed = 260f;

    /// <summary>
    /// Radius.
    /// </summary>
    [Range(0f, 100f)] public float radius = 30f;

}
