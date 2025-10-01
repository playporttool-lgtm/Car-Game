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
/// UI gameplay canvas manager. 
/// </summary>
public class BR_UI_CanvasManager : MonoBehaviour {

    #region Singleton
    private static BR_UI_CanvasManager instance;
    /// <summary>
    /// Singleton instance of the BR_UI_CanvasManager.
    /// </summary>
    public static BR_UI_CanvasManager Instance { get { if (instance == null) instance = FindFirstObjectByType<BR_UI_CanvasManager>(); return instance; } }
    #endregion

    /// <summary>
    /// Reference to the current player.
    /// </summary>
    private BR_PlayerManager player;

    /// <summary>
    /// Panels for different UI states.
    /// </summary>
    [Header("Panels")]
    public GameObject gamePlayPanel;
    public GameObject gameOverPanel;
    public GameObject optionsPanel;
    public GameObject countdownPanel;
    public GameObject fadePanel;
    public GameObject wrongWayPanel;
    public GameObject waitingForOtherPlayers;
    public GameObject leavingMatchPanel;

    /// <summary>
    /// Texts and sliders to display player's score, time, and other stats.
    /// </summary>
    [Header("Dash Texts")]
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI timer;
    public TextMeshProUGUI prize;
    public TextMeshProUGUI currentPosition;
    public TextMeshProUGUI currentPositionExtensionWord;
    public TextMeshProUGUI currentPosition2;
    public TextMeshProUGUI currentPositionExtensionWord2;
    public TextMeshProUGUI currentLap;
    public TextMeshProUGUI targetLap;

    public TextMeshProUGUI bestTimeTotal;

    /// <summary>
    /// Determines whether to deactivate the gameplay panel at the start.
    /// </summary>
    public bool deactiveGameplayPanelAtStart = true;

    /// <summary>
    /// Prefab and content for displaying networked player scores during the game.
    /// </summary>
    public BR_UI_NetworkPlayerScore networkPlayerScorePrefab;
    public Transform networkPlayerScoreContent;
    private List<BR_UI_NetworkPlayerScore> allNetworkPlayerScores = new List<BR_UI_NetworkPlayerScore>();

    /// <summary>
    /// Prefab and content for displaying networked player scores at the end of the game (scoreboard).
    /// </summary>
    public BR_UI_NetworkPlayerEndScore networkPlayerEndScorePrefab;
    public Transform networkPlayerEndScoreContent;
    private List<BR_UI_NetworkPlayerEndScore> allNetworkPlayerEndScores = new List<BR_UI_NetworkPlayerEndScore>();

    private float updateTimer = 1f;

    private void Awake() {

        //  Deactivates the gameplay panel when the game starts. Usually, the gameplay panel will be enabled by the "OnRaceStarted" event.
        if (deactiveGameplayPanelAtStart)
            gamePlayPanel.SetActive(false);

        fadePanel.SetActive(true);

    }

    private void OnEnable() {

        //  Listening to events.
        BR_GameplayManager.OnLocalPlayerSpawned += GameplayManager_OnPlayerSpawned;
        BR_GameplayManager.OnLocalRaceStarted += GameplayManager_OnRaceStarted;
        BR_GameplayManager.OnLocalCountdownStarted += BR_GameplayManager_OnCountdownStarted;
        BR_GameplayManager.OnLocalRaceFinished += GameplayManager_OnRaceFinished;
        BR_GameplayManager.OnLocalRacePaused += GameplayManager_OnRacePaused;

        //  Getting the player name.
        playerName.text = BR_API.GetPlayerName();

    }

    /// <summary>
    /// When the countdown starts.
    /// </summary>
    private void BR_GameplayManager_OnCountdownStarted() {

        //  Enabling the gameplay panel and disabling the gameover panel.
        countdownPanel.SetActive(true);
        gamePlayPanel.SetActive(true);
        gameOverPanel.SetActive(false);

    }

    /// <summary>
    /// When the game is paused.
    /// </summary>
    /// <param name="state">The pause state.</param>
    private void GameplayManager_OnRacePaused(bool state) {

        //  Enabling/disabling the options panel.
        optionsPanel.SetActive(state);

    }

    /// <summary>
    /// When the local player spawns.
    /// </summary>
    /// <param name="Player">Reference to the local player.</param>
    private void GameplayManager_OnPlayerSpawned(BR_PlayerManager Player) {

        //  Assigning the player.
        player = Player;

    }

    /// <summary>
    /// When the race ends.
    /// </summary>
    /// <param name="Player">Reference to the player who finished the race.</param>
    private void GameplayManager_OnRaceFinished(BR_PlayerManager Player) {

        //  Disabling the gameplay panel and enabling the gameover panel.
        gamePlayPanel.SetActive(false);
        gameOverPanel.SetActive(true);

        //  Setting the text of the best total time.
        bestTimeTotal.text = BR_TimerToText.TimerText(BR_API.GetBestCurrentTimeTotal(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name));

    }

