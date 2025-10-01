//----------------------------------------------
//			Burnout Racers
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

#if PHOTON_UNITY_NETWORKING
using Photon.Realtime;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
#endif

/// <summary>
/// Manages overall gameplay. Spawns player/AI vehicles, observes them, handles countdown for start, and finishes the race.
/// </summary>
public class BR_GameplayManager : MonoBehaviour {

    #region Singleton
    /// <summary>
    /// Singleton instance of BR_GameplayManager.
    /// </summary>
    private static BR_GameplayManager instance;

    /// <summary>
    /// Provides access to the singleton instance of BR_GameplayManager.
    /// </summary>
    public static BR_GameplayManager Instance {
        get {
            if (instance == null)
                instance = FindFirstObjectByType<BR_GameplayManager>();
            return instance;
        }
    }
    #endregion

#if PHOTON_UNITY_NETWORKING

    /// <summary>
    /// PhotonView component for network communication.
    /// </summary>
    private PhotonView photonViewComponent;

#endif

    /// <summary>
    /// The player vehicle managed by the gameplay manager.
    /// </summary>
    public BR_PlayerManager currentPlayerCar;

    /// <summary>
    /// Index of the selected player vehicle in the main menu.
    /// </summary>
    private int selectedPlayerCarIndex = 0;

    /// <summary>
    /// Array of spawn points where players and AI can be spawned.
    /// </summary>
    public Transform[] spawnPoints;

    /// <summary>
    /// Indicates whether the game is in offline mode.
    /// </summary>
    public bool offlineMode = false;

    /// <summary>
    /// Enum representing the current game state.
    /// </summary>
    public enum GameState { Idle, Intro, Countdown, Race }

    /// <summary>
    /// The current game state.
    /// </summary>
    public GameState gameState = GameState.Idle;

    /// <summary>
    /// Enum representing the game type. Players earn money and points only in the Race mode.
    /// </summary>
    public enum GameType { Race, Practice }

    /// <summary>
    /// The current game type.
    /// </summary>
    public GameType gameType = GameType.Race;

    /// <summary>
    /// Indicates whether bots are enabled.
    /// </summary>
    public bool botsEnabled = false;

    /// <summary>
    /// Indicates whether the race has started.
    /// </summary>
    public bool raceStarted = false;

    /// <summary>
    /// Indicates whether the game is currently paused.
    /// </summary>
    private bool paused = false;

    /// <summary>
    /// If true, the headlights of the vehicle will be enabled (night mode).
    /// </summary>
    public bool isNight = false;

    /// <summary>
    /// The target number of laps to complete.
    /// </summary>
    public int targetLap = 2;

    /// <summary>
    /// The current lap count.
    /// </summary>
    public int currentLap = -1;

    /// <summary>
    /// The current position of the player in the race.
    /// </summary>
    public int currentPosition = -1;

    /// <summary>
    /// The position in which the player finished the race.
    /// </summary>
    public int finishedPosition = -1;

    /// <summary>
    /// The number of race points earned after finishing the race.
    /// </summary>
    public int earnedRacePoints = 0;

    /// <summary>
    /// The race prize earned after finishing the race.
    /// </summary>
    public int earnedRacePrize = 0;

    /// <summary>
    /// The time taken to complete the current lap.
    /// </summary>
    public float lapTime = -1f;

    /// <summary>
    /// The total time taken to complete the race.
    /// </summary>
    public float totalTime = -1f;

    /// <summary>
    /// The prize awarded for finishing in first place.
    /// </summary>
    public int firstPrize = 20000;

    /// <summary>
    /// The prize awarded for finishing in second place.
    /// </summary>
    public int secondPrize = 10000;

    /// <summary>
    /// The prize awarded for finishing in third place.
    /// </summary>
    public int thirdPrize = 5000;

    #region Events

    /// <summary>
    /// Delegate for the event triggered when the player vehicle spawns.
    /// </summary>
    /// <param name="Player">The player manager of the spawned player.</param>
    public delegate void onPlayerSpawned(BR_PlayerManager Player);
    public static event onPlayerSpawned OnLocalPlayerSpawned;

    /// <summary>
    /// Delegate for the event triggered when the race starts.
    /// </summary>
    public delegate void onRaceStarted();
    public static event onRaceStarted OnLocalRaceStarted;

    /// <summary>
    /// Delegate for the event triggered when the countdown starts.
    /// </summary>
    public delegate void onCountdownStarted();
    public static event onCountdownStarted OnLocalCountdownStarted;

