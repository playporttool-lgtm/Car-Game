//----------------------------------------------
//			Burnout Racers
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// Mobile UI Drag used for orbiting the Showroom Camera in the main menu.
/// </summary>
public class BR_UI_MobileDrag : MonoBehaviour, IDragHandler {

    private BR_ShowroomCamera showroomCamera;

    private void Awake() {

        showroomCamera = FindFirstObjectByType<BR_ShowroomCamera>();

    }

    public void OnDrag(PointerEventData data) {

        if (!showroomCamera)
            showroomCamera = FindFirstObjectByType<BR_ShowroomCamera>();

        if (!showroomCamera)
            return;

        showroomCamera.OnDrag(data);

    }

}
