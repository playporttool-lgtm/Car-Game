#if RCCP_PHOTON && PHOTON_UNITY_NETWORKING
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class RCCP_PhotonManager : MonoBehaviourPunCallbacks {

    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static RCCP_PhotonManager Instance;

    /// <summary>
    /// Gameplay scene name to load target level.
    /// </summary>
    public string gameplaySceneName = "Gameplay Scene Name";

    [Header("UI Elements")]

    /// <summary>
    /// Input field for player nickname.
    /// </summary>
    public InputField nickPanel;

    /// <summary>
    /// Panel for browsing available rooms.
    /// </summary>
    public GameObject browseRoomsPanel;

    /// <summary>
    /// Parent object containing room entries.
    /// </summary>
    public GameObject roomsContent;

    /// <summary>
    /// Panel displaying chat messages.
    /// </summary>
    public GameObject chatLinesPanel;

    /// <summary>
    /// Parent object containing chat line entries.
    /// </summary>
    public GameObject chatLinesContent;

    /// <summary>
    /// UI element shown when no rooms are available.
    /// </summary>
    public GameObject noRoomsYet;

    /// <summary>
    /// Button to create a new room.
    /// </summary>
    public GameObject createRoomButton;

    /// <summary>
    /// Button to exit the current room.
    /// </summary>
    public GameObject exitRoomButton;

    [Header("UI Texts")]

    /// <summary>
    /// Status text showing connection progress.
    /// </summary>
    public Text status;

    /// <summary>
    /// Text showing total online players.
    /// </summary>
    public Text totalOnlinePlayers;

    /// <summary>
    /// Text showing total available rooms.
    /// </summary>
    public Text totalRooms;

    /// <summary>
    /// Text displaying current region.
    /// </summary>
    public Text region;

    [Header("Other Prefabs")]

    /// <summary>
    /// Prefab for room entry UI.
    /// </summary>
    public RCCP_PhotonUIRoom roomPrefab;

    /// <summary>
    /// Prefab for chat line UI.
    /// </summary>
    public RCCP_PhotonUIChatLine chatLinePrefab;

    /// <summary>
    /// Cached informer singleton.
    /// </summary>
    private RCCP_UI_Informer uiInformer;

    /// <summary>
    /// Cached room infos.
    /// </summary>
    private Dictionary<string, RoomInfo> cachedRoomList;

    /// <summary>
    /// Active room entry GameObjects.
    /// </summary>
    private Dictionary<string, GameObject> roomListEntries;

    private void Awake() {

        if (Instance == null) {

            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);

        } else {

            Destroy(gameObject);
            return;

        }

    }

    private void Start() {

        uiInformer = RCCP_UI_Informer.Instance;

        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListEntries = new Dictionary<string, GameObject>();

        status.text = "Ready to connect";

        nickPanel.text = "New Player " + Random.Range(0, 99999).ToString();

    }

    /// <summary>
    /// Initiates connection to Photon servers with the entered nickname.
    /// </summary>
    public void Connect() {

        if (nickPanel.text.Length < 4) {

            if (uiInformer)
                uiInformer.Display("4 Characters Needed At Least");

            return;

        }

        if (!PhotonNetwork.IsConnected) {

            ConnectToServer();

        } else {

            nickPanel.gameObject.SetActive(false);

        }

    }

    /// <summary>
    /// Sends request to connect using Photon settings.
    /// </summary>
    private void ConnectToServer() {

        status.text = "Connecting to server";

        PhotonNetwork.NickName = nickPanel.text;
        PhotonNetwork.ConnectUsingSettings();

        nickPanel.gameObject.SetActive(false);

        uiInformer.Display("Connecting to server");

    }

    /// <summary>
    /// Called when connected to Master server; joins lobby.
    /// </summary>
    public override void OnConnectedToMaster() {

        status.text = "Entering to lobby";

        if (uiInformer)
            uiInformer.Display("Connected to server, entering lobby");

        PhotonNetwork.JoinLobby();

    }

    /// <summary>
    /// Called upon joining the lobby; shows room browser.
    /// </summary>
    public override void OnJoinedLobby() {

        status.text = "Entered lobby";

        nickPanel.gameObject.SetActive(false);
        browseRoomsPanel.SetActive(true);
        createRoomButton.SetActive(true);
        exitRoomButton.SetActive(false);
        chatLinesPanel.SetActive(false);

        UpdateLobbyStats();

        if (uiInformer)
            uiInformer.Display("Entered lobby");

    }

    /// <summary>
    /// Called when the room list in the lobby is updated; diffs and updates UI.
    /// </summary>
    public override void OnRoomListUpdate(List<RoomInfo> roomList) {

        var updated = new HashSet<string>();

        foreach (RoomInfo info in roomList) {

            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList) {

                if (cachedRoomList.ContainsKey(info.Name)) {

                    cachedRoomList.Remove(info.Name);

                    if (roomListEntries.TryGetValue(info.Name, out var oldEntry)) {

                        Destroy(oldEntry);
                        roomListEntries.Remove(info.Name);

                    }

                }

                continue;

            }

            if (cachedRoomList.ContainsKey(info.Name)) {

                cachedRoomList[info.Name] = info;

                if (roomListEntries.TryGetValue(info.Name, out var entry)) {

                    entry.GetComponent<RCCP_PhotonUIRoom>().Check(
                        info.Name,
                        info.PlayerCount + " / " + info.MaxPlayers
                    );

                }

            } else {

                cachedRoomList.Add(info.Name, info);

                var newEntry = Instantiate(roomPrefab.gameObject);
                newEntry.transform.SetParent(roomsContent.transform);
                newEntry.transform.localScale = Vector3.one;
                newEntry.GetComponent<RCCP_PhotonUIRoom>().Check(
                    info.Name,
                    info.PlayerCount + " / " + info.MaxPlayers
                );
                roomListEntries.Add(info.Name, newEntry);

            }

            updated.Add(info.Name);

        }

        noRoomsYet.SetActive(roomListEntries.Count == 0);

        UpdateLobbyStats();

    }

    /// <summary>
    /// Updates the lobby statistics (players, rooms, region).
    /// </summary>
    private void UpdateLobbyStats() {

        totalOnlinePlayers.text = "Total Online Players: " + PhotonNetwork.CountOfPlayers;
        totalRooms.text = "Total Online Rooms: " + PhotonNetwork.CountOfRooms;
        region.text = "Region: " + PhotonNetwork.CloudRegion;

    }

    /// <summary>
    /// Called when joining a room; transitions from lobby to in-room UI.
    /// </summary>
    public override void OnJoinedRoom() {

        status.text = "Joined room";

        nickPanel.gameObject.SetActive(false);
        browseRoomsPanel.SetActive(false);
        createRoomButton.SetActive(false);
        exitRoomButton.SetActive(true);
        chatLinesPanel.SetActive(true);

        if (uiInformer)
            uiInformer.Display("Joined room, you can spawn your vehicle from 'Options' menu");

        LoadLevel(gameplaySceneName);

    }

    /// <summary>
    /// Called when creating a room succeeds; same behavior as joining.
    /// </summary>
    public override void OnCreatedRoom() {

        status.text = "Created room";

        nickPanel.gameObject.SetActive(false);
        browseRoomsPanel.SetActive(false);
        createRoomButton.SetActive(false);
        exitRoomButton.SetActive(true);
        chatLinesPanel.SetActive(true);

        if (uiInformer)
            uiInformer.Display("Created room, you can spawn your vehicle from 'Options' menu");

        LoadLevel(gameplaySceneName);

    }

    /// <summary>
    /// Creates or joins a room with default settings.
    /// </summary>
    public void CreateRoom() {

        var options = new RoomOptions {

            MaxPlayers = 4,
            IsOpen = true,
            IsVisible = true

        };

        PhotonNetwork.JoinOrCreateRoom(
            "New Room " + Random.Range(0, 999),
            options,
            TypedLobby.Default
        );

    }

    /// <summary>
    /// Joins the specified room by name.
    /// </summary>
    /// <param name="room">UI wrapper containing the room name.</param>
    public void JoinSelectedRoom(RCCP_PhotonUIRoom room) {

        PhotonNetwork.JoinRoom(room.roomName.text);

    }

    /// <summary>
    /// Sends a chat message and clears the input.
    /// </summary>
    /// <param name="inputField">TextMeshPro input field.</param>
    public void Chat(InputField inputField) {

        var message = inputField.text.Trim();
        if (string.IsNullOrEmpty(message)) {

            return;

        }

        photonView.RPC(
            "RPCChat",
            RpcTarget.AllBuffered,
            PhotonNetwork.NickName,
            message
        );

        inputField.text = "";

    }

    /// <summary>
    /// RPC handler: displays incoming chat lines and trims old messages.
    /// </summary>
    [PunRPC]
    public void RPCChat(string nickName, string text) {

        var newLineGO = Instantiate(
            chatLinePrefab.gameObject,
            chatLinesContent.transform
        );

        var newChatLine = newLineGO.GetComponent<RCCP_PhotonUIChatLine>();
        newChatLine.Line($"{nickName} : {text}");

        var chatLines = chatLinesContent
            .GetComponentsInChildren<RCCP_PhotonUIChatLine>();

        if (chatLines.Length > 7) {

            Destroy(chatLines[0].gameObject);

        }

        var scrollRect = chatLinesPanel
            .GetComponentInParent<ScrollRect>();
        if (scrollRect != null) {

            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;

        }

    }

    /// <summary>
    /// Leaves the current room.
    /// </summary>
    public void ExitRoom() {

        PhotonNetwork.LeaveRoom();

    }

    /// <summary>
    /// Called when left room; returns to lobby UI.
    /// </summary>
    public override void OnLeftRoom() {

        status.text = "Exited room";

        nickPanel.gameObject.SetActive(false);
        browseRoomsPanel.SetActive(true);
        createRoomButton.SetActive(true);
        exitRoomButton.SetActive(false);
        chatLinesPanel.SetActive(false);

    }

    /// <summary>
    /// Leaves the lobby.
    /// </summary>
    public void ExitLobby() {

        PhotonNetwork.LeaveLobby();

    }

    /// <summary>
    /// Called when left lobby; returns to initial UI.
    /// </summary>
    public override void OnLeftLobby() {

        status.text = "Exited to lobby";

        nickPanel.gameObject.SetActive(true);
        browseRoomsPanel.SetActive(false);
        createRoomButton.SetActive(false);
        exitRoomButton.SetActive(false);
        chatLinesPanel.SetActive(false);

    }

    /// <summary>
    /// Called when disconnected; resets UI to initial state.
    /// </summary>
    public override void OnDisconnected(DisconnectCause cause) {

        status.text = "Disconnected: " + cause;

        if (uiInformer)
            uiInformer.Display("Disconnected: " + cause);

        nickPanel.gameObject.SetActive(true);
        browseRoomsPanel.SetActive(false);
        createRoomButton.SetActive(false);
        exitRoomButton.SetActive(false);
        chatLinesPanel.SetActive(false);

    }

    /// <summary>
    /// Handles networked scene loading.
    /// </summary>
    /// <param name="level">Name of the scene to load.</param>
    public void LoadLevel(string level) {

        PhotonNetwork.LoadLevel(level);

    }

    /// <summary>
    /// Called when room creation fails; displays error.
    /// </summary>
    public override void OnCreateRoomFailed(short returnCode, string message) {

        status.text = $"Create room failed: {message}";

        if (uiInformer)
            uiInformer.Display($"Create room failed: {message}");

        browseRoomsPanel.SetActive(true);
        createRoomButton.SetActive(true);

    }

    /// <summary>
    /// Called when joining room fails; displays error.
    /// </summary>
    public override void OnJoinRoomFailed(short returnCode, string message) {

        status.text = $"Join room failed: {message}";

        if (uiInformer)
            uiInformer.Display($"Join room failed: {message}");

        browseRoomsPanel.SetActive(true);
        createRoomButton.SetActive(true);

    }

}
#endif