    /// <summary>
    /// Delegate for the event triggered when the race finishes.
    /// </summary>
    /// <param name="player">The player manager of the finished player.</param>
    public delegate void onRaceFinished(BR_PlayerManager player);
    public static event onRaceFinished OnLocalRaceFinished;

    /// <summary>
    /// Delegate for the event triggered when the race is paused.
    /// </summary>
    /// <param name="state">The paused state of the race.</param>
    public delegate void onRacePaused(bool state);
    public static event onRacePaused OnLocalRacePaused;

    #endregion

    /// <summary>
    /// Dictionary storing all spawned racers, indexed by their unique IDs.
    /// </summary>
    public Dictionary<int, BR_PlayerManager> allRacers = new Dictionary<int, BR_PlayerManager>();

    /// <summary>
    /// List of all racers sorted by their positions.
    /// </summary>
    public List<BR_PlayerManager> allRacersInOrder = new List<BR_PlayerManager>();

    /// <summary>
    /// List of all network players who have finished the race, sorted by their positions.
    /// </summary>
    public List<BR_PlayerManager> allNetworkPlayersFinished = new List<BR_PlayerManager>();

    /// <summary>
    /// List of all spawned bots.
    /// </summary>
    public List<BR_PlayerManager> allBots = new List<BR_PlayerManager>();

    /// <summary>
    /// The total number of players ready before starting the race.
    /// </summary>
    public int totalReadyPlayers = 0;

    private void Awake() {

#if PHOTON_UNITY_NETWORKING
        photonViewComponent = GetComponent<PhotonView>();
#endif

#if PHOTON_UNITY_NETWORKING
        offlineMode = !BR_API.IsMultiplayer();
#else
        offlineMode = true;
#endif

#if PHOTON_UNITY_NETWORKING
        if (!offlineMode)
            GetRoomProperties();
        else
            GetLocalProperties();
#else
        GetLocalProperties();
#endif

        // If the game type is practice, set the target lap to -1, which is infinite.
        if (gameType == GameType.Practice)
            targetLap = -1;

        // Setting the quality level.
        int qualityLevel = PlayerPrefs.GetInt("QualityLevel", 1);
        QualitySettings.SetQualityLevel(qualityLevel);

        // Resetting the current scene's best lap times.
        BR_API.ResetBestCurrentLapTime(SceneManager.GetActiveScene().name);
        BR_API.ResetBestCurrentTimeTotal(SceneManager.GetActiveScene().name);

        // If it's multiplayer, enable the waiting for other players panel before everyone is ready.
        BR_UI_CanvasManager.Instance.waitingForOtherPlayers.SetActive(!offlineMode);

        // Make sure time scale is set, and AudioListener is not paused.
        Time.timeScale = BR_Settings.Instance.timeScale;
        AudioListener.pause = false;

        AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume", 1f);       // Setting volume.
        selectedPlayerCarIndex = BR_API.GetVehicle();       // Getting the index of the selected vehicle on the main menu.

        // Current lap is 1.
        currentLap = 1;

        // Current time is 0.
        totalTime = 0f;

    }

    private IEnumerator Start() {

        // Delay before starting the race.
        yield return new WaitForSeconds(1f);

#if PHOTON_UNITY_NETWORKING

        // If it's online, check connection and room properties.
        if (!offlineMode)
            StartCoroutine(CheckPhotonConnectionAndRoom());

#else

        offlineMode = true;

#endif

#if !PHOTON_UNITY_NETWORKING

        // Spawning the local player.
        SpawnPlayerOffline();

        // Spawning bots if enabled.
        if (botsEnabled)
            SpawnBotsOffline();

#else

        // Spawning the local player or online player.
        if (!offlineMode)
            SpawnPlayerOnline();
        else
            SpawnPlayerOffline();

        // Spawning bots if enabled.
        if (botsEnabled) {

            if (offlineMode)
                SpawnBotsOffline();
            else
                SpawnBotsOnline();

        }

#endif

        // Starting the race if the game type is practice. If the game type is race, the master client must wait for other players to load the scene before starting the race. The master client will send an RPC to everyone to start the countdown and race.
        if (gameType == GameType.Practice)
            StartCoroutine(StartCountdown());

        // Disabling the fixed camera mode before the race starts. We won't use this mode until you finish the race. It will be enabled and in use when the player finishes the race.
        RCCP_SceneManager.Instance.activePlayerCamera.useFixedCameraMode = false;

    }

#if PHOTON_UNITY_NETWORKING

