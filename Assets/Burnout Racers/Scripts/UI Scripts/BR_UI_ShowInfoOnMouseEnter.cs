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
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BR_UI_ShowInfoOnMouseEnter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public GameObject info;

    public float offsetX = 135f;
    public float offsetY = -50f;

    private void Awake() {

        if (!info) {

            Debug.LogError("Info is not selected on " + transform.name + ", disabling it.");
            enabled = false;
            return;

        }

    }

    private void Start() {

        info.SetActive(false);

    }

    public void OnPointerEnter(PointerEventData data) {

        info.SetActive(true);

    }

    public void OnPointerExit(PointerEventData data) {

        info.SetActive(false);

    }

    private void Update() {

        if (!info.activeInHierarchy)
            return;

        Vector3 vpPosition = Camera.main.WorldToViewportPoint(Input.mousePosition);
        info.GetComponent<RectTransform>().anchoredPosition3D = vpPosition;

        Vector3[] v = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(v);

        float maxX = Mathf.Max(v[0].x, v[1].x, v[2].x, v[3].x);
        float minX = Mathf.Min(v[0].x, v[1].x, v[2].x, v[3].x);

        if (maxX < 0 || minX > Screen.height)
            info.transform.position = new Vector2(Input.mousePosition.x - offsetX, Input.mousePosition.y + offsetY);
        else
            info.transform.position = new Vector2(Input.mousePosition.x + offsetX, Input.mousePosition.y + offsetY);

    }

}
