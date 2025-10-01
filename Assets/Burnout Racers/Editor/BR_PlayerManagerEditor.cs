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
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using System.Linq;

[CustomEditor(typeof(BR_PlayerManager))]
public class BR_PlayerManagerEditor : Editor {

    BR_PlayerManager prop;
    static bool showInfo = false;

    public override void OnInspectorGUI() {

        prop = (BR_PlayerManager)target;
        serializedObject.Update();

        showInfo = EditorGUILayout.Toggle("Show Info", showInfo);

        GUI.enabled = false;

        if (showInfo)
            DrawDefaultInspector();

        GUI.enabled = true;

        bool isPersistent = EditorUtility.IsPersistent(prop.gameObject);

        if (isPersistent)
            GUI.enabled = false;

        if (!Application.isPlaying) {

            if (PrefabUtility.GetCorrespondingObjectFromOriginalSource(prop.gameObject) == null) {

                EditorGUILayout.HelpBox("You'll need to create a new prefab for the vehicle first.", MessageType.Info);
                Color defColor = GUI.color;
                GUI.color = Color.red;

                if (GUILayout.Button("Create Prefab"))
                    CreatePrefab();

                GUI.color = defColor;

            } else {

                EditorGUILayout.HelpBox("Don't forget to save changes.", MessageType.Info);
                Color defColor = GUI.color;
                GUI.color = Color.green;

                if (GUILayout.Button("Save Prefab"))
                    SavePrefab();

                GUI.color = defColor;

            }

            bool foundPrefab = false;

            for (int i = 0; i < BR_PlayerCars.Instance.playerCars.Length; i++) {

                if (BR_PlayerCars.Instance.playerCars[i].car != null) {

                    if (prop.transform.name == BR_PlayerCars.Instance.playerCars[i].car.transform.name) {

                        foundPrefab = true;
                        break;

                    }

                }

            }

            if (!foundPrefab) {

                EditorGUILayout.HelpBox("Player vehicles list doesn't include this vehicle yet!", MessageType.Info);
                Color defColor = GUI.color;
                GUI.color = Color.green;

                if (GUILayout.Button("Add Prefab To Player Vehicles List")) {

                    if (PrefabUtility.GetCorrespondingObjectFromOriginalSource(prop.gameObject) == null)
                        CreatePrefab();
                    else
                        SavePrefab();

                    AddToList();

                }

                GUI.color = defColor;

            }

            GUI.enabled = true;

#if PHOTON_UNITY_NETWORKING

            if (prop.gameObject.GetComponent<Photon.Pun.PhotonView>() == null) {

                if (GUILayout.Button("PhotonView is missing. Add PhotonView"))
                    prop.gameObject.AddComponent<Photon.Pun.PhotonView>();

            }

#endif

        }

        EditorGUILayout.BeginVertical(GUI.skin.box);

        EditorGUILayout.LabelField("Quick Actions", EditorStyles.centeredGreyMiniLabel);

        RCCP_CarController carController = prop.GetComponentInParent<RCCP_CarController>(true);
        RCCP_Engine engine = carController.GetComponentInChildren<RCCP_Engine>(true);
        RCCP_Stability handling = carController.GetComponentInChildren<RCCP_Stability>(true);
        RCCP_Axle[] axles = carController.GetComponentsInChildren<RCCP_Axle>(true);
        RCCP_Nos nos = carController.GetComponentInChildren<RCCP_Nos>(true);

        if (engine)
            engine.maximumTorqueAsNM = EditorGUILayout.Slider(new GUIContent("Engine Torque"), engine.maximumTorqueAsNM, 300f, 1200f);

        if (handling) {

            handling.steerHelperStrength = EditorGUILayout.Slider(new GUIContent("Handling"), handling.steerHelperStrength, .1f, 1f);
            handling.tractionHelperStrength = handling.steerHelperStrength;

        }

        if (axles != null && axles.Length > 0) {

            float averageBrakeTorque = 0f;

            for (int i = 0; i < axles.Length; i++) {

                if (axles[i] != null)
                    averageBrakeTorque += axles[i].maxBrakeTorque;

            }

            averageBrakeTorque /= (float)Mathf.Clamp(axles.Length, 1, 10);

            averageBrakeTorque = EditorGUILayout.Slider(new GUIContent("Brake Torque"), averageBrakeTorque, 1000f, 10000f);

            for (int i = 0; i < axles.Length; i++) {

                if (axles[i] != null)
                    axles[i].maxBrakeTorque = averageBrakeTorque;

            }

        }

        if (engine != null)
            engine.maximumSpeed = EditorGUILayout.Slider(new GUIContent("Speed"), (int)engine.maximumSpeed, 160f, 400f);

        EditorGUILayout.Separator();

        RCCP_VehicleUpgrade_Engine upgrade_Engine = carController.GetComponentInChildren<RCCP_VehicleUpgrade_Engine>(true);
        RCCP_VehicleUpgrade_Handling upgrade_Handling = carController.GetComponentInChildren<RCCP_VehicleUpgrade_Handling>(true);
        RCCP_VehicleUpgrade_Brake upgrade_Brake = carController.GetComponentInChildren<RCCP_VehicleUpgrade_Brake>(true);
        RCCP_VehicleUpgrade_Speed upgrade_Speed = carController.GetComponentInChildren<RCCP_VehicleUpgrade_Speed>(true);

        if (upgrade_Engine)
            upgrade_Engine.efficiency = EditorGUILayout.Slider(new GUIContent("Engine Upgrade Efficiency"), upgrade_Engine.efficiency, 1f, 2f);

        if (upgrade_Handling)
            upgrade_Handling.efficiency = EditorGUILayout.Slider(new GUIContent("Handling Upgrade Efficiency"), upgrade_Handling.efficiency, 1f, 2f);

        if (upgrade_Brake)
            upgrade_Brake.efficiency = EditorGUILayout.Slider(new GUIContent("Brake Upgrade Efficiency"), upgrade_Brake.efficiency, 1f, 2f);

        if (upgrade_Speed)
            upgrade_Speed.efficiency = EditorGUILayout.Slider(new GUIContent("Speed Upgrade Efficiency"), upgrade_Speed.efficiency, 1f, 2f);

        EditorGUILayout.Separator();

        Color orgGuiColor = GUI.color;

        if (carController) {
            GUI.color = Color.green; EditorGUILayout.LabelField("Car Controller Status: " + "Equipped, check the RCCP_CarController component.");
        } else {
            GUI.color = Color.red; EditorGUILayout.LabelField("Car Controller Status: " + "Not Equipped, Not OK.");
        }

        if (engine) {
            GUI.color = Color.green; EditorGUILayout.LabelField("Engine Status: " + "Equipped, OK");
        } else {
            GUI.color = Color.red; EditorGUILayout.LabelField("Engine Status: " + "Not Equipped, please add engine to the vehicle.");
        }

        if (handling) {
            GUI.color = Color.green; EditorGUILayout.LabelField("Handling Status: " + "Equipped, OK");
        } else {
            GUI.color = Color.red; EditorGUILayout.LabelField("Handling Status: " + "Not Equipped, please add stability to the vehicle.");
        }

        if (axles != null && axles.Length > 0) {
            GUI.color = Color.green; EditorGUILayout.LabelField("Axles Status: " + "Equipped, OK");
        } else {
            GUI.color = Color.red; EditorGUILayout.LabelField("Axles Status: " + "Not Equipped, please add an axle to the vehicle.");
        }

        if (nos) {
            GUI.color = Color.green; EditorGUILayout.LabelField("NOS Status: " + "Equipped, OK");
        } else {
            GUI.color = Color.red; EditorGUILayout.LabelField("NOS Status: " + "Not Equipped, please add a NOS to the vehicle.");
        }

        if (carController.GetComponentInChildren<RCCP_Customizer>(true)) {
            GUI.color = Color.green; EditorGUILayout.LabelField("Customizer Status: " + "Equipped, OK");
        } else {
            GUI.color = Color.red; EditorGUILayout.LabelField("Customizer Status: " + "Not Equipped, please add customizer to the vehicle to use customization.");
        }

        if (carController.GetComponentInChildren<RCCP_Customizer>(true)) {

            if (upgrade_Engine) {
                GUI.color = Color.green; EditorGUILayout.LabelField("Upgrader Engine Status: " + "Equipped, OK");
            } else {
                GUI.color = Color.red; EditorGUILayout.LabelField("Upgrader Engine Status: " + "Not Equipped, please add an engine upgrader to the customizer to use upgrades.");
            }

            if (upgrade_Handling) {
                GUI.color = Color.green; EditorGUILayout.LabelField("Upgrader Handling Status: " + "Equipped, OK");
            } else {
                GUI.color = Color.red; EditorGUILayout.LabelField("Upgrader Handling Status: " + "Not Equipped, please add a handling upgrader to the customizer to use upgrades.");
            }

            if (upgrade_Brake) {
                GUI.color = Color.green; EditorGUILayout.LabelField("Upgrader Brake Status: " + "Equipped, OK");
            } else {
                GUI.color = Color.red; EditorGUILayout.LabelField("Upgrader Brake Status: " + "Not Equipped, please add a brake upgrader to the customizer to use upgrades.");
            }

            if (upgrade_Speed) {
                GUI.color = Color.green; EditorGUILayout.LabelField("Upgrader Speed Status: " + "Equipped, OK");
            } else {
                GUI.color = Color.red; EditorGUILayout.LabelField("Upgrader Speed Status: " + "Not Equipped, please add a speed upgrader to the customizer to use upgrades.");
            }

        }

        GUI.color = orgGuiColor;

        EditorGUILayout.EndVertical();

        if (GUI.changed)
            EditorUtility.SetDirty(prop);

        serializedObject.ApplyModifiedProperties();

    }