    public void OnEnable() {

        // Adding callback target for Photon.
#if PHOTON_UNITY_NETWORKING
        PhotonNetwork.AddCallbackTarget(this);
#endif

    }

    private void GetRoomProperties() {

        //// If not connected to the server, or not in a room, return to the main menu.
        //if (!PhotonNetwork.IsConnected) {

        //    Debug.LogError("Not connected to the server! Returning back to the lobby...");
        //    SceneManager.LoadScene(0);
        //    enabled = false;
        //    return;

        //}

        //// If not connected to the server, or not in a room, return to the main menu.
        //if (!PhotonNetwork.InRoom) {

        //    Debug.LogError("Not in a room! Returning back to the lobby...");
        //    SceneManager.LoadScene(0);
        //    enabled = false;
        //    return;

        //}

        //  Getting the room's properties.
        ExitGames.Client.Photon.Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

        //  Game type.
        string gameTypeString = (string)roomProperties["gametype"];

        //  Setting the game type according to the room properties.
        switch (gameTypeString) {

            case "Race":
                BR_API.SetGameType(0);
                gameType = GameType.Race;
                break;

            case "Practice":
                BR_API.SetGameType(1);
                gameType = GameType.Practice;
                break;

        }

        //  Getting the room's scene name.
        int sceneIndex = (int)roomProperties["scene"];

        BR_API.SetScene(sceneIndex);

        //  Laps.
        int laps = (int)roomProperties["laps"];

        //  Setting target laps.
        BR_API.SetLapsAmount((int)laps);

        // Getting the selected lap amount.
        targetLap = BR_API.GetLapsAmount();

        // Bots enabled?
        botsEnabled = BR_API.GetBots();

    }

    /// <summary>
    /// Checking the connection before the game starts.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckPhotonConnectionAndRoom() {

        // Delay.
        yield return new WaitForSeconds(.5f);

        // If not connected to the server, or not in a room, return to the main menu.
        if (!PhotonNetwork.IsConnectedAndReady) {

            Debug.LogError("Not connected to the server! Returning back to the lobby...");
            SceneManager.LoadScene(0);
            enabled = false;
            yield break;

        }

        // If not connected to the server, or not in a room, return to the main menu.
        if (!PhotonNetwork.InRoom) {

            Debug.LogError("Not in a room! Returning back to the lobby...");
            SceneManager.LoadScene(0);
            enabled = false;
            yield break;

        }

        // Sending RPC to the master client which tells them this player is ready.
        if (gameType == GameType.Race)
            photonViewComponent.RPC("PlayerJoinedAndReadyRPC", RpcTarget.MasterClient);

        yield return null;

    }

#endif

    private void GetLocalProperties() {

        // If it's offline mode, set game type to practice. Race mode is locked for single-player because we don't want to let players earn free points. 
        if (offlineMode)
            BR_API.SetGameType(1);

        // Getting the saved game type. 0 is race, 1 is practice.
        switch (BR_API.GetGameType()) {

            case 0:
                gameType = GameType.Race;
                break;

            case 1:
                gameType = GameType.Practice;
                break;

        }

        // Getting the selected lap amount.
        targetLap = BR_API.GetLapsAmount();

        // Bots enabled?
        botsEnabled = BR_API.GetBots();

    }

    /// <summary>
    /// Spawns offline bots.
    /// </summary>
    private void SpawnBotsOffline() {

        // Selected bots amount.
        int botsAmount = BR_API.GetBotsAmount();

        // Getting random indexes to spawn.
        List<int> randomIndexes = GetRandomBotIndexes(BR_BotCars.Instance.AICars.Length);

        // Looping the spawn points.
        for (int i = 0; i < spawnPoints.Length; i++) {

            // If index is 0, it's the player's. Continue.
            if (i == 0)
                continue;

            // Break the loop if the index is out of bounds.
            if (i > botsAmount)
                break;

            // Random unique index.
            int randomIndex;

            if (i < randomIndexes.Count)
                randomIndex = randomIndexes[i];
            else
                randomIndex = randomIndexes[i - (randomIndexes.Count - 1)];

            // Spawning the vehicle.
            RCCP_CarController AICar = Instantiate(BR_BotCars.Instance.AICars[randomIndex].car, spawnPoints[i].position, spawnPoints[i].rotation).GetComponent<RCCP_CarController>();

            // Setting headlights.
            AICar.Lights.lowBeamHeadlights = isNight;

#if PHOTON_UNITY_NETWORKING
            // Searching for Photon scripts. If found, destroy them because we don't need them for offline mode.
            RCCP_PhotonSync photonSync = AICar.GetComponent<RCCP_PhotonSync>();

            if (photonSync)
                Destroy(photonSync);

            PhotonView photonView = AICar.GetComponent<PhotonView>();

            if (photonView)
                Destroy(photonView);
#endif

            // Setting canControl bool of the vehicle to false when spawned. We'll enable it after the race starts.
            AICar.canControl = false;

            // Adding spawned bot to the list.
            allBots.Add(AICar.GetComponent<BR_PlayerManager>());

            // Calling an event when spawned.
            OfflinePlayerSpawned(AICar.GetComponent<BR_PlayerManager>());

        }

        // Getting random nicknames.
        List<string> randomNicknames = GetRandomNicknames(BR_BotCars.Instance.botNicknames, allBots.Count);

        // Assigning random nicknames to all bots.
        for (int i = 0; i < allBots.Count; i++)
            allBots[i].nickName = randomNicknames[i];

    }

