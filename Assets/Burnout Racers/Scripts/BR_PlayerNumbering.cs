//----------------------------------------------
//			Burnout Racers
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
//
//----------------------------------------------

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
#endif

#if PHOTON_UNITY_NETWORKING

/// <summary>
/// Assigning unique numbers to each network player. It will be used to spawn players at different spawn points.
/// </summary>
public class BR_PlayerNumbering : MonoBehaviourPunCallbacks {

    /// <summary>
    /// Instance.
    /// </summary>
    public static BR_PlayerNumbering instance;

    public static Player[] SortedPlayers;

    /// <summary>
    /// OnPlayerNumberingChanged delegate.
    /// </summary>
    public delegate void PlayerNumberingChanged();
    /// <summary>
    /// Called everytime the room Indexing was updated.
    /// </summary>
    public static event PlayerNumberingChanged OnPlayerNumberingChanged;

    /// <summary>
    /// Defines the room custom property name to use for room player indexing tracking.
    /// </summary>
    public const string RoomPlayerIndexedProp = "pNr";

    /// <summary>
    /// dont destroy on load flag for this Component's GameObject to survive Level Loading.
    /// </summary>
    public bool dontDestroyOnLoad = true;

    public void Awake() {

        if (instance != null && instance != this && instance.gameObject != null)
            DestroyImmediate(instance.gameObject);

        instance = this;

        if (dontDestroyOnLoad)
            DontDestroyOnLoad(this.gameObject);

        this.RefreshData();

    }

    public override void OnJoinedRoom() {

        this.RefreshData();

    }

    public override void OnLeftRoom() {

        PhotonNetwork.LocalPlayer.CustomProperties.Remove(BR_PlayerNumbering.RoomPlayerIndexedProp);

    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {

        this.RefreshData();

    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {

        this.RefreshData();

    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {

        if (changedProps != null && changedProps.ContainsKey(BR_PlayerNumbering.RoomPlayerIndexedProp))
            this.RefreshData();

    }

    public void RefreshData() {

        if (PhotonNetwork.CurrentRoom == null)
            return;

        if (PhotonNetwork.LocalPlayer.GetPlayerNumber() >= 0) {

            SortedPlayers = PhotonNetwork.CurrentRoom.Players.Values.OrderBy((p) => p.GetPlayerNumber()).ToArray();

            if (OnPlayerNumberingChanged != null)
                OnPlayerNumberingChanged();

            return;

        }


        HashSet<int> usedInts = new HashSet<int>();
        Player[] sorted = PhotonNetwork.PlayerList.OrderBy((p) => p.ActorNumber).ToArray();

        string allPlayers = "all players: ";

        foreach (Player player in sorted) {

            allPlayers += player.ActorNumber + "=pNr:" + player.GetPlayerNumber() + ", ";

            int number = player.GetPlayerNumber();

            if (player.IsLocal) {

                Debug.Log("PhotonNetwork.CurrentRoom.PlayerCount = " + PhotonNetwork.CurrentRoom.PlayerCount);

                // select a number
                for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++) {

                    if (!usedInts.Contains(i)) {

                        player.SetPlayerNumber(i);
                        break;

                    }

                }

                break;

            } else {

                if (number < 0)
                    break;
                else
                    usedInts.Add(number);

            }

        }

        SortedPlayers = PhotonNetwork.CurrentRoom.Players.Values.OrderBy((p) => p.GetPlayerNumber()).ToArray();

        if (OnPlayerNumberingChanged != null)
            OnPlayerNumberingChanged();

    }

}



public static class PlayerNumberingExtensions {

    public static int GetPlayerNumber(this Player player) {

        if (player == null)
            return -1;

        if (PhotonNetwork.OfflineMode)
            return 0;

        if (!PhotonNetwork.IsConnectedAndReady)
            return -1;

        object value;

        if (player.CustomProperties.TryGetValue(BR_PlayerNumbering.RoomPlayerIndexedProp, out value))
            return (byte)value;

        return -1;

    }

    public static void SetPlayerNumber(this Player player, int playerNumber) {

        if (player == null)
            return;

        if (PhotonNetwork.OfflineMode)
            return;

        if (playerNumber < 0)
            Debug.LogWarning("Setting invalid playerNumber: " + playerNumber + " for: " + player.ToStringFull());

        if (!PhotonNetwork.IsConnectedAndReady) {

            Debug.LogWarning("SetPlayerNumber was called in state: " + PhotonNetwork.NetworkClientState + ". Not IsConnectedAndReady.");
            return;

        }

        int current = player.GetPlayerNumber();

        if (current != playerNumber) {

            Debug.Log("PlayerNumbering: Set number " + playerNumber);
            player.SetCustomProperties(new Hashtable() { { BR_PlayerNumbering.RoomPlayerIndexedProp, (byte)playerNumber } });

        }

    }

}

#else

public class BR_PlayerNumbering : MonoBehaviour { }

#endif