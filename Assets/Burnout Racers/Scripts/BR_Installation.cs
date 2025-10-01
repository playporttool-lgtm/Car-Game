//----------------------------------------------
//			Burnout Racers
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
//
//----------------------------------------------
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;

public class BR_Installation {

    public static void Check() {

        bool layer1;
        bool layer2;

        string[] missingLayers = new string[5];

        layer1 = LayerExists("BR_Prop");
        layer2 = LayerExists("BR_Road");

        if (!layer1)
            missingLayers[0] = "BR_Prop";

        if (!layer2)
            missingLayers[2] = "BR_Road";

        if (!layer1 || !layer2) {

            if (EditorUtility.DisplayDialog("Found Missing Layers For Burnout Racers", "These layers will be added to the Tags and Layers\n\n" + missingLayers[0] + "\n" + missingLayers[1] + "\n" + missingLayers[2] + "\n", "Add")) {

                CheckLayer("BR_Prop");
                CheckLayer("BR_Road");

            }

        }

        //if (Directory.Exists("Assets/Burnout Racers/Temp")) {

        //    string TP_DemoVehicles = "Assets/Burnout Racers/Temp/RCCP_DemoVehicles.asset";
        //    string TP_DemoAssets = "Assets/Burnout Racers/Temp/RCCP_DemoContent.asset";
        //    string TP_DemoMaterials = "Assets/Burnout Racers/Temp/RCCP_DemoMaterials.asset";
        //    string TP_DemoScenes = "Assets/Burnout Racers/Temp/RCCP_DemoScenes.asset";

        //    string OP_DemoVehicles = "Assets/Burnout Racers/Realistic Car Controller Pro/Resources/RCCP_DemoVehicles.asset";
        //    string OP_DemoAssets = "Assets/Burnout Racers/Realistic Car Controller Pro/Resources/RCCP_DemoContent.asset";
        //    string OP_DemoMaterials = "Assets/Burnout Racers/Realistic Car Controller Pro/Resources/RCCP_DemoMaterials.asset";
        //    string OP_DemoScenes = "Assets/Burnout Racers/Realistic Car Controller Pro/Resources/RCCP_DemoScenes.asset";

        //    AssetDatabase.MoveAsset(TP_DemoVehicles, OP_DemoVehicles);
        //    AssetDatabase.MoveAsset(TP_DemoAssets, OP_DemoAssets);
        //    AssetDatabase.MoveAsset(TP_DemoMaterials, OP_DemoMaterials);
        //    AssetDatabase.MoveAsset(TP_DemoScenes, OP_DemoScenes);

        //    AssetDatabase.SaveAssets();

        //}

    }

    public static bool CheckTag(string tagName) {

        if (TagExists(tagName))
            return true;

        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        if (!PropertyExists(tagsProp, 0, tagsProp.arraySize, tagName)) {

            int index = tagsProp.arraySize;

            tagsProp.InsertArrayElementAtIndex(index);
            SerializedProperty sp = tagsProp.GetArrayElementAtIndex(index);

            sp.stringValue = tagName;
            Debug.Log("Tag: " + tagName + " has been added.");

            tagManager.ApplyModifiedProperties();

            return true;

        }

        return false;

    }

    public static string NewTag(string name) {

        CheckTag(name);

        if (name == null || name == "")
            name = "Untagged";

        return name;

    }

    public static bool RemoveTag(string tagName) {

        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        if (PropertyExists(tagsProp, 0, tagsProp.arraySize, tagName)) {

            SerializedProperty sp;

            for (int i = 0, j = tagsProp.arraySize; i < j; i++) {

                sp = tagsProp.GetArrayElementAtIndex(i);

                if (sp.stringValue == tagName) {

                    tagsProp.DeleteArrayElementAtIndex(i);
                    Debug.Log("Tag: " + tagName + " has been removed.");
                    tagManager.ApplyModifiedProperties();
                    return true;

                }

            }

        }

        return false;

    }

    public static bool TagExists(string tagName) {

        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        return PropertyExists(tagsProp, 0, 10000, tagName);

    }

    public static bool CheckLayer(string layerName) {

        if (LayerExists(layerName))
            return true;

        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layersProp = tagManager.FindProperty("layers");

        if (!PropertyExists(layersProp, 0, 31, layerName)) {

            SerializedProperty sp;

            for (int i = 8, j = 31; i < j; i++) {

                sp = layersProp.GetArrayElementAtIndex(i);

                if (sp.stringValue == "") {

                    sp.stringValue = layerName;
                    Debug.Log("Layer: " + layerName + " has been added.");
                    tagManager.ApplyModifiedProperties();
                    return true;

                }

                if (i == j)
                    Debug.Log("All allowed layers have been filled.");

            }

        }

        return false;

    }

    public static string NewLayer(string name) {

        if (name != null || name != "")
            CheckLayer(name);

        return name;

    }

    public static bool RemoveLayer(string layerName) {

        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layersProp = tagManager.FindProperty("layers");

        if (PropertyExists(layersProp, 0, layersProp.arraySize, layerName)) {

            SerializedProperty sp;

            for (int i = 0, j = layersProp.arraySize; i < j; i++) {

                sp = layersProp.GetArrayElementAtIndex(i);

                if (sp.stringValue == layerName) {

                    sp.stringValue = "";
                    Debug.Log("Layer: " + layerName + " has been removed.");
                    // Save settings
                    tagManager.ApplyModifiedProperties();
                    return true;

                }

            }

        }

        return false;

    }

    public static bool LayerExists(string layerName) {

        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layersProp = tagManager.FindProperty("layers");
        return PropertyExists(layersProp, 0, 31, layerName);

    }

    private static bool PropertyExists(SerializedProperty property, int start, int end, string value) {

        for (int i = start; i < end; i++) {

            SerializedProperty t = property.GetArrayElementAtIndex(i);

            if (t.stringValue.Equals(value))
                return true;

        }

        return false;

    }

}
#endif