#if PHOTON_UNITY_NETWORKING

    /// <summary>
    /// Spawns online bots.
    /// </summary>
    private void SpawnBotsOnline() {

        // Return if we're not the master client.
        if (!PhotonNetwork.IsMasterClient)
            return;

        // Getting random indexes to spawn.
        List<int> randomIndexes = GetRandomBotIndexes(BR_BotCars.Instance.AICars.Length);

        // Looping the existing players.
        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++) {

            // If index is < player count, continue.
            if (i < PhotonNetwork.CurrentRoom.PlayerCount)
                continue;

            // Random unique index.
            int randomIndex;

            if (i < randomIndexes.Count)
                randomIndex = randomIndexes[i];
            else
                randomIndex = randomIndexes[i - (randomIndexes.Count - 1)];

            // Spawning the vehicle.
            RCCP_CarController AICar = PhotonNetwork.Instantiate("AI Cars/" + BR_BotCars.Instance.AICars[randomIndex].car.transform.name, spawnPoints[i].position, spawnPoints[i].rotation).GetComponent<RCCP_CarController>();

            // Setting canControl bool of the vehicle to false when spawned. We'll enable it after the race starts.
            AICar.canControl = false;

            // Setting headlights.
            AICar.Lights.lowBeamHeadlights = isNight;

            // Adding spawned bot to the list.
            allBots.Add(AICar.GetComponent<BR_PlayerManager>());

            // Calling an RPC event when a network player is spawned. All other clients will be notified.
            photonViewComponent.RPC("NetworkPlayerSpawned", RpcTarget.AllBuffered, AICar.GetComponent<BR_PlayerManager>().photonView.ViewID);

        }

        // Getting random nicknames.
        List<string> randomNicknames = GetRandomNicknames(BR_BotCars.Instance.botNicknames, allBots.Count);

        // Assigning random nicknames to all bots.
        for (int i = 0; i < allBots.Count; i++)
            allBots[i].nickName = randomNicknames[i];

    }

#endif

    private void Update() {

        for (int i = 0; i < allBots.Count; i++) {

            if (allBots[i] == null)
                allBots.RemoveAt(i);

        }

        // If the race has not started yet, no need to go further.
        if (!raceStarted)
            return;

        // Getting the current race position.
        if (currentPlayerCar)
            currentPosition = currentPlayerCar.RacePositioner.racePosition;

        // Getting the current lap.
        if (currentPlayerCar)
            currentLap = currentPlayerCar.RacePositioner.lap + 1;

        // Increase the timer.
        totalTime += Time.deltaTime;
        lapTime += Time.deltaTime;

        // Checking all network players before sorting. Removing any that are null.
        if (allRacersInOrder != null) {

            for (int i = 0; i < allRacersInOrder.Count; i++) {

                if (allRacersInOrder[i] == null)
                    allRacersInOrder.RemoveAt(i);

            }

        }

        // Sort the list if everything is okay.
        bool allOK = false;

        // Sorting the network players.
        if (allRacersInOrder != null && allRacers.Count >= 2) {

            allOK = true;

            for (int i = 0; i < allRacersInOrder.Count; i++) {

                if (allRacersInOrder[i] == null)
                    allOK = false;

            }

        }

        // Sort the list by position if all players are okay.
        if (allOK)
            allRacersInOrder.Sort(SortByPosition);

    }

    /// <summary>
    /// Spawns the offline player vehicle.
    /// </summary>
    private void SpawnPlayerOffline() {

        print("Spawning offline player");

        // Spawning and assigning the player vehicle.
        currentPlayerCar = RCCP.SpawnRCC(BR_PlayerCars.Instance.playerCars[selectedPlayerCarIndex].car.GetComponent<RCCP_CarController>(), spawnPoints[0].position, spawnPoints[0].rotation, true, false, true).GetComponent<BR_PlayerManager>();

        //  Loads the player's customization.
        LoadCustomization();

        // Setting canControl bool of the vehicle to false when spawned. We'll enable it after the race starts.
        currentPlayerCar.CarController.canControl = false;

        // Assigning the nickname of the vehicle as the player name.
        currentPlayerCar.nickName = BR_API.GetPlayerName();

#if PHOTON_UNITY_NETWORKING

        // Destroying the PhotonView for offline mode.
        if (currentPlayerCar.TryGetComponent(out PhotonView pV))
            Destroy(pV);

        // Destroying the RCCP_PhotonSync for offline mode.
        if (currentPlayerCar.TryGetComponent(out RCCP_PhotonSync pS))
            Destroy(pS);

#endif

        OfflinePlayerSpawned(currentPlayerCar);

        // Calling an event when the player spawns.
        if (OnLocalPlayerSpawned != null)
            OnLocalPlayerSpawned(currentPlayerCar);

        // Setting the headlights of the vehicle.
        if (currentPlayerCar.CarController.Lights)
            currentPlayerCar.CarController.Lights.lowBeamHeadlights = isNight;

    }

