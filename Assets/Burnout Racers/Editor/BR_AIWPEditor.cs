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

[CustomEditor(typeof(BR_AIWaypointsContainer))]
public class BR_AIWPEditor : Editor {

    BR_AIWaypointsContainer wpScript;

    public override void OnInspectorGUI() {

        wpScript = (BR_AIWaypointsContainer)target;
        serializedObject.Update();

        EditorGUILayout.HelpBox("Create Waypoints By Shift + Left Mouse Button On Your Road", MessageType.Info);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("waypoints"), new GUIContent("Waypoints", "Waypoints"), true);

        foreach (Transform item in wpScript.transform) {

            if (item != wpScript.transform && item.gameObject.GetComponent<BR_Waypoint>() == null)
                item.gameObject.AddComponent<BR_Waypoint>();

        }

        if (GUILayout.Button("Delete Waypoints")) {

            foreach (BR_Waypoint t in wpScript.waypoints)
                DestroyImmediate(t.gameObject);

            wpScript.waypoints.Clear();
            EditorUtility.SetDirty(wpScript);

        }

        if (GUILayout.Button("Invert")) {

            Invert();
            EditorUtility.SetDirty(wpScript);

        }

        if (GUILayout.Button("Smooth")) {

            Smooth();
            EditorUtility.SetDirty(wpScript);

        }

        if (GUI.changed)
            EditorUtility.SetDirty(wpScript);

        serializedObject.ApplyModifiedProperties();

    }

    private void OnSceneGUI() {

        Event e = Event.current;
        wpScript = (BR_AIWaypointsContainer)target;

        if (e != null) {

            if (e.isMouse && e.shift && e.type == EventType.MouseDown) {

                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hit = new RaycastHit();

                int controlId = GUIUtility.GetControlID(FocusType.Passive);

                // Tell the UI your event is the main one to use, it override the selection in  the scene view
                GUIUtility.hotControl = controlId;
                // Don't forget to use the event
                Event.current.Use();

                if (Physics.Raycast(ray, out hit, 5000.0f)) {

                    Vector3 newTilePosition = hit.point;

                    GameObject wp = new GameObject("Waypoint " + wpScript.waypoints.Count.ToString());
                    wp.AddComponent<BR_Waypoint>();
                    wp.transform.position = newTilePosition;
                    wp.transform.SetParent(wpScript.transform);

                    wpScript.GetAllWaypoints();

                    EditorUtility.SetDirty(wpScript);

                }

            }

        }

        wpScript.GetAllWaypoints();

    }

    public void Invert() {

        List<BR_Waypoint> waypoints = new List<BR_Waypoint>();
        wpScript.GetAllWaypoints();

        for (int i = 0; i < wpScript.waypoints.Count; i++) {

            if (wpScript.waypoints[i] != null)
                waypoints.Add(wpScript.waypoints[i]);

        }

        // Sort children in reverse order
        waypoints.Reverse();

        for (int i = 0; i < waypoints.Count; i++)
            waypoints[i].transform.SetSiblingIndex(i);

        wpScript.waypoints = waypoints;

    }

    public void Smooth() {

        List<BR_Waypoint> waypoints = new List<BR_Waypoint>();
        wpScript.GetAllWaypoints();

        for (int i = 0; i < wpScript.waypoints.Count; i++) {

            if (wpScript.waypoints[i] != null)
                waypoints.Add(wpScript.waypoints[i]);

        }

        for (int i = 0; i < waypoints.Count; i++) {

            if (i < waypoints.Count - 1) {

                float distance = Vector3.Distance(waypoints[i].transform.position, waypoints[i + 1].transform.position);

                if (distance >= 5f) {

                    BR_Waypoint newWP = new GameObject("Waypoint_S").AddComponent<BR_Waypoint>();
                    newWP.transform.SetParent(wpScript.transform);
                    newWP.targetSpeed = (waypoints[i].targetSpeed + waypoints[i + 1].targetSpeed) / 2f;
                    newWP.transform.position = Vector3.Lerp(waypoints[i].transform.position, waypoints[i + 1].transform.position, .5f);
                    newWP.transform.rotation = Quaternion.Lerp(waypoints[i].transform.rotation, waypoints[i + 1].transform.rotation, .5f);
                    newWP.transform.SetSiblingIndex(waypoints[i].transform.GetSiblingIndex() + 1);

                }

            } else {

                float distance = Vector3.Distance(waypoints[i].transform.position, waypoints[0].transform.position);

                if (distance >= 5f) {

                    BR_Waypoint newWP = new GameObject("Waypoint_S").AddComponent<BR_Waypoint>();
                    newWP.transform.SetParent(wpScript.transform);
                    newWP.targetSpeed = (waypoints[i].targetSpeed + waypoints[0].targetSpeed) / 2f;
                    newWP.transform.position = Vector3.Lerp(waypoints[i].transform.position, waypoints[0].transform.position, .5f);
                    newWP.transform.rotation = Quaternion.Lerp(waypoints[i].transform.rotation, waypoints[0].transform.rotation, .5f);
                    newWP.transform.SetSiblingIndex(waypoints[i].transform.GetSiblingIndex() + 1);

                }

            }

        }

    }

}
