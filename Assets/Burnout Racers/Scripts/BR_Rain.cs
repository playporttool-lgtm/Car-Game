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

/// <summary>
/// Rain will be tracking the active main camera in the scene.
/// </summary>
public class BR_Rain : MonoBehaviour {

    /// <summary>
    /// Local position of the rain relative to the camera.
    /// </summary>
    public Vector3 localPos;

    /// <summary>
    /// Local rotation of the rain relative to the camera.
    /// </summary>
    public Quaternion localRot;

    private void Update() {

        // Get the main camera in the scene.
        Camera mainCam = Camera.main;

        // If no main camera is found, return.
        if (!mainCam)
            return;

        // Set the rain's position and rotation to match the camera's.
        transform.position = mainCam.transform.position;
        transform.rotation = mainCam.transform.rotation;

        // Adjust the rain's position based on the local offset.
        transform.position += transform.forward * localPos.z;
        transform.position += transform.up * localPos.y;

        // Adjust the rain's rotation based on the local rotation.
        transform.rotation *= localRot;

    }

    /// <summary>
    /// Context menu function to get the current local position and rotation of the rain.
    /// </summary>
    [ContextMenu("Get Position")]
    public void GetPos() {

        // Store the current local position and rotation.
        localPos = transform.localPosition;
        localRot = transform.localRotation;

    }

}