    /// <summary>
    /// Lists other network players at the end of the race.
    /// </summary>
    private void ListNetworkPlayersEndRace() {

        //  For each player, search and list their score.
        foreach (BR_PlayerManager item in BR_GameplayManager.Instance.allNetworkPlayersFinished) {

            bool foundSame = false;

            for (int i = 0; i < allNetworkPlayerEndScores.Count; i++) {

                if (allNetworkPlayerEndScores[i].player == item)
                    foundSame = true;

            }

            if (!foundSame) {

                BR_UI_NetworkPlayerEndScore networkPlayerScore = Instantiate(networkPlayerEndScorePrefab, networkPlayerEndScoreContent).gameObject.GetComponent<BR_UI_NetworkPlayerEndScore>();
                networkPlayerScore.player = item;
                allNetworkPlayerEndScores.Add(networkPlayerScore);

            }

        }

    }

    /// <summary>
    /// When the race starts.
    /// </summary>
    private void GameplayManager_OnRaceStarted() {

        //  Enabling the gameplay panel and disabling the gameover panel.
        countdownPanel.SetActive(false);
        gamePlayPanel.SetActive(true);
        gameOverPanel.SetActive(false);

    }

    /// <summary>
    /// Checks for other network players and updates the UI accordingly.
    /// </summary>
    private void CheckNetworkPlayers() {

        //  For each player, search and list their score.
        foreach (BR_PlayerManager item in BR_GameplayManager.Instance.allRacers.Values) {

            bool foundSame = false;

            for (int i = 0; i < allNetworkPlayerScores.Count; i++) {

                if (allNetworkPlayerScores[i].player == item)
                    foundSame = true;

            }

            if (!foundSame) {

                BR_UI_NetworkPlayerScore networkPlayerScore = Instantiate(networkPlayerScorePrefab, networkPlayerScoreContent).gameObject.GetComponent<BR_UI_NetworkPlayerScore>();
                networkPlayerScore.player = item;
                allNetworkPlayerScores.Add(networkPlayerScore);

            }

        }

    }

    private void Update() {

        updateTimer += Time.deltaTime;

        if (updateTimer >= 1f) {

            updateTimer = 0f;

            //  Listing other network players.
            if (BR_GameplayManager.Instance.allRacers != null)
                CheckNetworkPlayers();

            //  Listing other network players in the end of the race panel.
            if (BR_GameplayManager.Instance.allNetworkPlayersFinished != null && BR_GameplayManager.Instance.allNetworkPlayersFinished.Count >= 1)
                ListNetworkPlayersEndRace();

            if (player != null)
                wrongWayPanel.SetActive(player.RacePositioner.CheckWrongWay());

        }

    }

    private void LateUpdate() {

        //  Return if no player found.
        if (player == null)
            return;

        //  Getting the current timer.
        float timeCount = (BR_GameplayManager.Instance.totalTime);

        if (timeCount != -1)
            timer.text = BR_TimerToText.TimerText(timeCount);
        else
            timer.text = "";

        //  Getting the earned race prize amount.
        float earnedRacePrize = (BR_GameplayManager.Instance.earnedRacePrize);

        if (earnedRacePrize != 0)
            prize.text = "$ " + earnedRacePrize.ToString("F0");
        else
            prize.text = "";

        currentPosition.text = BR_GameplayManager.Instance.currentPosition.ToString("F0");
        currentLap.text = BR_GameplayManager.Instance.currentLap.ToString("F0");

        if (BR_GameplayManager.Instance.targetLap != -1)
            targetLap.text = BR_GameplayManager.Instance.targetLap.ToString("F0");
        else
            targetLap.text = "-";

        if (BR_GameplayManager.Instance.currentPosition == 1)
            currentPositionExtensionWord.text = "ST";
        else if (BR_GameplayManager.Instance.currentPosition == 2)
            currentPositionExtensionWord.text = "ND";
        else if (BR_GameplayManager.Instance.currentPosition == 3)
            currentPositionExtensionWord.text = "RD";
        else
            currentPositionExtensionWord.text = "TH";

        currentPosition2.text = BR_GameplayManager.Instance.finishedPosition.ToString("F0");

        if (BR_GameplayManager.Instance.finishedPosition == 1)
            currentPositionExtensionWord2.text = "ST";
        else if (BR_GameplayManager.Instance.finishedPosition == 2)
            currentPositionExtensionWord2.text = "ND";
        else if (BR_GameplayManager.Instance.finishedPosition == 3)
            currentPositionExtensionWord2.text = "RD";
        else
            currentPositionExtensionWord2.text = "TH";

    }

    /// <summary>
    /// Cancels the waiting state and quits the game.
    /// </summary>
    public void CancelWaiting() {

        BR_GameplayManager.Instance.Quit();

    }

    /// <summary>
    /// Restarts the race.
    /// </summary>
    public void Restart() {

        BR_GameplayManager.Instance.RestartRace();

    }

    /// <summary>
    /// Quits the game.
    /// </summary>
    public void Quit() {

        BR_GameplayManager.Instance.Quit();

    }

    private void OnDisable() {

        BR_GameplayManager.OnLocalPlayerSpawned -= GameplayManager_OnPlayerSpawned;
        BR_GameplayManager.OnLocalRaceStarted -= GameplayManager_OnRaceStarted;
        BR_GameplayManager.OnLocalCountdownStarted -= BR_GameplayManager_OnCountdownStarted;
        BR_GameplayManager.OnLocalRaceFinished -= GameplayManager_OnRaceFinished;
        BR_GameplayManager.OnLocalRacePaused -= GameplayManager_OnRacePaused;

    }

}
