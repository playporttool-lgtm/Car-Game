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
using TMPro;

public class BR_NicknameText3D : MonoBehaviour {

    public TextMeshPro text;
    public float alpha = .25f;

    private BR_PlayerManager playerManager;

    private Vector3 defLocation = Vector3.zero;

    private void Awake() {

        defLocation = text.transform.localPosition;
        text.alpha = alpha;

    }

    private IEnumerator Start() {

        Clean();

        yield return new WaitForSeconds(.5f);

        playerManager = GetComponent<BR_PlayerManager>();

        if (playerManager && playerManager.nickName != null)
            SetNick(playerManager.nickName);

        if (playerManager && playerManager.nickName == BR_API.GetPlayerName())
            Clean();

    }

    public void Clean() {

        text.text = "";

    }

    public void SetNick(string newNickName) {

        text.text = newNickName;

    }

    private void Update() {

        text.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        text.transform.position += Vector3.up * defLocation.y;

        Camera mainCamera = Camera.main;

        if (mainCamera)
            text.transform.forward = mainCamera.transform.forward;

    }

    private void Reset() {

        text = GetComponentInChildren<TextMeshPro>();

        if (!text) {

            GameObject newTextGO = new GameObject("BR_NicknameText3D");
            newTextGO.transform.SetParent(transform, false);
            newTextGO.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            newTextGO.transform.localScale = new Vector3(.22f, .22f, .22f);
            newTextGO.transform.position += Vector3.up * 1.8f;

            TextMeshPro textMeshProUGUI = newTextGO.AddComponent<TextMeshPro>();
            textMeshProUGUI.text = "Nickname";
            textMeshProUGUI.horizontalAlignment = HorizontalAlignmentOptions.Center;
            textMeshProUGUI.verticalAlignment = VerticalAlignmentOptions.Middle;
            textMeshProUGUI.fontSize = 36;

            text = textMeshProUGUI;

        }

    }

}
