//----------------------------------------------
//			Burnout Racers
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
//
//----------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
#if PHOTON_UNITY_NETWORKING
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
#endif

#if PHOTON_UNITY_NETWORKING

/// <summary>
/// Main menu lobby system with Photon PUN2.
/// </summary>
[RequireComponent(typeof(PhotonView))]
public class BR_PhotonLobbyManager : MonoBehaviourPunCallbacks {

    private static BR_PhotonLobbyManager instance;

    /// <summary>
    /// Instance.
    /// </summary>
    public static BR_PhotonLobbyManager Instance {

        get {

            if (instance == null)
                instance = FindFirstObjectByType<BR_PhotonLobbyManager>();

            return instance;

        }

    }

    /// <summary>
    /// Connection status text.
    /// </summary>
    [Header("Texts")]
    public TextMeshProUGUI connectionStatusText;

    /// <summary>
    /// Total online players text.
    /// </summary>
    [Header("Texts")]
    public TextMeshProUGUI totalOnlinePlayersText;

    /// <summary>
    /// Room name input field.
    /// </summary>
    [Header("Input Fields")]
    public TMP_InputField roomNameInput;
    public TMP_InputField roomPasswordInput;
    public TMP_InputField roomPasswordEnterInput;
    public TMP_InputField searchRoomNameInput;

    /// <summary>
    /// Buttons and infos.
    /// </summary>
    [Header("Buttons")]
    public GameObject readyButton;
    public GameObject startGameButton;
    public Image readyOnImage;
    public GameObject readyInfo;
    public GameObject noActiveRooms;

    /// <summary>
    /// Ready on color.
    /// </summary>
    public Color readyOnColor;

    /// <summary>
    /// Max players toggles.
    /// </summary>
    public Toggle maxPlayers2;
    public Toggle maxPlayers3;
    public Toggle maxPlayers4;
    public Toggle maxPlayers5;
    public Toggle maxPlayers6;
    public Toggle maxPlayers7;

    /// <summary>
    /// Current maximum players.
    /// </summary>
    public int currentMaxPlayers = 4;

    /// <summary>
    /// Panels.
    /// </summary>
    [Header("Panels")]
    public GameObject connectingPanel;
    public GameObject loginPanel;
    public GameObject selectionPanel;
    public GameObject sceneSelectionPanel;
    public GameObject createRoomPanel;
    public GameObject creatingRoomPanel;
    public GameObject joinRandomRoomPanel;
    public GameObject joiningPanel;
    public GameObject roomListPanel;
    public GameObject playerListPanel;
    public GameObject kickPlayerPanel;
    public GameObject joiningRoomPanel;
    public GameObject leavingRoomPanel;
    public GameObject roomPasswordPanel;

    /// <summary>
    /// Roomlist Content and Prefab
    /// </summary>
    [Header("Roomlist Content and Prefab")]
    public GameObject roomListContent;
    public GameObject roomListEntryPrefab;

    /// <summary>
    /// Playerlist Content and Prefab
    /// </summary>
    [Header("Playerlist Content and Prefab")]
    public GameObject playerListContent;
    public GameObject playerListEntryPrefab;

    /// <summary>
    /// Region Buttons
    /// </summary>
    [Header("Region Buttons")]
    public TMP_Dropdown regionDropdown;

#if PHOTON_UNITY_NETWORKING

    /// <summary>
    /// Dictionaries for cached rooms and players.
    /// </summary>
    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;
    private Dictionary<string, BR_UI_PlayerListLine> playerListEntries;

#endif

    /// <summary>
    /// Randomizes the room name.
    /// </summary>
    [Space()]
    public bool randomizeRoomName = true;

#if PHOTON_UNITY_NETWORKING

    /// <summary>
    /// All players in the room.
    /// </summary>
    public List<Player> playersInRoom = new List<Player>();

#endif

    /// <summary>
    /// Kicked player's nickname.
    /// </summary>
    private string kickPlayerName = "";

    /// <summary>
    /// Handles enabling the Photon Lobby Manager, including connecting to the master server.
    /// </summary>
    public override void OnEnable() {

        base.OnEnable();

#if PHOTON_UNITY_NETWORKING

        //  Setting rates.
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 30;

        //  Make sure auto sync scene is disabled.
        PhotonNetwork.AutomaticallySyncScene = true;

        //  Connecting to the server.
        ConnectMasterServer();

#endif

    }

