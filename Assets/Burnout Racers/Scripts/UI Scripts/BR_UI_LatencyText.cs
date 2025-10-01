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
using TMPro;
#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
#endif

/// <summary>
/// Latency text.
/// </summary>
public class BR_UI_LatencyText : MonoBehaviour {

    private TextMeshProUGUI text;

    private void Start() {

        text = GetComponent<TextMeshProUGUI>();
        text.text = "";

        InvokeRepeating(nameof(UpdateText), 1f, 1f);

    }

    private void UpdateText() {

#if PHOTON_UNITY_NETWORKING

        if (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom) {

            text.text = "";
            return;

        }

        text.text = PhotonNetwork.GetPing().ToString("F0") + " Ms";

#else

        text.text = "";

#endif

    }

}
