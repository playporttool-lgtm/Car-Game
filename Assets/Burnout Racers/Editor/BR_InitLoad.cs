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
using System.IO;

[InitializeOnLoad]
public class BR_InitLoad {

    static BR_InitLoad() {

        EditorApplication.delayCall += EditorDelayedUpdate;

    }

    static void EditorDelayedUpdate() {

        //BR_SetScriptingSymbol.SetEnabled("PHOTON_UNITY_NETWORKING", false);
        //BR_SetScriptingSymbol.SetEnabled("PUN_2_0_OR_NEWER", false);
        //BR_SetScriptingSymbol.SetEnabled("PUN_2_OR_NEWER", false);
        //BR_SetScriptingSymbol.SetEnabled("PUN_2_19_OR_NEWER", false);
        //BR_SetScriptingSymbol.SetEnabled("AMPLIFY_SHADER_EDITOR", false);
        //BR_SetScriptingSymbol.SetEnabled("BCG_BURNOUTRACERS", false);
        //BR_SetScriptingSymbol.SetEnabled("BCG_BURNOUTDRIFT", false);
        //BR_SetScriptingSymbol.SetEnabled("EASYROADS3D_PRO", false);

#if !BCG_BURNOUTRACERS

        BR_SetScriptingSymbol.SetEnabled("BCG_BURNOUTRACERS", true);
        EditorApplication.ExecuteMenuItem("Window/TextMeshPro/Import TMP Essential Resources");

        EditorUtility.DisplayDialog("Regards from BoneCracker Games", "Thank you for purchasing and using Burnout Racers. Please read the documentation before use. Also check out the online documentation for updated info. Have fun :)", "Let's get started!");
        EditorUtility.DisplayDialog("Don't Change Directory / Name", "Please don't change directory or name of the asset. Some assets are using direct asset paths, therefore asset root should be 'Assets/Burnout Racers/'", "Ok");
        EditorUtility.DisplayDialog("New Input System", "Burnout Racers is using new input system. Legacy input system is deprecated. Make sure your project has Input System installed through the Package Manager. Import screen will ask you to install dependencies, choose Yes.", "Ok");

        BR_WelcomeWindow.OpenWindow();
        Selection.activeObject = BR_Settings.Instance;

        EditorUtility.DisplayDialog("Restart Unity", "Please restart Unity after importing the package. Otherwise inputs may not work for the first time.", "Ok");

#endif

        BR_Installation.Check();

    }

}