    /// <summary>
    /// Initializes various settings on start, including room name randomization and player count management.
    /// </summary>
    private void Start() {

#if PHOTON_UNITY_NETWORKING

        //  Initializing dictionaries.
        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListEntries = new Dictionary<string, GameObject>();
        playerListEntries = new Dictionary<string, BR_UI_PlayerListLine>();

        //  Randomizing the room name.
        if (randomizeRoomName)
            roomNameInput.SetTextWithoutNotify(BR_API.GetPlayerName() + "'s Room " + UnityEngine.Random.Range(0, 10000).ToString());

        if (roomPasswordInput)
            roomPasswordInput.SetTextWithoutNotify("");

        if (roomPasswordEnterInput)
            roomPasswordEnterInput.SetTextWithoutNotify("");

        if (searchRoomNameInput)
            searchRoomNameInput.SetTextWithoutNotify("");

        //  Current maximum players.
        currentMaxPlayers = 4;

        //  Managing maximum player toggles.
        switch (currentMaxPlayers) {

            case 2:

                maxPlayers2.SetIsOnWithoutNotify(true);
                maxPlayers3.SetIsOnWithoutNotify(false);
                maxPlayers4.SetIsOnWithoutNotify(false);
                maxPlayers5.SetIsOnWithoutNotify(false);
                maxPlayers6.SetIsOnWithoutNotify(false);
                maxPlayers7.SetIsOnWithoutNotify(false);

                break;

            case 3:

                maxPlayers2.SetIsOnWithoutNotify(false);
                maxPlayers3.SetIsOnWithoutNotify(true);
                maxPlayers4.SetIsOnWithoutNotify(false);
                maxPlayers5.SetIsOnWithoutNotify(false);
                maxPlayers6.SetIsOnWithoutNotify(false);
                maxPlayers7.SetIsOnWithoutNotify(false);

                break;

            case 4:

                maxPlayers2.SetIsOnWithoutNotify(false);
                maxPlayers3.SetIsOnWithoutNotify(false);
                maxPlayers4.SetIsOnWithoutNotify(true);
                maxPlayers5.SetIsOnWithoutNotify(false);
                maxPlayers6.SetIsOnWithoutNotify(false);
                maxPlayers7.SetIsOnWithoutNotify(false);

                break;

            case 5:

                maxPlayers2.SetIsOnWithoutNotify(false);
                maxPlayers3.SetIsOnWithoutNotify(false);
                maxPlayers4.SetIsOnWithoutNotify(false);
                maxPlayers5.SetIsOnWithoutNotify(true);
                maxPlayers6.SetIsOnWithoutNotify(false);
                maxPlayers7.SetIsOnWithoutNotify(false);

                break;

            case 6:

                maxPlayers2.SetIsOnWithoutNotify(false);
                maxPlayers3.SetIsOnWithoutNotify(false);
                maxPlayers4.SetIsOnWithoutNotify(false);
                maxPlayers5.SetIsOnWithoutNotify(false);
                maxPlayers6.SetIsOnWithoutNotify(true);
                maxPlayers7.SetIsOnWithoutNotify(false);

                break;

            case 7:

                maxPlayers2.SetIsOnWithoutNotify(false);
                maxPlayers3.SetIsOnWithoutNotify(false);
                maxPlayers4.SetIsOnWithoutNotify(false);
                maxPlayers5.SetIsOnWithoutNotify(false);
                maxPlayers6.SetIsOnWithoutNotify(false);
                maxPlayers7.SetIsOnWithoutNotify(true);

                break;

        }

        //  Region.
        string region = PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion;

        //  Setting region text.
        if (region != "") {

            switch (region) {

                case "eu":
                    regionDropdown.SetValueWithoutNotify(0);
                    break;

                case "us":
                    regionDropdown.SetValueWithoutNotify(1);
                    break;

                case "asia":
                    regionDropdown.SetValueWithoutNotify(2);
                    break;

            }

        } else {

            regionDropdown.SetValueWithoutNotify(3);

        }

#endif

    }

    /// <summary>
    /// Connecting to the master server.
    /// </summary>
    public void ConnectMasterServer() {

#if PHOTON_UNITY_NETWORKING

        // If not connected, connect. Otherwise open selection panel directly.
        if (!PhotonNetwork.IsConnectedAndReady) {

            OpenMenu(loginPanel);

        } else {

            //  Join the lobby if we're not in.
            if (!PhotonNetwork.InLobby)
                PhotonNetwork.JoinLobby();
            else
                OpenMenu(selectionPanel);

            //  Leave the room if we're in.
            if (PhotonNetwork.InRoom)
                PhotonNetwork.LeaveRoom();

        }

#endif

    }

    #region CALLBACKS

#if PHOTON_UNITY_NETWORKING

    /// <summary>
    /// When connected to the server.
    /// </summary>
    public override void OnConnectedToMaster() {

        //  Join the lobby if we're not in.
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();
        else
            OpenMenu(selectionPanel);

    }

