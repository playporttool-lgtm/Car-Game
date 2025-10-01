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
using UnityEditor.SceneManagement;

public class BR_EditorWindows : Editor {

    #region Help
    [MenuItem("Tools/BoneCracker Games/Burnout Racers/Help", false, 0)]
    public static void Help() {

        EditorUtility.DisplayDialog("Contact", "Please include your invoice number while sending a contact form.", "Close");

        string url = "https://www.bonecrackergames.com/contact/";
        Application.OpenURL(url);

    }

    #endregion Help

    [MenuItem("Tools/BoneCracker Games/Burnout Racers/BR Settings", false, -1000)]
    [MenuItem("GameObject/BoneCracker Games/Burnout Racers/BR Settings", false, -1000)]
    public static void Settings() {

        Selection.activeObject = BR_Settings.Instance;

    }

    [MenuItem("Tools/BoneCracker Games/Burnout Racers/Scene Setups/Add MainMenu Managers", false, -900)]
    [MenuItem("GameObject/BoneCracker Games/Burnout Racers/Scene Setups/Add Mainmenu Managers", false, -900)]
    public static void AddMainMenuManagers() {

        BR_SceneManager.MainMenu();

    }

    [MenuItem("Tools/BoneCracker Games/Burnout Racers/Scene Setups/Add Gameplay Managers", false, -900)]
    [MenuItem("GameObject/BoneCracker Games/Burnout Racers/Scene Setups/Add Gameplay Managers", false, -900)]
    public static void AddGameplayManagers() {

        BR_SceneManager.Gameplay();

    }

    [MenuItem("Tools/BoneCracker Games/Burnout Racers/Player Vehicles", false, -800)]
    [MenuItem("GameObject/BoneCracker Games/Burnout Racers/Player Vehicles", false, -800)]
    public static void OpenPlayerVehiclesList() {

        Selection.activeObject = BR_PlayerCars.Instance;

    }

    [MenuItem("Tools/BoneCracker Games/Burnout Racers/AI Bot Vehicles", false, -800)]
    [MenuItem("GameObject/BoneCracker Games/Burnout Racers/AI Bot Vehicles", false, -800)]
    public static void OpenBotVehiclesList() {

        Selection.activeObject = BR_BotCars.Instance;

    }

    [MenuItem("Tools/BoneCracker Games/Burnout Racers/Enable Mobile Controller", false, -700)]
    [MenuItem("GameObject/BoneCracker Games/Burnout Racers/Enable Mobile Controller", false, -700)]
    public static void EnableMobileController() {

        RCCP_Settings.Instance.mobileControllerEnabled = true;
        EditorUtility.DisplayDialog("Enabled Mobile Controllers", "Mobile controller of the Realistic Car Controller has been enabled.", "Ok");

    }

    [MenuItem("Tools/BoneCracker Games/Burnout Racers/Disable Mobile Controller", false, -700)]
    [MenuItem("GameObject/BoneCracker Games/Burnout Racers/Disable Mobile Controller", false, -700)]
    public static void DisableMobileController() {

        RCCP_Settings.Instance.mobileControllerEnabled = false;
        EditorUtility.DisplayDialog("Disabled Mobile Controllers", "Mobile controller of the Realistic Car Controller has been disabled.", "Ok");

    }

}
