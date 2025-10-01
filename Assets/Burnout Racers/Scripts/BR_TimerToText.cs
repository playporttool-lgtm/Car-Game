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
/// Float timer to string with proper format.
/// </summary>
public class BR_TimerToText {

    public static string TimerText(float timer) {

        string counter;

        int min;
        int sec;
        int fraction;

        min = (int)(timer / 60f);
        sec = (int)(timer % 60f);
        fraction = (int)((timer * 100) % 100);

        counter = string.Format("{00:00}:{01:00}:{02:00}", min, sec, fraction);

        return counter;

    }

}
