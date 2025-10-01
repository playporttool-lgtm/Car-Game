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

[CustomEditor(typeof(BR_BotCars))]
public class BR_BotCarsEditor : Editor {

    BR_BotCars asset;
    List<BR_BotCars.SelectableAIBotCars> AICars = new List<BR_BotCars.SelectableAIBotCars>();
    Color orgColor;
    Vector2 scrollPos;

    public override void OnInspectorGUI() {

        serializedObject.Update();
        asset = (BR_BotCars)target;
        orgColor = GUI.color;
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
        EditorGUIUtility.labelWidth = 80f;

        GUILayout.Label("Bot Vehicles", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;

        if (asset.AICars != null && asset.AICars.Length >= 1) {

            for (int i = 0; i < asset.AICars.Length; i++) {

                if (asset.AICars[i] != null) {

                    EditorGUILayout.Space();

                    EditorGUILayout.BeginVertical(GUI.skin.box);

                    if (asset.AICars[i].car != null)
                        EditorGUILayout.LabelField(asset.AICars[i].car.name, EditorStyles.boldLabel);

                    EditorGUILayout.Space();

                    asset.AICars[i].car = (GameObject)EditorGUILayout.ObjectField("AI Car Prefab", asset.AICars[i].car, typeof(GameObject), false, GUILayout.MaxWidth(475f));

                    EditorGUILayout.Space();

                    if (asset.AICars[i].car != null && asset.AICars[i].car.GetComponent<RCCP_CarController>() == null)
                        EditorGUILayout.HelpBox("Select A RCCP Based Car", MessageType.Error);

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button("Edit", GUILayout.MaxWidth(50f)))
                        Selection.activeGameObject = asset.AICars[i].car;

                    if (GUILayout.Button("\u2191", GUILayout.MaxWidth(25f)))
                        Up(i);

                    if (GUILayout.Button("\u2193", GUILayout.MaxWidth(25f)))
                        Down(i);

                    GUI.color = Color.red;

                    if (GUILayout.Button("X", GUILayout.MaxWidth(25f)))
                        RemoveCar(i);

                    GUI.color = orgColor;

                    EditorGUILayout.Space();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();

                } else {

                    asset.AICars[i] = new BR_BotCars.SelectableAIBotCars();

                }

            }

        }

        GUI.color = Color.cyan;

        if (GUILayout.Button("Create AI Bot Car"))
            AddNewCar();

        GUI.color = orgColor;

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("botNicknames"), true);

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Created by Ekrem Bugra Ozdoganlar\nBoneCracker Games", EditorStyles.centeredGreyMiniLabel, GUILayout.MaxHeight(50f));

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
            EditorUtility.SetDirty(asset);

    }

    private void AddNewCar() {

        AICars.Clear();

        if (asset.AICars != null && asset.AICars.Length >= 1)
            AICars.AddRange(asset.AICars);

        BR_BotCars.SelectableAIBotCars newCar = new BR_BotCars.SelectableAIBotCars();
        AICars.Add(newCar);
        asset.AICars = AICars.ToArray();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(asset);

    }

    private void RemoveCar(int index) {

        AICars.Clear();

        if (asset.AICars != null && asset.AICars.Length >= 1)
            AICars.AddRange(asset.AICars);

        AICars.RemoveAt(index);
        asset.AICars = AICars.ToArray();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(asset);

    }

    private void Up(int index) {

        if (index <= 0)
            return;

        AICars.Clear();

        if (asset.AICars != null && asset.AICars.Length >= 1)
            AICars.AddRange(asset.AICars);

        BR_BotCars.SelectableAIBotCars currentCar = AICars[index];
        BR_BotCars.SelectableAIBotCars previousCar = AICars[index - 1];

        AICars.RemoveAt(index);
        AICars.RemoveAt(index - 1);

        AICars.Insert(index - 1, currentCar);
        AICars.Insert(index, previousCar);

        asset.AICars = AICars.ToArray();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(asset);

    }

    private void Down(int index) {

        if (index >= asset.AICars.Length - 1)
            return;

        AICars.Clear();

        if (asset.AICars != null && asset.AICars.Length >= 1)
            AICars.AddRange(asset.AICars);

        BR_BotCars.SelectableAIBotCars currentCar = AICars[index];
        BR_BotCars.SelectableAIBotCars nextCar = AICars[index + 1];

        AICars.RemoveAt(index);
        AICars.Insert(index, nextCar);

        AICars.RemoveAt(index + 1);
        AICars.Insert(index + 1, currentCar);

        asset.AICars = AICars.ToArray();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(asset);

    }

}
