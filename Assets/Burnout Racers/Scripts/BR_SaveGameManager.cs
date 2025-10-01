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
using System.IO;

/// <summary>
/// Save game manager. Saves, loads, and deletes the save file. Used by BR_API.
/// </summary>
public class BR_SaveGameManager {

    /// <summary>
    /// Save data.
    /// </summary>
    public static BR_SaveData saveData = new BR_SaveData();

    /// <summary>
    /// Save file name.
    /// </summary>
    private const string saveFileName = "BR_SaveData.json";

    /// <summary>
    /// Save.
    /// </summary>
    public static void Save() {

        if (saveData == null)
            saveData = new BR_SaveData();

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(Application.persistentDataPath + "/" + saveFileName, json);

    }

    /// <summary>
    /// Load.
    /// </summary>
    public static void Load() {

        if (saveData == null)
            saveData = new BR_SaveData();

        if (!File.Exists(Application.persistentDataPath + "/" + saveFileName))
            return;

        string json = File.ReadAllText(Application.persistentDataPath + "/" + saveFileName);

        if (!string.IsNullOrEmpty(json))
            saveData = (BR_SaveData)JsonUtility.FromJson(json, typeof(BR_SaveData));

    }

    /// <summary>
    /// Delete save file.
    /// </summary>
    public static void Delete() {

        saveData = new BR_SaveData();

        if (!File.Exists(Application.persistentDataPath + "/" + saveFileName))
            return;

        File.Delete(Application.persistentDataPath + "/" + saveFileName);

    }

}
