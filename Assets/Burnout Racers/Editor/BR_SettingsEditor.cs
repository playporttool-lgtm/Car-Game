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

[CustomEditor(typeof(BR_Settings))]
public class BR_SettingsEditor : Editor {

    BR_Settings asset;

    public bool UseLeaderboard {

        get {

            bool _bool = BR_Settings.Instance.useLeaderboard;
            return _bool;

        }

        set {

            bool _bool = BR_Settings.Instance.useLeaderboard;

            if (_bool == value)
                return;

            BR_Settings.Instance.useLeaderboard = value;
            BR_SetScriptingSymbol.SetEnabled("BCG_BURNOUTRACERS_LEADERBOARD", value);

        }

    }

    public override void OnInspectorGUI() {

        serializedObject.Update();
        asset = (BR_Settings)target;

        DrawDefaultInspector();

        EditorGUILayout.Space();

        UseLeaderboard = EditorGUILayout.Toggle(new GUIContent("Use Leaderboard", "It will enable leaderboard features. Be sure to import the necessary 3rd party asset before enabling it."), UseLeaderboard);

        EditorGUILayout.HelpBox("Leaderboard requires 'Leaderboard Creator' asset developed by Danial Jumagaliyev. You need to import the asset to the project to use it. Don't enable the leaderboard option if your project doesn't have the asset. More info can be found in the documentation. ", MessageType.Info);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Created by Ekrem Bugra Ozdoganlar\nBoneCracker Games", EditorStyles.centeredGreyMiniLabel, GUILayout.MaxHeight(50f));

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
            EditorUtility.SetDirty(asset);

    }

}
