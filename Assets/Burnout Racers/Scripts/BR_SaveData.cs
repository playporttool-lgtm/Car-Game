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
/// Save data of the player. Can be accessed from BR_SaveGameManager.saveData.
/// </summary>
[System.Serializable]
public class BR_SaveData {

    public string playerName = "PlayerName";
    public int playerMoney = 0;
    public int selectedVehicle = 0;
    public int selectedScene = 1;
    public int selectedGameType = 0;
    public int targetLaps = 3;
    public int racePoints = 0;

    public List<int> ownedVehicles = new List<int>();

    public bool firstGameplay = true;

    public BR_SaveData() { }

}