    public override void OnJoinedLobby() {

        //  Open the selection menu.
        OpenMenu(selectionPanel);

    }

/// <summary>
/// When joined a room.
/// </summary>
public override void OnJoinedRoom() {

    //  Getting the room's properties.
    Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

    //  Game type.
    string gameType = (string)roomProperties["gametype"];

    //  Setting the game type according to the room properties.
    switch (gameType) {

        case "Race":
            BR_API.SetGameType(0);
            break;

        case "Practice":
            BR_API.SetGameType(1);
            break;

    }

    //  Adding necessary properties to the player's custom properties.
    Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
    
    // Check if it's a 2-player room for auto-ready
    if (PhotonNetwork.CurrentRoom.MaxPlayers == 2) {
        hash["Ready"] = true;  // Auto-ready for 2-player rooms
    } else {
        hash["Ready"] = false;
        
        if (BR_API.GetGameType() == 1)
            hash["Ready"] = true;
    }

    PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

    //  Getting the room's scene name.
    int sceneIndex = (int)roomProperties["scene"];

    BR_MainMenuManager.Instance.SelectScene(sceneIndex);

    //  Laps.
    int laps = (int)roomProperties["laps"];

    //  Setting target laps.
    BR_API.SetLapsAmount((int)laps);

    //  Opening the player list panel (In room).
    OpenMenu(playerListPanel);

    //  Clearing the player list content.
    ClearPlayerListView();

    //  Clearing the players in room.
    playersInRoom.Clear();

    //  For each player in the room, add it to the list.
    foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        playersInRoom.Add(player);

    //  Update the player list.
    UpdatePlayerListView(playersInRoom);

    //  Check ready and start buttons.
    CheckButtonsInRoom();

}

    /// <summary>
    /// When failed to join a room.
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRoomFailed(short returnCode, string message) {

        BR_UI_Informer.Instance.Info("FAILED TO JOIN THE ROOM");

        //  Opening the selection panel.
        OpenMenu(selectionPanel);

    }

    /// <summary>
    /// When failed to join a random room.
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRandomFailed(short returnCode, string message) {

        BR_UI_Informer.Instance.Info("NO ANY ACTIVE ROOMS FOR MATCHMAKING\nCREATING A NEW ROOM");

    }

    /// <summary>
    /// When left room.
    /// </summary>
    public override void OnLeftRoom() {

        //  Adding necessary properties to the custom properties.
        Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
        hash["Ready"] = false;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        //  Opening the selection panel.
        OpenMenu(selectionPanel);

    }

    /// <summary>
    /// When failed to create a room.
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnCreateRoomFailed(short returnCode, string message) {

        BR_UI_Informer.Instance.Info("FAILED TO CREATE A ROOM");

        //  Opening the selection panel.
        OpenMenu(selectionPanel);

    }

    /// <summary>
    /// When updated room list in the lobby.
    /// </summary>
    /// <param name="roomList"></param>
    public override void OnRoomListUpdate(List<RoomInfo> roomList) {

        ClearRoomListView();
        UpdateCachedRoomList(roomList);
        UpdateRoomListView(roomList);
        SearchRoom();

    }

    /// <summary>
    /// When a player entered the room.
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Player newPlayer) {

        //  Clearing the player list view.
        ClearPlayerListView();

        //  Clearing the players in room.
        playersInRoom.Clear();

        //  For each player in the room, add it to the list.
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
            playersInRoom.Add(player);

        //  Updating the player list view.
        UpdatePlayerListView(playersInRoom);

        //  Checking ready and start buttons.
        CheckButtonsInRoom();

    }

    /// <summary>
    /// When a player left the room.
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Player otherPlayer) {

        //  Clearing the player list view.
        ClearPlayerListView();

        //  Clearing the players in room.
        playersInRoom.Clear();

        //  For each player in the room, add it to the list.
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
            playersInRoom.Add(player);

        //  Updating the player list view.
        UpdatePlayerListView(playersInRoom);

        //  Checking ready and start buttons.
        CheckButtonsInRoom();

    }

    /// <summary>
    /// When a player updates his own player custom properties.
    /// </summary>
    /// <param name="targetPlayer"></param>
    /// <param name="changedProps"></param>
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {

        //  Checking ready statements.
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) {

            if (playerListEntries.TryGetValue(PhotonNetwork.PlayerList[i].NickName, out BR_UI_PlayerListLine playerLine))
                playerLine.SetPlayerReady((bool)PhotonNetwork.PlayerList[i].CustomProperties["Ready"]);

        }

        //  Checking ready and start buttons.
        CheckButtonsInRoom();

    }

    /// <summary>
    /// When properties of the room has changed by master client.
    /// </summary>
    /// <param name="propertiesThatChanged"></param>
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) {

        //  Checking ready and start buttons.
        CheckButtonsInRoom();

    }

    public override void OnMasterClientSwitched(Player newMasterClient) {

        BR_UI_Informer.Instance.Info("MASTER HAS BEEN CHANGED TO " + newMasterClient.NickName);

        if (newMasterClient.NickName == PhotonNetwork.LocalPlayer.NickName)
            BR_UI_Informer.Instance.Info("YOU ARE THE MASTER NOW");

    }

    public override void OnDisconnected(DisconnectCause cause) {

        if (cause != DisconnectCause.DisconnectByClientLogic)
            BR_UI_Informer.Instance.Info("DISCONNECTED! REASON: " + cause.ToString());

        //  Opening the login panel.
        OpenMenu(loginPanel);

    }