#if PHOTON_UNITY_NETWORKING

    /// <summary>
    /// Spawns the online player vehicle.
    /// </summary>
    private void SpawnPlayerOnline() {

        print("Spawning online player");

        // Current spawn index according to the player's actor number.
        int curSpawnIndex = (byte)PhotonNetwork.LocalPlayer.CustomProperties[BR_PlayerNumbering.RoomPlayerIndexedProp];

        // Getting the name of the vehicle because we'll be using the string to spawn the Photon prefab.
        string selectedVehicleName = BR_PlayerCars.Instance.playerCars[selectedPlayerCarIndex].car.transform.name;

        // Spawning the local player.
        GameObject playerCar = PhotonNetwork.Instantiate("Player Cars/" + selectedVehicleName, spawnPoints[curSpawnIndex].position, spawnPoints[curSpawnIndex].rotation);

        // Assigning the current player car.
        currentPlayerCar = playerCar.GetComponent<BR_PlayerManager>();

        // Setting canControl bool of the vehicle to false when spawned. We'll enable it after the race starts.
        currentPlayerCar.CarController.canControl = false;

        // Assigning the nickname of the vehicle as the player name.
        currentPlayerCar.nickName = BR_API.GetPlayerName();

        //  Loads the player's customization.
        LoadCustomization();

        // Calling an RPC method when a network vehicle is spawned. All other clients will be notified.
        photonViewComponent.RPC("NetworkPlayerSpawned", RpcTarget.AllBuffered, currentPlayerCar.photonView.ViewID);

        // Calling an event when the player spawns.
        if (OnLocalPlayerSpawned != null)
            OnLocalPlayerSpawned(currentPlayerCar);

        // Setting the lights of the vehicle.
        if (currentPlayerCar.CarController.Lights)
            currentPlayerCar.CarController.Lights.lowBeamHeadlights = isNight;

    }

    /// <summary>
    /// RPC method when a network player spawns. Adds the spawned vehicle to the dictionary.
    /// </summary>
    /// <param name="viewID">The Photon view ID of the spawned player.</param>
    [PunRPC]
    public void NetworkPlayerSpawned(int viewID) {

        // Adding the spawned network player to the dictionary.
        if (!allRacers.ContainsKey(viewID)) {

            PhotonView racer = PhotonView.Find(viewID);

            if (racer != null) {

                allRacers.Add(viewID, racer.gameObject.GetComponent<BR_PlayerManager>());
                allRacersInOrder.Add(racer.gameObject.GetComponent<BR_PlayerManager>());

            }

        }

    }

