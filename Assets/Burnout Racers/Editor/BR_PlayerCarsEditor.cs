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

[CustomEditor(typeof(BR_PlayerCars))]
public class BR_PlayerCarsEditor : Editor {

    BR_PlayerCars asset;
    List<BR_PlayerCars.SelectablePlayerCars> playerCars = new List<BR_PlayerCars.SelectablePlayerCars>();
    Color orgColor;
    Vector2 scrollPos;

    public override void OnInspectorGUI() {

        serializedObject.Update();
        asset = (BR_PlayerCars)target;
        orgColor = GUI.color;
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
        EditorGUIUtility.labelWidth = 80f;

        GUILayout.Label("Player Cars", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;

        if (asset.playerCars != null && asset.playerCars.Length >= 1) {

            for (int i = 0; i < asset.playerCars.Length; i++) {

                if (asset.playerCars[i] != null) {

                    EditorGUILayout.Space();

                    EditorGUILayout.BeginVertical(GUI.skin.box);

                    if (asset.playerCars[i].car != null)
                        EditorGUILayout.LabelField(asset.playerCars[i].car.name, EditorStyles.boldLabel);

                    EditorGUILayout.Space();

                    asset.playerCars[i].car = (GameObject)EditorGUILayout.ObjectField("Player Car Prefab", asset.playerCars[i].car, typeof(GameObject), false, GUILayout.MaxWidth(475f));

                    EditorGUILayout.Space();

                    if (asset.playerCars[i].car != null && asset.playerCars[i].car.GetComponent<RCCP_CarController>() == null)
                        EditorGUILayout.HelpBox("Select A RCCP Based Car", MessageType.Error);

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();

                    asset.playerCars[i].price = EditorGUILayout.IntField("Price", asset.playerCars[i].price, GUILayout.MaxWidth(150f));

                    if (GUILayout.Button("Edit", GUILayout.MaxWidth(50f)))
                        Selection.activeGameObject = asset.playerCars[i].car;

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

                    asset.playerCars[i] = new BR_PlayerCars.SelectablePlayerCars();

                }

            }

        }

        GUI.color = Color.cyan;

        if (GUILayout.Button("Create Player Car"))
            AddNewCar();

        GUI.color = orgColor;

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Created by Ekrem Bugra Ozdoganlar\nBoneCracker Games", EditorStyles.centeredGreyMiniLabel, GUILayout.MaxHeight(50f));

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
            EditorUtility.SetDirty(asset);

    }

    private void AddNewCar() {

        playerCars.Clear();
        playerCars.AddRange(asset.playerCars);
        BR_PlayerCars.SelectablePlayerCars newCar = new BR_PlayerCars.SelectablePlayerCars();
        playerCars.Add(newCar);
        asset.playerCars = playerCars.ToArray();
        BR_API.SetVehicle(0);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(asset);

    }

    private void RemoveCar(int index) {

        playerCars.Clear();
        playerCars.AddRange(asset.playerCars);
        playerCars.RemoveAt(index);
        asset.playerCars = playerCars.ToArray();
        BR_API.SetVehicle(0);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(asset);

    }

    private void Up(int index) {

        if (index <= 0)
            return;

        playerCars.Clear();
        playerCars.AddRange(asset.playerCars);

        BR_PlayerCars.SelectablePlayerCars currentCar = playerCars[index];
        BR_PlayerCars.SelectablePlayerCars previousCar = playerCars[index - 1];

        playerCars.RemoveAt(index);
        playerCars.RemoveAt(index - 1);

        playerCars.Insert(index - 1, currentCar);
        playerCars.Insert(index, previousCar);

        asset.playerCars = playerCars.ToArray();
        BR_API.SetVehicle(0);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(asset);

    }

    private void Down(int index) {

        if (index >= asset.playerCars.Length - 1)
            return;

        playerCars.Clear();
        playerCars.AddRange(asset.playerCars);

        BR_PlayerCars.SelectablePlayerCars currentCar = playerCars[index];
        BR_PlayerCars.SelectablePlayerCars nextCar = playerCars[index + 1];

        playerCars.RemoveAt(index);
        playerCars.Insert(index, nextCar);

        playerCars.RemoveAt(index + 1);
        playerCars.Insert(index + 1, currentCar);

        asset.playerCars = playerCars.ToArray();
        BR_API.SetVehicle(0);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(asset);

    }

}