#endif

    #endregion

#if PHOTON_UNITY_NETWORKING

/// <summary>
/// Checks ready and start game buttons in the room panel.
/// </summary>
private void CheckButtonsInRoom() {

    // Auto-start for 2-player rooms when both players are ready
    if (PhotonNetwork.CurrentRoom.MaxPlayers == 2 && PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.LocalPlayer.IsMasterClient) {
        bool bothReady = true;
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values) {
            if (!(bool)player.CustomProperties["Ready"]) {
                bothReady = false;
                break;
            }
        }
        
        if (bothReady) {
            // Start game automatically after 1 second
            Invoke("StartGameButton", 1f);
            return; // Exit early since we're auto-starting
        }
    }

    //  If we're not the master client, disable the start game button. Only master client can start the game.
    if (!PhotonNetwork.LocalPlayer.IsMasterClient) {

        //  Disabling the start game button.
        startGameButton.SetActive(false);

    } else {

        //  Everyone ready?
        bool allReady = true;

        //  For each player in the room, check their ready statements.
        foreach (var item in playerListEntries) {

            if (item.Value.PlayerNameText.text != "Waiting Player..." && !item.Value.isPlayerReady)
                allReady = false;

        }

        //  If game type is race, enable the start game button only if everyone is ready. Master client can't start the game if there are no any other players in the room.
        if (BR_API.GetGameType() == 0) {

            if (PhotonNetwork.CurrentRoom.PlayerCount < 2 && !BR_API.GetBots())
                startGameButton.SetActive(false);
            else
                startGameButton.SetActive(allReady);

        } else {

            //  If game type is practice, no need to wait everyone's ready statement. Master client can start the game whenever he wants.
            startGameButton.SetActive(true);

        }

    }

    //  Setting ready button's image color.
    if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["Ready"])
        readyOnImage.color = readyOnColor;
    else
        readyOnImage.color = Color.gray;

    //  Ready info.
    if (BR_API.GetGameType() == 0)
        readyInfo.SetActive(true);
    else
        readyInfo.SetActive(false);

}

#endif

    #region BUTTONS

    /// <summary>
    /// Connects and logins.
    /// </summary>
    public void LoginButton() {

#if PHOTON_UNITY_NETWORKING

        //  Only connect if we're not connected to the master server.
        if (PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer) {

            //  Getting the player name.
            string playerName = BR_API.GetPlayerName();

            //  If player name is not empty...
            if (!playerName.Equals("")) {

                //  Adding necessary properties to the player's custom properties.
                Hashtable hash = new Hashtable();
                hash.Add("Ready", false);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

                //  Setting photon player's nickname.
                PhotonNetwork.LocalPlayer.NickName = playerName;

                //  Connecting to the server by using above settings.
                PhotonNetwork.ConnectUsingSettings();

                //  Opening the connecting now panel.
                OpenMenu(connectingPanel);

            } else {

                BR_UI_Informer.Instance.Info("PLAYER NAME IS INVALID!");
                Debug.LogError("Player Name is invalid.");

            }

        }

#endif

    }


    /// <summary>
