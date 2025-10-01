//----------------------------------------------
//			Burnout Racers
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
#endif

/// <summary>
/// UI Player line used in the main menu.
/// </summary>
public class BR_UI_PlayerListLine : MonoBehaviour {

    public TextMeshProUGUI PlayerNameText;
    public TextMeshProUGUI PlayerReadyText;
    public GameObject kickButton;

    private string playerName = "";
    public bool isPlayerReady = false;

    private void OnEnable() {

        kickButton.SetActive(false);

    }

    /// <summary>
    /// Registering the player.
    /// </summary>
    /// <param name="nickName"></param>
    public void Register(string nickName) {

        playerName = nickName;
        PlayerNameText.text = playerName;

        SetPlayerReady(false);

#if PHOTON_UNITY_NETWORKING

        if (PhotonNetwork.IsMasterClient && nickName != PhotonNetwork.LocalPlayer.NickName)
            kickButton.SetActive(true);

#endif

    }

    /// <summary>
    /// Setting ready status.
    /// </summary>
    /// <param name="playerReady"></param>
    public void SetPlayerReady(bool playerReady) {

        isPlayerReady = playerReady;
        PlayerReadyText.text = playerReady ? "Ready" : "Not Ready";

    }

    /// <summary>
    /// Kicks the target player.
    /// </summary>
    public void KickPlayer() {

#if PHOTON_UNITY_NETWORKING

        if (playerName != "")
            BR_PhotonLobbyManager.Instance.KickPlayerPanel(playerName);

#endif

    }

}
