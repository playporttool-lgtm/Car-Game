//----------------------------------------------
//			Burnout Racers
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.Collections;
using System.Collections.Generic;

public class BR_PhotonInitLoad {

    [DidReloadScripts]
    private static void OnScriptsReloaded() {

        if (EditorApplication.isCompiling || EditorApplication.isUpdating) {

            EditorApplication.delayCall += OnScriptsReloaded;
            return;

        }

        EditorApplication.delayCall += DelayCall;

    }

    private static void DelayCall() {

#if !PHOTON_UNITY_NETWORKING

        bool choose = EditorUtility.DisplayDialog("Photon PUN2 couldn't found in the project", "Please install latest version of Photon PUN2 to your project. Type in your AppID, and you're ready to go! More info can be found in the documentations. \n \nProject will be compiled successfully, but some game features will be disabled and unable to use. Once you install Photon PUN2, this informer will disappear. \n \nBoth versions of Photon PUN2 (Free and Paid) are supported.", "Open Photon PUN2 Asset Store", "Close");

        if (choose)
            Application.OpenURL(BR_AssetPaths.assetStorePUN2Path);

#endif

    }

}
