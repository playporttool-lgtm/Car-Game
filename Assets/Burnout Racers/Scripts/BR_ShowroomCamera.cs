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
using UnityEngine.EventSystems;

/// <summary>
/// Showroom camera used in the main menu.
/// </summary>
public class BR_ShowroomCamera : MonoBehaviour {

    /// <summary>
    /// Camera target. Usually our spawn point.
    /// </summary>
    public Transform target;

    /// <summary>
    /// Z Distance.
    /// </summary>
    public float distance = 8f;

    /// <summary>
    /// Auto orbiting now?
    /// </summary>
    [Space]
    public bool orbitingNow = true;

    /// <summary>
    /// Auto orbiting speed.
    /// </summary>
    public float orbitSpeed = 5f;

    /// <summary>
    /// Smooth orbiting.
    /// </summary>
    [Space]
    public bool smooth = true;

    /// <summary>
    /// Smooth orbiting factor.
    /// </summary>
    public float smoothingFactor = 5f;

    /// <summary>
    /// Minimum Y degree.
    /// </summary>
    [Space]
    public float minY = 5f;

    /// <summary>
    /// Maximum Y degree.
    /// </summary>
    public float maxY = 35f;

    /// <summary>
    /// Drag speed.
    /// </summary>
    [Space]
    public float dragSpeed = 10f;

    /// <summary>
    /// Orbit X.
    /// </summary>
    public float orbitX = 0f;

    /// <summary>
    /// Orbit Y.
    /// </summary>
    public float orbitY = 0f;

    private void LateUpdate() {

        // If there is no target, return.
        if (!target) {

            Debug.LogWarning("Camera target not found!");
            enabled = false;
            return;

        }

        // If auto orbiting is enabled, increase orbitX slowly with orbitSpeed factor.
        if (orbitingNow)
            orbitX += Time.deltaTime * orbitSpeed;

        //  Clamping orbit Y.
        orbitY = ClampAngle(orbitY, minY, maxY);

        // Calculating rotation and position of the camera.
        Quaternion rotation = Quaternion.Euler(orbitY, orbitX, 0);
        Vector3 position = rotation * new Vector3(0f, 0f, -distance) + target.transform.position;

        // Setting position and rotation of the camera.
        if (!smooth)
            transform.SetPositionAndRotation(position, rotation);
        else
            transform.SetPositionAndRotation(Vector3.Lerp(transform.position, position, Time.unscaledDeltaTime * 10f), Quaternion.Slerp(transform.rotation, rotation, Time.unscaledDeltaTime * 10f));

    }

    /// <summary>
    /// Clamps the angle.
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    private float ClampAngle(float angle, float min, float max) {

        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;

        return Mathf.Clamp(angle, min, max);

    }

    /// <summary>
    /// Toggles the auto rotation bool.
    /// </summary>
    /// <param name="state"></param>
    public void ToggleAutoRotation(bool state) {

        orbitingNow = state;

    }

    /// <summary>
    /// On drag.
    /// </summary>
    /// <param name="pointerData"></param>
    public void OnDrag(PointerEventData pointerData) {

        // Receiving drag input from UI.
        orbitX += pointerData.delta.x * dragSpeed * .02f;
        orbitY -= pointerData.delta.y * dragSpeed * .02f;

    }

}