#endif

    /// <summary>
    /// Method when an offline player spawns. Assigns a random view ID to it and adds it to the dictionary.
    /// </summary>
    /// <param name="offlineSpawnedPlayer">The player manager of the spawned offline player.</param>
    public void OfflinePlayerSpawned(BR_PlayerManager offlineSpawnedPlayer) {

        // Assigning a random view ID for the offline player.
        int viewID = Random.Range(0, 9999);

        // If the view ID exists, randomize it again.
        foreach (int item in allRacers.Keys) {

            while (viewID == item)
                viewID = Random.Range(0, 9999);

        }

        // Adding the spawned offline player to the dictionary.
        if (!allRacers.ContainsKey(viewID)) {

            allRacers.Add(viewID, offlineSpawnedPlayer);
            allRacersInOrder.Add(offlineSpawnedPlayer);

        }

    }

    /// <summary>
    /// Starts the countdown before the race begins. Includes intro animation as well if selected.
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartCountdown() {

        yield return new WaitWhile(() => currentPlayerCar == null);

        // Game state is idle before the countdown.
        gameState = GameState.Idle;

        // Disabling the waiting for other player panel canvas.
        BR_UI_CanvasManager.Instance.waitingForOtherPlayers.SetActive(false);

        // Enabling the intro animation camera and playing it.
        BR_IntroCameraAnimation introAnimation = FindFirstObjectByType<BR_IntroCameraAnimation>(FindObjectsInactive.Include);

        // If intro animation is found, animate it and wait for x seconds.
        if (introAnimation) {

            // Game state is intro now.
            gameState = GameState.Intro;

            // Animating the camera's animator.
            introAnimation.AnimateCamera(true, currentPlayerCar.transform);

            // Waiting for the animation to end.
            yield return new WaitForSeconds(4);

        }

        // Game state is countdown now.
        gameState = GameState.Countdown;

        // Countdown starts now. Calling an event when this happens.
        if (OnLocalCountdownStarted != null)
            OnLocalCountdownStarted();

        print("Countdown Started");

        // Disabling the animation camera if selected and exists.
        if (introAnimation)
            introAnimation.AnimateCamera(false, currentPlayerCar.transform);

        // Registers the current spawned car as the player vehicle, disabling controllable state and enabling engine state. The race doesn't start yet; we need to wait for the countdown.
        RCCP.RegisterPlayerVehicle(currentPlayerCar.CarController, false, true);

        // Waiting for the countdown for x seconds.
        yield return new WaitForSeconds(4);

        // This is the important part. If we're in online mode and we're the master client, call the RPC method when the countdown ends and the race starts. All other clients will be notified, and everyone will start the race at the same time.
        // If the game mode is practice, or single-player, don't call the RPC method. Start the race locally.
        if (!offlineMode) {

#if PHOTON_UNITY_NETWORKING
            if (PhotonNetwork.IsMasterClient && gameType == GameType.Race)
                photonViewComponent.RPC("StartRaceRPC", RpcTarget.AllBuffered);
            else if (gameType == GameType.Practice)
                StartRace();
#else
            StartRace();
#endif

        } else {

            StartRace();

        }

    }

    /// <summary>
    /// Starts the race directly.
    /// </summary>
    public void StartRace() {

        // Game state is race now.
        gameState = GameState.Race;

        // Calling an event when the race starts.
        if (OnLocalRaceStarted != null)
            OnLocalRaceStarted();

        print("Race Started");

        // Registers the current spawned car as the player vehicle and enables controllable state along with engine state.
        RCCP.RegisterPlayerVehicle(currentPlayerCar.CarController, true, true);

        // Enabling all bots if they exist.
        if (botsEnabled && allBots != null && allBots.Count > 0) {

            for (int i = 0; i < allBots.Count; i++)
                allBots[i].CarController.canControl = true;

        }

        // Race started...
        raceStarted = true;

    }

    /// <summary>
    /// A racer finished the race.
    /// </summary>
    /// <param name="finishedRacer">The player manager of the racer who finished.</param>
    public void RacerFinishedRace(BR_PlayerManager finishedRacer) {

        // If it's our vehicle, finish the race.
        if (Equals(finishedRacer, currentPlayerCar))
            LocalPlayerFinishedRace();

        // If in online mode, call the RPC method when a racer finishes the race. All other clients will be notified.
        if (!offlineMode) {

#if PHOTON_UNITY_NETWORKING
            // RPC method when a race finishes.
            photonViewComponent.RPC("RacerFinishedRPC", RpcTarget.All, finishedRacer.photonView.ViewID);
#endif

        } else {

            // Disabling controllable state of the player vehicle.
            finishedRacer.CarController.canControl = false;

            // Adding the racer to the finished racers list.
            if (!allNetworkPlayersFinished.Contains(finishedRacer))
                allNetworkPlayersFinished.Add(finishedRacer);

            // If the finished racer vehicle is ours, set the finished position.
            for (int i = 0; i < allNetworkPlayersFinished.Count; i++) {

                if (allNetworkPlayersFinished[i] != null) {

                    if (finishedPosition == -1 && Equals(allNetworkPlayersFinished[i].gameObject, currentPlayerCar.gameObject))
                        finishedPosition = i + 1;

                }

            }

        }

    }

    /// <summary>
    /// Our vehicle finished the race.
    /// </summary>
    public void LocalPlayerFinishedRace() {

        // If the race has not started yet, no need to go further.
        if (!raceStarted)
            return;

        // Game state is idle now.
        gameState = GameState.Idle;

        print("Race Completed");

        // Race ended.
        raceStarted = false;

        // Enabling the fixed camera mode after the race finishes.
        RCCP_SceneManager.Instance.activePlayerCamera.useFixedCameraMode = true;
        RCCP_SceneManager.Instance.activePlayerCamera.ChangeCamera(RCCP_Camera.CameraMode.FIXED);

        // Disabling controllable state of the player vehicle.
        currentPlayerCar.CarController.canControl = false;

        // Add race points related to the position in the race if the game type is not single-player or practice.
        // Prizes can be changed directly on the inspector panel.
        if (!offlineMode) {

            if (gameType == GameType.Race) {

                if (currentPosition == 1) {

                    earnedRacePrize = firstPrize;
                    earnedRacePoints = 2;

                } else if (currentPosition >= allRacersInOrder.Count) {

                    earnedRacePrize = 0;
                    earnedRacePoints = -1;

                } else {

                    if (currentPosition == 2)
                        earnedRacePrize = secondPrize;
                    else if (currentPosition == 3)
                        earnedRacePrize = thirdPrize;
                    else
                        earnedRacePrize = 0;

                    earnedRacePoints = 1;

                }

            }

            // Adding race points.
            BR_API.AddRacePoints(earnedRacePoints);

        }

        // Save the current best lap and current best total times only in online mode.
        BR_API.SetBestCurrentTimeTotal(totalTime, SceneManager.GetActiveScene().name);
        BR_API.SetBestCurrentLapTime(lapTime, SceneManager.GetActiveScene().name);

        // Save the best lap and best total times.
        BR_API.SetBestTimeTotal(totalTime, SceneManager.GetActiveScene().name);
        BR_API.SetBestLapTime(lapTime, SceneManager.GetActiveScene().name);

        // If the player earned a prize, add it as money.
        if (earnedRacePrize > 0)
            BR_API.AddMoney(earnedRacePrize);

        // Calling the race finished event.
        if (OnLocalRaceFinished != null)
            OnLocalRaceFinished(currentPlayerCar);

    }

    /// <summary>
    /// Pauses or resumes the game. In online mode, it won't set timeScale to 0.
    /// </summary>
    public void Pause() {

        if (paused) {

            paused = !paused;

            if (offlineMode) {

                // Setting timeScale to the given value in BR_Settings and resuming AudioListener.
                Time.timeScale = BR_Settings.Instance.timeScale;
                AudioListener.pause = false;

            }

            print("Resumed");

        } else {

            paused = !paused;

            if (offlineMode) {

                // Setting timeScale to 0 and pausing the AudioListener.
                Time.timeScale = 0f;
                AudioListener.pause = true;

            }

            print("Paused");

        }

        // Calling an event when the race is paused.
        if (OnLocalRacePaused != null)
            OnLocalRacePaused(paused);

    }

    /// <summary>
    /// Quits the race and applies a penalty.
    /// </summary>
    public void Quit() {

        print("Leaving the race");

        // Enabling the leaving the race panel.
        BR_UI_CanvasManager.Instance.leavingMatchPanel.SetActive(true);

        // In online mode, consume 2 points as a penalty.
        if (raceStarted && gameType == GameType.Race && !offlineMode)
            BR_API.ConsumeRacePoints(2);

#if PHOTON_UNITY_NETWORKING

        // Leaving the room if in online mode. Otherwise, directly load the main menu.
        if (!offlineMode)
            PhotonNetwork.LeaveRoom();
        else
            SceneManager.LoadScene(0);

#else

        // Load the main menu.
        SceneManager.LoadScene(0);

#endif

    }

    /// <summary>
    /// Restarts the race. Allowed in single-player mode only.
    /// </summary>
    public void RestartRace() {

        print("Restarting");

        // Reloading the same scene.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    /// <summary>
    /// Sorts the players by comparing their positions.
    /// </summary>
    /// <param name="p1">The first player to compare.</param>
    /// <param name="p2">The second player to compare.</param>
    /// <returns>An integer indicating the relative order of the players.</returns>
    private static int SortByPosition(BR_PlayerManager p1, BR_PlayerManager p2) {

        // Return if one of the compared players is null.
        if (p1 == null || p2 == null)
            return 0;

        return p2.RacePositioner.GetRacePosition().CompareTo(p1.RacePositioner.GetRacePosition());

    }

    /// <summary>
    /// Gets random unique index numbers to spawn bots.
    /// </summary>
    /// <param name="count">The number of indexes to generate.</param>
    /// <returns>A list of random unique index numbers.</returns>
    public List<int> GetRandomBotIndexes(int count) {

        List<int> randomIndexes = new List<int>();
        int pickedIndex = Random.Range(0, BR_BotCars.Instance.AICars.Length);

        for (int k = 0; k < count; k++) {

            for (int i = 0; i < randomIndexes.Count; i++) {

                while (Equals(pickedIndex, randomIndexes[i]))
                    pickedIndex = Random.Range(0, BR_BotCars.Instance.AICars.Length);

            }

            randomIndexes.Add(pickedIndex);

        }

        return randomIndexes;

    }

    /// <summary>
    /// Gets random unique nicknames to assign to bots.
    /// </summary>
    /// <param name="nicknames">The array of available nicknames.</param>
    /// <param name="count">The number of nicknames to generate.</param>
    /// <returns>A list of random unique nicknames.</returns>
    public List<string> GetRandomNicknames(string[] nicknames, int count) {

        List<string> randomNicknames = new List<string>();
        string pickedNickname = nicknames[Random.Range(0, nicknames.Length)];

        for (int k = 0; k < count; k++) {

            for (int i = 0; i < randomNicknames.Count; i++) {

                while (Equals(pickedNickname, randomNicknames[i]))
                    pickedNickname = nicknames[Random.Range(0, nicknames.Length)];

            }

            randomNicknames.Add(pickedNickname);

        }

        return randomNicknames;

    }

    /// <summary>
    /// Loads the player's customization.
    /// </summary>
    public void LoadCustomization() {

        RCCP_Customizer customizer = currentPlayerCar.CarController.Customizer;

        if (!customizer)
            return;

        customizer.Load();
        customizer.Initialize();

    }

    public void OnDisable() {

        // Removing the callback target.
#if PHOTON_UNITY_NETWORKING
        PhotonNetwork.RemoveCallbackTarget(this);
#endif

    }

    #region RPC METHODS

#if PHOTON_UNITY_NETWORKING

    /// <summary>
    /// Calling RPC on all clients to make sure everyone is ready before the race starts.
    /// </summary>
    [PunRPC]
    public void PlayerJoinedAndReadyRPC() {

        totalReadyPlayers++;

        if (totalReadyPlayers >= PhotonNetwork.CurrentRoom.PlayerCount)
            photonViewComponent.RPC("StartCountdownRPC", RpcTarget.All);

    }

    /// <summary>
    /// RPC method to start the countdown.
    /// </summary>
    [PunRPC]
    public void StartCountdownRPC() {

        StartCoroutine(StartCountdown());

    }

    /// <summary>
    /// RPC method to start the race.
    /// </summary>
    [PunRPC]
    public void StartRaceRPC() {

        StartRace();

    }

    /// <summary>
    /// A racer finished the race. Adds them to the finished list.
    /// </summary>
    /// <param name="photonViewID">The Photon view ID of the racer who finished.</param>
    [PunRPC]
    public void RacerFinishedRPC(int photonViewID) {

        if (PhotonView.Find(photonViewID) == null) {

            Debug.LogError("Racer with " + photonViewID + " PhotonView ID has finished the race but couldn't be found in the scene!?");
            return;

        }

        // Finding the finished racer in the scene by their PhotonView ID.
        BR_PlayerManager finishedRacer = PhotonView.Find(photonViewID).gameObject.GetComponent<BR_PlayerManager>();

        // Disabling controllable state of the player vehicle.
        finishedRacer.CarController.canControl = false;

        // Adding the racer to the finished racers list.
        if (!allNetworkPlayersFinished.Contains(finishedRacer))
            allNetworkPlayersFinished.Add(finishedRacer);

        // If the finished racer vehicle is ours, set the finished position.
        for (int i = 0; i < allNetworkPlayersFinished.Count; i++) {

            if (allNetworkPlayersFinished[i] != null) {

                if (finishedPosition == -1 && Equals(allNetworkPlayersFinished[i].gameObject, currentPlayerCar.gameObject))
                    finishedPosition = i + 1;

            }

        }

    }

#endif

    #endregion

}