    private void CreatePrefab() {

        PrefabUtility.SaveAsPrefabAssetAndConnect(prop.gameObject, "Assets/Burnout Racers/Resources/Player Cars/" + prop.gameObject.name + ".prefab", InteractionMode.UserAction);

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(prop);

        Debug.Log("Created Prefab");

    }

    private void SavePrefab() {

        PrefabUtility.SaveAsPrefabAssetAndConnect(prop.gameObject, "Assets/Burnout Racers/Resources/" +
            "Player Cars/" + prop.gameObject.name + ".prefab", InteractionMode.UserAction);

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(prop);

        Debug.Log("Saved Prefab");

    }

    private void AddToList() {

        var playerCarsSO = new SerializedObject(BR_PlayerCars.Instance);
        SerializedProperty carsList = playerCarsSO.FindProperty("playerCars");

        Object prefabRoot = PrefabUtility.GetCorrespondingObjectFromOriginalSource(prop.gameObject);

        carsList.InsertArrayElementAtIndex(carsList.arraySize);
        carsList.GetArrayElementAtIndex(carsList.arraySize - 1).FindPropertyRelative("car").objectReferenceValue = prefabRoot;
        carsList.GetArrayElementAtIndex(carsList.arraySize - 1).FindPropertyRelative("price").intValue = 0;

        playerCarsSO.ApplyModifiedProperties();

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(prop);

        Selection.activeObject = BR_PlayerCars.Instance;

        Debug.Log("Added Prefab To The Player Vehicles List");

    }

}
