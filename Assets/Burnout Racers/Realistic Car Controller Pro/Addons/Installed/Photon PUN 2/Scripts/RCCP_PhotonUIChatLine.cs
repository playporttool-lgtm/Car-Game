//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2025 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RCCP_PhotonUIChatLine : MonoBehaviour {

    public Text text;

    private void Awake() {

        text = GetComponent<Text>();

    }

    public void Line(string chatText) {

        text.text = chatText;

    }

}
