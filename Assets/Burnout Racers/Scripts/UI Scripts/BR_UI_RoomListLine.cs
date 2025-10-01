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
/// UI element representing a room in the multiplayer room list.
/// </summary>
public class BR_UI_RoomListLine : MonoBehaviour {

    public TextMeshProUGUI roomNameText;
    public TextMeshProUGUI mapNameText;
    public TextMeshProUGUI gameTypeText;
    public TextMeshProUGUI roomPlayersText;
    public TextMeshProUGUI lapsText;
    public Button joinRoomButton;
    public GameObject lockedImage;

    private string roomName;
    private string roomPassword;

    private void Start() {

#if PHOTON_UNITY_NETWORKING

        //  Adding listener to the button to join the room when clicked.
        joinRoomButton.onClick.AddListener(() => {

            if (roomPassword == "")
                BR_PhotonLobbyManager.Instance.JoinToRoom(roomName);
            else
                BR_PhotonLobbyManager.Instance.OpenMenu(BR_PhotonLobbyManager.Instance.roomPasswordPanel);

            if (roomPassword != "")
                BR_PhotonLobbyManager.Instance.EnterRoomPassword(roomPassword, (result) => {

                    if (result)
                        BR_PhotonLobbyManager.Instance.JoinToRoom(roomName);
                    else
                        BR_UI_Informer.Instance.Info("Room Password Is Incorrect");

                });

        });

#endif

    }

    /// <summary>
    /// Initializes the room list line with the provided details.
    /// </summary>
    /// <param name="_roomName">Name of the room.</param>
    /// <param name="mapName">Name of the map.</param>
    /// <param name="gameType">Type of the game (e.g., Race, Practice).</param>
    /// <param name="laps">Number of laps for the race.</param>
    /// <param name="currentPlayers">Number of players currently in the room.</param>
    /// <param name="maxPlayers">Maximum number of players allowed in the room.</param>
    public void Initialize(string _roomName, string mapName, string gameType, int laps, byte currentPlayers, byte maxPlayers, string _roomPassword) {

        roomName = _roomName;
        roomPassword = _roomPassword;

        roomNameText.text = roomName;
        mapNameText.text = mapName;
        roomPlayersText.text = currentPlayers.ToString() + " / " + maxPlayers.ToString();
        gameTypeText.text = gameType;
        lapsText.text = laps.ToString();
        lockedImage.SetActive(roomPassword == "" ? false : true);

        // Disable the join button if the room is full.
        if (currentPlayers >= maxPlayers)
            joinRoomButton.interactable = false;
        else
            joinRoomButton.interactable = true;

    }

}