/// Two player quick matchmaking. Finds a 2-player room with space or creates one.
/// Automatically starts when 2 players are connected.
/// </summary>
public void TwoPlayerQuickMatchButton()
{
#if PHOTON_UNITY_NETWORKING

    // Join the lobby if we're not in
    if (!PhotonNetwork.InLobby)
        PhotonNetwork.JoinLobby();

    // Set game type to Race
    BR_API.SetGameType(0);

    // Set laps to 2
    BR_API.SetLapsAmount(2);

    // Set scene (use 1 for testing, adjust as needed)
    BR_API.SetScene(1);

    // Opening the joining random room panel
    OpenMenu(joinRandomRoomPanel);

    // Setting room properties to search for
    Hashtable expectedRoomProperties = new Hashtable();
    expectedRoomProperties.Add("gametype", "Race");
    expectedRoomProperties.Add("laps", 2);

    // Custom properties for lobby visibility
    string[] customPropertiesForLobby = new string[4];
    customPropertiesForLobby[0] = "scene";
    customPropertiesForLobby[1] = "gametype";
    customPropertiesForLobby[2] = "laps";
    customPropertiesForLobby[3] = "password";

    // Custom properties for room
    Hashtable customPropertiesForRoom = new Hashtable();
    customPropertiesForRoom.Add("scene", BR_API.GetScene());
    customPropertiesForRoom.Add("gametype", "Race");
    customPropertiesForRoom.Add("laps", 2);
    customPropertiesForRoom.Add("password", "");

    // Room options for creating a new room if no match found
    RoomOptions roomOptions = new RoomOptions { MaxPlayers = 2 };
    roomOptions.CleanupCacheOnLeave = false;
    roomOptions.IsOpen = true;
    roomOptions.IsVisible = true;
    roomOptions.CustomRoomPropertiesForLobby = customPropertiesForLobby;
    roomOptions.CustomRoomProperties = customPropertiesForRoom;

    // Try to join random room matching criteria, or create new one
    PhotonNetwork.JoinRandomOrCreateRoom(
        expectedRoomProperties,
        2, // Max players in the room to search for
        MatchmakingMode.FillRoom,
        TypedLobby.Default,
        null,
        UnityEngine.Random.Range(1000, 9999).ToString(), // Random room name
        roomOptions
    );

#endif
}


    /// <summary>
    /// Quick matchmaking button. Finds a random room, or creates a new room.
    /// </summary>
    public void QuickMatchingButton() {

#if PHOTON_UNITY_NETWORKING

        //  Join the lobby if we're not in.
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();

        //  Opening the joining random room panel.
        OpenMenu(joinRandomRoomPanel);

        //  Setting room properties.
        Hashtable roomProperties = new Hashtable();
        roomProperties.Add("gametype", "Race");
        roomProperties.Add("laps", 3);

        //  Attempting to join a random room.
        PhotonNetwork.JoinRandomOrCreateRoom(roomProperties);

#endif

    }

    /// <summary>
    /// Opens up the create room panel. Creates a new room with given room options.
    /// </summary>
    public void CreateRoomMenuButton() {

#if PHOTON_UNITY_NETWORKING

        //  Join the lobby if we're not in.
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();

        //  Opening the create room panel.
        OpenMenu(createRoomPanel);

#endif

    }

    /// <summary>
    /// Room listing button.
    /// </summary>
    public void ShowRoomsButton() {

#if PHOTON_UNITY_NETWORKING

        //  Join the lobby if we're not in.
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();

        //  Opening the room list panel.
        OpenMenu(roomListPanel);

#endif

    }

    /// <summary>
    /// Creates a new room with given room options.
    /// </summary>
    public void CreateRoomButton() {

        //  Return if the room name doesn't have at least 4 characters.
        if (roomNameInput.text.Length < 4) {

            BR_UI_Informer.Instance.Info("4 CHARACTERS NEEDED AT LEAST");
            return;

        }

#if PHOTON_UNITY_NETWORKING

        //  Opening the creating room panel.
        OpenMenu(creatingRoomPanel);

        //  Creating new custom properties for lobby.
        string[] customPropertiesForLobby = new string[4];
        customPropertiesForLobby[0] = "scene";
        customPropertiesForLobby[1] = "gametype";
        customPropertiesForLobby[2] = "laps";
        customPropertiesForLobby[3] = "password";

        //  Creating new custom properties for room.
        Hashtable customPropertiesForRoom = new Hashtable();
        customPropertiesForRoom.Add("scene", BR_API.GetScene());
        customPropertiesForRoom.Add("gametype", GetGameTypeName(BR_API.GetGameType()));
        customPropertiesForRoom.Add("laps", BR_API.GetLapsAmount());
        customPropertiesForRoom.Add("password", roomPasswordInput.text.Trim());

        //  Room options. Setting max players, clean objects after player leaves, room is open and visible. And finally setting room's properties.
        RoomOptions options = new RoomOptions { MaxPlayers = (byte)currentMaxPlayers };
        options.CleanupCacheOnLeave = false;
        options.IsOpen = true;
        options.IsVisible = true;
        options.CustomRoomPropertiesForLobby = customPropertiesForLobby;
        options.CustomRoomProperties = customPropertiesForRoom;

        //  Creating the room with given options.
        PhotonNetwork.CreateRoom(roomNameInput.text, options, TypedLobby.Default);

#endif

    }

    /// <summary>
    /// Join lobby.
    /// </summary>
    public void JoinLobby() {

#if PHOTON_UNITY_NETWORKING

        //  Join the lobby if we're not in.
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();

#endif

    }

    /// <summary>
    /// Attempts to join the target room.
    /// </summary>
    /// <param name="roomName"></param>
    public void JoinToRoom(string roomName) {

#if PHOTON_UNITY_NETWORKING

        OpenMenu(joiningPanel);
        PhotonNetwork.JoinRoom(roomName);

#endif

    }

    /// <summary>
    /// Go back button.
    /// </summary>
    public void BackButton() {

#if PHOTON_UNITY_NETWORKING

        //  Leave the room first.
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();

        //  Join the lobby if we're not in.
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();

#endif

    }

    /// <summary>
    /// Cancel and disconnect button.
    /// </summary>
    public void CancelButton() {

#if PHOTON_UNITY_NETWORKING

        PhotonNetwork.Disconnect();

#endif

    }

    /// <summary>
    /// Leave the room button.
    /// </summary>
    public void LeaveRoom() {

#if PHOTON_UNITY_NETWORKING

        //  Leave the room first.
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();

        //  Opening leaving room panel.
        OpenMenu(leavingRoomPanel);

#endif

    }

    /// <summary>
    /// Leave lobby button. Not used...
    /// </summary>
    public void LeaveLobby() {

#if PHOTON_UNITY_NETWORKING

        //  Leaving the lobby. Not used.
        if (PhotonNetwork.InLobby)
            PhotonNetwork.LeaveLobby();

#endif

    }

    /// <summary>
    /// Ready button.
    /// </summary>
    public void ReadyButton() {

#if PHOTON_UNITY_NETWORKING

        //  Getting player's custom properties.
        Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;

        //  Setting the ready statement.
        bool current = (bool)hash["Ready"];
        hash["Ready"] = !current;

        //  And finally setting the player's custom properties.
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

#endif

    }

    /// <summary>
    /// Starts the game by loading the target gameplay scene.
    /// </summary>
    public void StartGameButton() {

#if PHOTON_UNITY_NETWORKING

        //  Opening joining game panel.
        OpenMenu(joiningPanel);

        //  If we're the master client, set room's open and visible statements.
        if (PhotonNetwork.IsMasterClient) {

            //  Getting the room's properties.
            Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            string gameType = (string)roomProperties["gametype"];

            //  If game type is race, room is not visible and not open to other players.
            //  If game type is practice, room is visible and open to other players.
            switch (gameType) {

                case "Race":
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    PhotonNetwork.CurrentRoom.IsVisible = false;
                    break;

                case "Practice":
                    PhotonNetwork.CurrentRoom.IsOpen = true;
                    PhotonNetwork.CurrentRoom.IsVisible = true;
                    break;

            }

            int laps = (int)roomProperties["laps"];
            BR_API.SetLapsAmount((int)laps);

            //  Loading the target gameplay scene.
            PhotonNetwork.LoadLevel(BR_API.GetScene());

        }

#endif

    }

    #endregion

    /// <summary>
    /// Opens the target menu.
    /// </summary>
    /// <param name="activeMenu"></param>
    public void OpenMenu(GameObject activeMenu) {

        connectingPanel.SetActive(false);
        loginPanel.SetActive(false);
        selectionPanel.SetActive(false);
        sceneSelectionPanel.SetActive(false);
        createRoomPanel.SetActive(false);
        creatingRoomPanel.SetActive(false);
        joinRandomRoomPanel.SetActive(false);
        joiningPanel.SetActive(false);
        roomListPanel.SetActive(false);
        playerListPanel.SetActive(false);
        joiningRoomPanel.SetActive(false);
        leavingRoomPanel.SetActive(false);
        roomPasswordPanel.SetActive(false);

        //  Only selected panel will be activated. All other panels will be disabled.
        activeMenu.SetActive(true);

        if (activeMenu && activeMenu == createRoomPanel)
            roomPasswordInput.SetTextWithoutNotify("");

        if (activeMenu && activeMenu == roomPasswordPanel)
            roomPasswordEnterInput.SetTextWithoutNotify("");

        if (activeMenu && activeMenu == roomListPanel)
            ClearSearchRoomFilter();

    }

#if PHOTON_UNITY_NETWORKING

    /// <summary>
    /// Updates cached room list.
    /// </summary>
    /// <param name="roomList"></param>
    private void UpdateCachedRoomList(List<RoomInfo> roomList) {

        foreach (RoomInfo info in roomList) {

            // Remove room from cached room list if it got closed, became invisible or was marked as removed
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList) {

                if (cachedRoomList.ContainsKey(info.Name))
                    cachedRoomList.Remove(info.Name);

                continue;

            }

            // Update cached room info
            if (cachedRoomList.ContainsKey(info.Name))
                cachedRoomList[info.Name] = info;
            else
                cachedRoomList.Add(info.Name, info);

        }

    }

    /// <summary>
    /// Listing all rooms.
    /// </summary>
    /// <param name="roomList"></param>
    private void UpdateRoomListView(List<RoomInfo> roomList) {

        bool foundRoom = false;

        foreach (RoomInfo info in cachedRoomList.Values) {

            foundRoom = true;
            GameObject entry = Instantiate(roomListEntryPrefab);
            entry.transform.SetParent(roomListContent.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<BR_UI_RoomListLine>().Initialize(info.Name, GetSceneName((int)info.CustomProperties["scene"]), (string)info.CustomProperties["gametype"], (int)info.CustomProperties["laps"], (byte)info.PlayerCount, (byte)info.MaxPlayers, (string)info.CustomProperties["password"]);
            roomListEntries.Add(info.Name, entry);

        }

        noActiveRooms.SetActive(!foundRoom);

    }

    /// <summary>
    /// Listing all players.
    /// </summary>
    /// <param name="playerList"></param>
    private void UpdatePlayerListView(List<Player> playerList) {

        foreach (Player player in playerList) {

            BR_UI_PlayerListLine entry = Instantiate(playerListEntryPrefab).gameObject.GetComponent<BR_UI_PlayerListLine>();
            entry.transform.SetParent(playerListContent.transform);
            entry.transform.localScale = Vector3.one;
            entry.Register(player.NickName);
            entry.SetPlayerReady((bool)player.CustomProperties["Ready"]);
            playerListEntries.Add(player.NickName, entry);

        }

    }

    /// <summary>
    /// Clears the cache for room list.
    /// </summary>
    private void ClearRoomListView() {

        foreach (GameObject entry in roomListEntries.Values)
            Destroy(entry.gameObject);

        roomListEntries.Clear();

    }

    /// <summary>
    /// Clears the cache for players in the room.
    /// </summary>
    private void ClearPlayerListView() {

        foreach (BR_UI_PlayerListLine entry in playerListEntries.Values)
            Destroy(entry.gameObject);

        playerListEntries.Clear();

    }

    /// <summary>
    /// Updates the connection status and online players text every frame.
    /// </summary>
    private void Update() {

        connectionStatusText.text = "Connection Status: " + PhotonNetwork.NetworkClientState;

        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InLobby && PhotonNetwork.CountOfPlayers >= 1)
            totalOnlinePlayersText.text = "Online Players: " + PhotonNetwork.CountOfPlayers.ToString();
        else
            totalOnlinePlayersText.text = "Online Players: -";

    }

#endif

    /// <summary>
    /// Kicks the target player by using his nickname.
    /// </summary>
    /// <param name="playerName"></param>
    public void KickPlayerPanel(string playerName) {

        kickPlayerName = playerName;
        kickPlayerPanel.SetActive(true);
        kickPlayerPanel.GetComponentInChildren<TextMeshProUGUI>().text = "Kick Player " + playerName + "?";

    }

    /// <summary>
    /// Kicks the player.
    /// </summary>
    public void KickPlayer() {

#if PHOTON_UNITY_NETWORKING
        photonView.RPC("KickPlayerRPC", RpcTarget.All, kickPlayerName);
#endif

        kickPlayerPanel.SetActive(false);

    }

#if PHOTON_UNITY_NETWORKING

    /// <summary>
    /// Kicks the target player. If kicked player's nickname is our player, leave the room.
    /// </summary>
    /// <param name="playerName"></param>
    [PunRPC]
    public void KickPlayerRPC(string playerName) {

        if (playerName != PhotonNetwork.LocalPlayer.NickName)
            return;

        PhotonNetwork.LeaveRoom();
        BR_UI_Informer.Instance.Info("YOU HAVE BEEN KICKED FROM THE ROOM!");

    }

#endif

    /// <summary>
    /// Sets the region by using dropdown's value as text.
    /// </summary>
    /// <param name="dropdown"></param>
    public void SelectRegion(TMP_Dropdown dropdown) {

#if PHOTON_UNITY_NETWORKING

        if (dropdown.options[dropdown.value].text != "Auto")
            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = dropdown.options[dropdown.value].text;
        else
            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "";

#endif

    }

    /// <summary>
    /// Handles disabling the Photon Lobby Manager.
    /// </summary>
    public override void OnDisable() {

        base.OnDisable();

    }

    /// <summary>
    /// Handles destroying the Photon Lobby Manager.
    /// </summary>
    private void OnDestroy() {

#if PHOTON_UNITY_NETWORKING

        //  Removing callback.
        PhotonNetwork.RemoveCallbackTarget(this);

#endif

    }

    /// <summary>
    /// Gets the scene name by using build index.
    /// </summary>
    /// <param name="BuildIndex"></param>
    /// <returns></returns>
    private static string GetSceneName(int BuildIndex) {

        string path = SceneUtility.GetScenePathByBuildIndex(BuildIndex);
        int slash = path.LastIndexOf('/');
        string name = path.Substring(slash + 1);
        int dot = name.LastIndexOf('.');
        return name.Substring(0, dot);

    }

    /// <summary>
    /// Gets the game type name as string.
    /// </summary>
    /// <param name="gameTypeIndex"></param>
    /// <returns></returns>
    private static string GetGameTypeName(int gameTypeIndex) {

        switch (gameTypeIndex) {

            case 0:
                return "Race";

            case 1:
                return "Practice";

            default:
                break;
        }

        return "";

    }

    /// <summary>
    /// Sets the maximum player count by using toggles.
    /// </summary>
    /// <param name="toggle"></param>
    public void SetMaxPlayersByToggle(Toggle toggle) {

        switch (toggle.name) {

            case "2":

                currentMaxPlayers = 2;

                maxPlayers2.SetIsOnWithoutNotify(true);
                maxPlayers3.SetIsOnWithoutNotify(false);
                maxPlayers4.SetIsOnWithoutNotify(false);
                maxPlayers5.SetIsOnWithoutNotify(false);
                maxPlayers6.SetIsOnWithoutNotify(false);
                maxPlayers7.SetIsOnWithoutNotify(false);

                break;

            case "3":

                currentMaxPlayers = 3;

                maxPlayers2.SetIsOnWithoutNotify(false);
                maxPlayers3.SetIsOnWithoutNotify(true);
                maxPlayers4.SetIsOnWithoutNotify(false);
                maxPlayers5.SetIsOnWithoutNotify(false);
                maxPlayers6.SetIsOnWithoutNotify(false);
                maxPlayers7.SetIsOnWithoutNotify(false);

                break;

            case "4":

                currentMaxPlayers = 4;

                maxPlayers2.SetIsOnWithoutNotify(false);
                maxPlayers3.SetIsOnWithoutNotify(false);
                maxPlayers4.SetIsOnWithoutNotify(true);
                maxPlayers5.SetIsOnWithoutNotify(false);
                maxPlayers6.SetIsOnWithoutNotify(false);
                maxPlayers7.SetIsOnWithoutNotify(false);

                break;

            case "5":

                currentMaxPlayers = 5;

                maxPlayers2.SetIsOnWithoutNotify(false);
                maxPlayers3.SetIsOnWithoutNotify(false);
                maxPlayers4.SetIsOnWithoutNotify(false);
                maxPlayers5.SetIsOnWithoutNotify(true);
                maxPlayers6.SetIsOnWithoutNotify(false);
                maxPlayers7.SetIsOnWithoutNotify(false);

                break;

            case "6":

                currentMaxPlayers = 6;

                maxPlayers2.SetIsOnWithoutNotify(false);
                maxPlayers3.SetIsOnWithoutNotify(false);
                maxPlayers4.SetIsOnWithoutNotify(false);
                maxPlayers5.SetIsOnWithoutNotify(false);
                maxPlayers6.SetIsOnWithoutNotify(true);
                maxPlayers7.SetIsOnWithoutNotify(false);

                break;

            case "7":

                currentMaxPlayers = 7;

                maxPlayers2.SetIsOnWithoutNotify(false);
                maxPlayers3.SetIsOnWithoutNotify(false);
                maxPlayers4.SetIsOnWithoutNotify(false);
                maxPlayers5.SetIsOnWithoutNotify(false);
                maxPlayers6.SetIsOnWithoutNotify(false);
                maxPlayers7.SetIsOnWithoutNotify(true);

                break;

        }

    }

    public void EnterRoomPassword(string roomPassword, Action<bool> callback) {

        roomPasswordEnterInput.onEndEdit.RemoveAllListeners();

        // Add a listener to the onValueChanged event
        roomPasswordEnterInput.onEndEdit.AddListener((text) => {

            if (roomPasswordEnterInput.text.Trim() == roomPassword.Trim())
                callback?.Invoke(true);
            else
                callback?.Invoke(false);

        });

    }

    public void SearchRoom() {

        if (searchRoomNameInput.text == "")
            return;

        List<string> roomNames = new List<string>();

        foreach (string info in cachedRoomList.Keys) {

            if (info != null)
                roomNames.Add(info);

        }

        // Find rooms containing the input (case-insensitive)
        List<string> foundRooms = roomNames.FindAll(room => room.ToLower().Contains(searchRoomNameInput.text.ToLower()));
        List<RoomInfo> activeTheseRooms = new List<RoomInfo>();

        foreach (string info in cachedRoomList.Keys) {

            for (int i = 0; i < foundRooms.Count; i++) {

                if (info != null && info.Contains(foundRooms[i])) {

                    if (cachedRoomList.TryGetValue(info, out RoomInfo ri))
                        activeTheseRooms.Add(ri);

                }

            }

        }

        foreach (string info in roomListEntries.Keys) {

            if (roomListEntries.TryGetValue(info, out GameObject ri1))
                ri1.SetActive(false);

            for (int i = 0; i < activeTheseRooms.Count; i++) {

                if (info.Contains(foundRooms[i])) {

                    if (roomListEntries.TryGetValue(info, out GameObject ri2))
                        ri2.SetActive(true);

                }

            }

        }

        //// Display results
        //if (!string.IsNullOrEmpty(foundRoom))
        //    JoinToRoom(foundRoom);
        //else
        //    BR_UI_Informer.Instance.Info("Room Couldn't Found!");

    }

    public void ClearSearchRoomFilter() {

        searchRoomNameInput.text = "";

        foreach (string info in roomListEntries.Keys) {

            if (roomListEntries.TryGetValue(info, out GameObject ri1))
                ri1.SetActive(true);

        }

    }

    /// <summary>
    /// Disconnects from the server.
    /// </summary>
    public void DisconnectButton() {

#if PHOTON_UNITY_NETWORKING
        PhotonNetwork.Disconnect();
#endif

    }

}

#else

public class BR_PhotonLobbyManager : MonoBehaviour
{
}

#endif