//----------------------------------------------
//			Burnout Racers
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Networking;
using BEKStudio;

#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
#endif

/// <summary>
/// Manages the main menu. Spawns player vehicles, switches them, selects / purchases them. Handles UI panels, sliders, and texts.
/// </summary>
public class BR_MainMenuManager : MonoBehaviour {

    private static BR_MainMenuManager instance;

    /// <summary>
    /// Instance.
    /// </summary>
    public static BR_MainMenuManager Instance {

        get {

            if (instance == null)
                instance = FindFirstObjectByType<BR_MainMenuManager>();

            return instance;

        }

    }

    /// <summary>
    /// Control the player vehicle at the main menu.
    /// </summary>
    public bool controlPlayerVehicle = false;

    private bool controlPlayerVehicleNow = false;

    /// <summary>
    /// Instantiated all player vehicles.
    /// </summary>
    private List<GameObject> instantiatedPlayerCars = new List<GameObject>();

    /// <summary>
    /// Current vehicle in the scene.
    /// </summary>
    public BR_PlayerManager currentPlayerCar;

    /// <summary>
    /// Current player money.
    /// </summary>
    private int playerMoney = 0;

    /// <summary>
    /// Current race points.
    /// </summary>
    private int racePoints = 0;

    /// <summary>
    /// Current selected vehicle index.
    /// </summary>
    [HideInInspector] public int currentPlayerCarIndex = 0;

    /// <summary>
    /// Spawn point.
    /// </summary>
    [Header("Spawn Point")]
    public Transform spawnPoint;

    //  All major UI panels.
    [Header("UI Menus")]
    public GameObject mainMenu;
    public GameObject carSelectMenu;
    public GameObject storeMenu;
    public GameObject onlineMenu;
    public GameObject loadingMenu;
    public GameObject welcomeMenu;
    public GameObject creditsMenu;
    public GameObject selectionModeMenu;
    public GameObject selectionSceneOfflineMenu;
    public GameObject purchaseConfirmationMenu;
    //public TextMeshProUGUI coinText;

    /// <summary>
    /// Displaying currency.
    /// </summary>
    [Header("UI Texts")]
    //public TextMeshProUGUI currencyText;

    /// <summary>
    /// Displaying race points text.
    /// </summary>
    public TextMeshProUGUI racePointsText;

    /// <summary>
    /// Displaying the current car's price.
    /// </summary>
    public TextMeshProUGUI priceText;

    /// <summary>
    /// Displaying the current item's price.
    /// </summary>
    public TextMeshProUGUI storeItemPriceText;

    /// <summary>
    /// Displaying player name.
    /// </summary>
    public TextMeshProUGUI playerNameText;

    //  UI buttons used to buy / select / mod.
    [Header("UI Buttons")]
    public GameObject buyCarButton;
    public GameObject selectCarButton;
    public GameObject selectedButton;
    public GameObject modCarPanel;
    public GameObject lockedImage;
    public GameObject storeConfirmationButtons;

    // UI car stats.
    [Header("UI Sliders")]
    public Slider engine;
    public Slider handling;
    public Slider brake;
    public Slider speed;

    // UI car stats (upgraded, maximum).
    [Header("UI Sliders Max")]
    public Slider engineMax;
    public Slider handlingMax;
    public Slider brakeMax;
    public Slider speedMax;

    /// <summary>
    /// UI horizontal gametype selectors.
    /// </summary>
    [Header("UI Horizontal GameType Selectors")]
    public BR_UI_HorizontalSelector[] gameTypeHorizontalSelectors;

    /// <summary>
    /// UI horizontal gamemode (multiplayer / singleplayer) selectors.
    /// </summary>
    [Header("UI Horizontal GameMode Selectors")]
    public BR_UI_HorizontalSelector[] botsHorizontalSelectors;

    /// <summary>
    /// UI horizontal gamemode (multiplayer / singleplayer) selectors.
    /// </summary>
    [Header("UI Horizontal Bot Amount Selectors")]
    public BR_UI_HorizontalSelector[] botsAmountHorizontalSelectors;

    /// <summary>
    /// UI horizontal lap amount selectors.
    /// </summary>
    [Header("UI Horizontal Lap Selectors")]
    public BR_UI_HorizontalSelector[] lapsAmountHorizontalSelectors;

    /// <summary>
    /// UI Loading panel with slider bar.
    /// </summary>
    [Header("UI Loading Section")]
    private AsyncOperation async;
    public Slider loadingBar;

    /// <summary>
    /// Nickname input field.
    /// </summary>
    public TMP_InputField nickNameInputField;

    /// <summary>
    /// Vehicle renderer.
    /// </summary>
    public GameObject vehicleRenderCamera;

    /// <summary>
    /// Background canvas when we're selecting.
    /// </summary>
    public GameObject vehicleBackgroundCanvas;

    /// <summary>
    /// General background canvas.
    /// </summary>
    public GameObject generalBackgroundImage;

    /// <summary>
    /// Headlights should be enabled or not?
    /// </summary>
    [Space()] public bool headlightsEnabled = true;

    public GameObject leaderboard;

    /// <summary>
    /// The last selected store item.
    /// </summary>
    private BR_UI_StoreItemButton lastSelectedStoreItem;


    //API variables
    public UserData UMdata;
    public UserData userDatas;
    private string apiUrl = "https://api.playport.lk/api/v1/users/profile";
    public TextMeshProUGUI avatarUsernameText;
    public string playerName;
    public TextMeshProUGUI coinText;
    private int tempCoin;


          [Header("FindAnims")]

        public int OpponentS;
        public Animator dp;
        public Animator coinflow;
        public Animator coinflow1;
        public Animator numlaod;
        public Animator nameload;
        public Animator coinBonus;

            [Header("Room")] 
        public TextMeshProUGUI roomGalleEntryPriceText;
        public TextMeshProUGUI roomKandyEntryPriceText;
        public TextMeshProUGUI roomColomboEntryPriceText;
        public TextMeshProUGUI roomJaffnaEntryPriceText;
        public TextMeshProUGUI roomSigiriEntryPriceText;

        public TextMeshProUGUI roomGalleWinPriceText;
        public TextMeshProUGUI roomKandyWinPriceText;
        public TextMeshProUGUI roomColomboWinPriceText;
        public TextMeshProUGUI roomJaffnaWinPriceText;
        public TextMeshProUGUI roomSigiriWinPriceText;


        [Header("Vs")] public GameObject vsScreen;
        public TextMeshProUGUI vsMsgText;
        public Button vsBackBtn;
        public GameObject vsUsersParent;
        public TextMeshProUGUI vsHomeUsernameText;
        public TextMeshProUGUI vsHomeUserBetText;
        public TextMeshProUGUI vsAwayUsernameText;
        public TextMeshProUGUI vsAwayUserBetText;
        public TextMeshProUGUI vsTotalBetText;
        IEnumerator noOpponentFound;
        IEnumerator OpponentFound;
        IEnumerator OpponentJoined;

        
       
         

 

    private void Awake() {

        //  Setting the latest saved quality level.
        int qualityLevel = PlayerPrefs.GetInt("QualityLevel", 1);
        QualitySettings.SetQualityLevel(qualityLevel);

        // Make sure timescale is set, and audiolistener is not paused.
        Time.timeScale = BR_Settings.Instance.timeScale;
        AudioListener.pause = false;

        // Getting volume with volume amount.
        AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        //  Getting last selected multiplayer option.

#if !PHOTON_UNITY_NETWORKING
                BR_API.SetMultiplayer(false);
#endif

#if !BCG_BURNOUTRACERS_LEADERBOARD
        if (leaderboard)
            Debug.LogWarning("You're not using the leaderboard, but leaderboard UI button in the main menu is enabled. You may want to disable or delete the gameobject. If you want to use the leaderboard features, enable it in the BR_Settings. More info can be found in the documentation.");
#endif

    }

    private void Start() {

        //api settings
        ShowLoadingScreen();
        GetUserProfile();
        //  Getting the player name.
        ShowRooms();
        PlayerPrefs.SetInt("coin",1000);
        BR_API.SetPlayerName(avatarUsernameText.text);
        string nickName = BR_API.GetPlayerName();
        

        //  If it's not empty, open the main menu. Otherwise open the welcome menu.
        if (BR_API.IsFirstGameplay()) {

            //OpenMenu(welcomeMenu);
            //nickName = "Player " + UnityEngine.Random.Range(0, 9999).ToString("F0");

        } else {
            //Main Menu
            SelectMode(true);
            OpenMenu(onlineMenu);

        }

        //  Getting the player money.
        OnPlayerMoneyChanged();

        racePoints = BR_API.GetRacePoints();

        //  Setting nickname input field.
        nickNameInputField.SetTextWithoutNotify(nickName);

        //  Getting latest selected player vehicle index.
        currentPlayerCarIndex = BR_API.GetVehicle();

        //  Setting all game type selectors.
        foreach (BR_UI_HorizontalSelector item in gameTypeHorizontalSelectors)
            item.value = BR_API.GetGameType();

        //  Setting all bot selectors.
        foreach (BR_UI_HorizontalSelector item in botsHorizontalSelectors)
            item.value = BR_API.GetBots() == false ? 0 : 1;

        //  Setting all bot amount selectors.
        foreach (BR_UI_HorizontalSelector item in botsAmountHorizontalSelectors)
            item.value = BR_API.GetBotsAmount() - 1;

        //  Setting all lap amount selectors.
        foreach (BR_UI_HorizontalSelector item in lapsAmountHorizontalSelectors)
            item.value = BR_API.GetLapsAmount() - 1;

        // Creating all vehicles at once, and disabling all of them.
        CreateCars();

        //  Enabling only selected vehicle (Not instantiating again).
        SpawnPlayer();

    }

    private void OnEnable() {

        //  Listening event when money changed.
        BR_API.OnPlayerMoneyChanged += OnPlayerMoneyChanged;

    }



    //API features

    private string LimitNameText(string fullName, int maxLength = 8)
    {
        if (string.IsNullOrEmpty(fullName)) return "";
        return fullName.Length > maxLength ? fullName.Substring(0, maxLength) + ".." : fullName;
    }

    private string FormatCurrency(int amount)
    {
        if (amount >= 1000000)
        {
            // For millions: 1.5M, 2M, etc.
            return (amount / 1000000f).ToString("0.#") + "M";
        }
        else if (amount >= 1000)
        {
            // For thousands: 1k, 1.2k, 15k, etc.
            return (amount / 1000f).ToString("0.#") + "k";
        }
        else
        {
            // Below 1000: show as-is
            return amount.ToString();
        }
    }
   public void UpdateCurrencyText()
        {
            if (PlayerPrefs.GetInt("coin") == 0)
            {
                coinText.text = "0";

              
            }
            else
            {
                coinText.text = FormatCurrency(PlayerPrefs.GetInt("coin", 0));
            }
        }

    public void RoomsBtn(string room)
        {
            //setupScreens();
            //checking coin balance
            string msg = "Your account balance is low!\r\nPlease top up your account\r\nto continue playing";
            int coinBal = PlayerPrefs.GetInt("coin");

            if (coinBal <= 0)
            {
                //CheckCoinBalance(msg);
                print("No coins....");
            }

            else
            {
                
                if (room == "Colombo" && Constants.ROOM_COLOMBO_ENTRY_PRICE<= coinBal)
                {
                    setupScreens();
                    BR_PhotonLobbyManager.Instance.roomEntryPice = Constants.ROOM_COLOMBO_ENTRY_PRICE;
                    //PhotonController.Instance.FindRoom();
                    PlayerPrefs.SetString("roomName", room);
                }
                else if (room == "Galle" && Constants.ROOM_GALLE_ENTRY_PRICE <= coinBal)
                {
                    setupScreens();
                    BR_PhotonLobbyManager.Instance.roomEntryPice = Constants.ROOM_GALLE_ENTRY_PRICE;
                    //PhotonController.Instance.FindRoom();
                    PlayerPrefs.SetString("roomName", room);
                }
                else if (room == "Kandy" && Constants.ROOM_KANDY_ENTRY_PRICE <= coinBal)
                {
                    setupScreens();
                    BR_PhotonLobbyManager.Instance.roomEntryPice = Constants.ROOM_KANDY_ENTRY_PRICE;
                    //PhotonController.Instance.FindRoom();
                    PlayerPrefs.SetString("roomName", room);
                }
                else if (room == "Sigiri" && Constants.ROOM_SIGIRI_ENTRY_PRICE <= coinBal)
                {
                    setupScreens();
                    BR_PhotonLobbyManager.Instance.roomEntryPice = Constants.ROOM_SIGIRI_ENTRY_PRICE;
                    //PhotonController.Instance.FindRoom();
                    PlayerPrefs.SetString("roomName", room);
                }
                else if (room == "Jaffna" && Constants.ROOM_JAFFNA_ENTRY_PRICE <= coinBal)
                {
                    setupScreens();
                    BR_PhotonLobbyManager.Instance.roomEntryPice = Constants.ROOM_JAFFNA_ENTRY_PRICE;
                    //PhotonController.Instance.FindRoom();
                    PlayerPrefs.SetString("roomName", room);
                }
                else
                {
                    //CheckCoinBalance(msg);
                }

                
            }
            
        }

        public void ShowRooms()
        {
            /*roomLondonEntryPriceText.text = Constants.ROOM_LONDON_ENTRY_PRICE.ToString("###,###,###");
            roomLondonWinPriceText.text = (Constants.ROOM_LONDON_ENTRY_PRICE * 2).ToString("###,###,###");
            roomParisEntryPriceText.text = Constants.ROOM_PARIS_ENTRY_PRICE.ToString("###,###,###");
            roomParisWinPriceText.text = (Constants.ROOM_PARIS_ENTRY_PRICE * 2).ToString("###,###,###");
            roomBerlinEntryPriceText.text = Constants.ROOM_BERLIN_ENTRY_PRICE.ToString("###,###,###");
            roomBerlinWinPriceText.text = (Constants.ROOM_BERLIN_ENTRY_PRICE * 2).ToString("###,###,###");

 
*/
            roomGalleEntryPriceText.text = "Entry Fee: "+ Constants.ROOM_GALLE_ENTRY_PRICE.ToString() + "LKR";
            roomKandyEntryPriceText.text = "Entry Fee: " + Constants.ROOM_KANDY_ENTRY_PRICE.ToString() + "LKR";
            roomColomboEntryPriceText.text = "Entry Fee: " + Constants.ROOM_COLOMBO_ENTRY_PRICE.ToString() + "LKR";
            roomJaffnaEntryPriceText.text = "Entry Fee: " + Constants.ROOM_JAFFNA_ENTRY_PRICE.ToString() + "LKR";
            roomSigiriEntryPriceText.text = "Entry Fee: " + Constants.ROOM_SIGIRI_ENTRY_PRICE.ToString() + "LKR";

            roomGalleWinPriceText.text =  Constants.ROOM_GALLE_WIN_PRICE.ToString() + "LKR";
            roomKandyWinPriceText.text = Constants.ROOM_KANDY_WIN_PRICE.ToString() + "LKR";
            roomColomboWinPriceText.text =  Constants.ROOM_COLOMBO_WIN_PRICE.ToString() + "LKR";
            roomJaffnaWinPriceText.text =  Constants.ROOM_JAFFNA_WIN_PRICE.ToString() + "LKR";
            roomSigiriWinPriceText.text = Constants.ROOM_SIGIRI_WIN_PRICE.ToString() + "LKR";


            //PhotonController.Instance.whichMode = "carrom";
            //Debug.Log("Show rooms");
            //vsScreen.SetActive(false);
            //NoOpponentScreen.SetActive(false);
            //shopScreen.SetActive(false);

            //mainScreen.SetActive(false);
            //usernameScreen.SetActive(false);
            //roomScreen.SetActive(true);
        }


    // API Response structure
    [System.Serializable]
    public class ApiResponse
    {
        public bool success;
        public string message;
        public UserData data;
    }

    // User data structure matching the API response
    [System.Serializable]
    public class UserData
    {
        public string id;
        public string username;
        public string phone_number;
        public string email;
        public string password_hash;
        public int total_coins;
        public string profile_image_url;
        public bool is_active;
        public bool is_verified;
        public string status;
        public string suspension_reason;
        public string suspended_until;
        public string kyc_status;
        public string kyc_documents;
        public int risk_score;
        public string created_at;
        public string updated_at;
        public string last_login_at;
        public string suspended_by;
        public string game_id;
    }


    public void ShowLoadingScreen()
    {
        //coinErrorPanel.SetActive(false);
        //LoadingScreen.SetActive(true);
        //loadingBar.value = 0;

        // Start the loading process
        StartCoroutine(LoadingProcess());
    }


    private IEnumerator LoadingProcess()
    {
        bool photonReady = false;
        bool userDataReady = false;
        float loadingProgress = 0f;
        float maxLoadingTime = 25f; // Maximum loading time
        float timer = 0f;

        // Start both processes concurrently
        StartCoroutine(WaitForPhotonConnection(() => photonReady = true));
        StartCoroutine(WaitForUserData(() => userDataReady = true));

        // Update loading bar while waiting
        while (!photonReady || !userDataReady)
        {
            timer += Time.deltaTime;

            // Check for timeout
            if (timer >= maxLoadingTime)
            {
                Debug.Log("Loading timeout - Connection failed");
                string msg = "No Connection! Please check your internet and try again.";
                //CheckConnection(msg);
                //LoadingScreen.SetActive(false);
                yield break;
            }

            // Calculate progress based on completion status
            float targetProgress = 0f;
            if (photonReady) targetProgress += 0.5f;
            if (userDataReady) targetProgress += 0.5f;

            // Smooth progress bar animation
            loadingProgress = Mathf.Lerp(loadingProgress, targetProgress, Time.deltaTime * 2f);
            //loadingBar.value = loadingProgress;

            yield return null;
        }

        // Final animation to complete
        while (loadingBar.value < 0.99f)
        {
            //loadingBar.value = Mathf.Lerp(loadingBar.value, 1f, Time.deltaTime * 5f);
            yield return null;
        }

        // Small delay before hiding loading screen
        yield return new WaitForSeconds(0.5f);
        //LoadingScreen.SetActive(false);
    }

    private IEnumerator WaitForPhotonConnection(System.Action onComplete)
    {
        // Wait until Photon is connected to Master Server and joined lobby
        while (!PhotonNetwork.IsConnectedAndReady ||
            !PhotonNetwork.InLobby ||
            PhotonNetwork.Server != Photon.Realtime.ServerConnection.MasterServer)
        {

            // Additional check to ensure we're in the right state for matchmaking
            if (PhotonNetwork.IsConnected &&
                PhotonNetwork.Server != Photon.Realtime.ServerConnection.MasterServer)
            {
                //Debug.Log("Waiting for Master Server connection...");
            }
            else if (PhotonNetwork.IsConnectedAndReady && !PhotonNetwork.InLobby)
            {
                //Debug.Log("Waiting for lobby join...");

            }

            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("Photon ready for matchmaking - Connected to Master Server and in Lobby");
        onComplete?.Invoke();
    }

    private IEnumerator WaitForUserData(System.Action onComplete)
    {
        // Start fetching user profile
        yield return StartCoroutine(GetUserProfileCoroutine());

        // Ensure UMdata is set (fallback to default if API fails)
        if (UMdata == null || string.IsNullOrEmpty(UMdata.username))
        {
            UMdata = userDatas; // Use your fallback data
            if (string.IsNullOrEmpty(UMdata.username))
            {
                UMdata.username = "Guest"; // Final fallback
            }
        }

        onComplete?.Invoke();
    }

    public void GetUserProfile()
    {
        Debug.Log("Fetching user profile...");

        StartCoroutine(GetUserProfileCoroutine());

    }

    public string GAME_ID
    {
        get
        {
            if (string.IsNullOrEmpty(_gameId))
            {
                _gameId = PlayerPrefs.GetString("GAME_ID", "");
            }
            return _gameId;
        }
        set
        {
            _gameId = value;
            if (!string.IsNullOrEmpty(value))
            {
                PlayerPrefs.SetString("GAME_ID", value);
                PlayerPrefs.Save();
                Debug.Log("GAME_ID saved to PlayerPrefs: " + value);
            }
        }
    }
    private string _gameId = "";

    [System.Serializable]
    public class UpdateGameStatsRequest
    {
        public string game_id;
        public int wins;
        public int losses;
    }

    [System.Serializable]
    public class GameStatsResponse
    {
        public bool success;
        public string message;
        // Add more fields here if the API returns additional data
    }

    // ============================================
    // Add these methods to your MenuController script
    // ============================================

    // Game ID - replace with your actual game ID


    // Call this when player wins
    public void UpdateWins()
    {
        StartCoroutine(UpdateGameStatsCoroutine(wins: 1, losses: 0));
    }

    // Call this when player loses
    public void UpdateLosses()
    {
        StartCoroutine(UpdateGameStatsCoroutine(wins: 0, losses: 1));
    }

    // Main coroutine for updating game stats
    public IEnumerator UpdateGameStatsCoroutine(int wins, int losses)
    {
        // Validate GAME_ID
        if (string.IsNullOrEmpty(GAME_ID))
        {
            Debug.LogError("[UpdateGameStats] GAME_ID is empty! Cannot update stats.");
            Debug.LogError("Make sure the URL contains the gameId parameter with & separator");
            yield break;
        }

        Debug.Log($"[UpdateGameStats] Sending update with GAME_ID: {GAME_ID}");

        string json;
        if (wins > 0)
        {
            json = $"{{\"game_id\":\"{GAME_ID}\",\"wins\":{wins}}}";
        }
        else
        {
            json = $"{{\"game_id\":\"{GAME_ID}\",\"losses\":{losses}}}";
        }

        Debug.Log($"[UpdateGameStats] JSON payload: {json}");

        string token = GetTokenFromURL();

        using (var request = new UnityWebRequest("https://api.playport.lk/api/v1/games-stats", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", "Bearer " + token);
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"[UpdateGameStats:POST] Error: {request.error} (HTTP {request.responseCode})");
                Debug.LogError($"[UpdateGameStats:POST] Body: {request.downloadHandler.text}");
                yield break;
            }

            string jsonResponse = request.downloadHandler.text;
            Debug.Log("[UpdateGameStats:POST] Raw Response: " + jsonResponse);

            GameStatsResponse response = JsonUtility.FromJson<GameStatsResponse>(jsonResponse);
            if (response != null && response.success)
            {
                Debug.Log($"[UpdateGameStats:POST] Success! {(wins > 0 ? "Win" : "Loss")} recorded. Message: {response.message}");
            }
            else
            {
                Debug.LogWarning("[UpdateGameStats:POST] API reported success=false");
            }
        }
    }

    [System.Serializable]
        public class UpdateProfileRequest
        {
            public int total_coins;
        }
        public IEnumerator UpdateUserProfileCoroutine_PUT(int newTotalCoins)
        {
            var update = new UpdateProfileRequest { total_coins = newTotalCoins };
            string json = JsonUtility.ToJson(update);

            string token = GetTokenFromURL();

            // UnityWebRequest.Put creates an UploadHandlerRaw for you, but we still must set Content-Type
            using (var request = UnityWebRequest.Put("https://api.playport.lk/api/v1/users/profile", json))
            {
                request.SetRequestHeader("Authorization", "Bearer " + token);
                request.SetRequestHeader("Content-Type", "application/json");
                request.downloadHandler = new DownloadHandlerBuffer();

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError ||
                    request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError($"[UpdateUserProfile:PUT] Error: {request.error} (HTTP {request.responseCode})");
                    Debug.LogError($"[UpdateUserProfile:PUT] Body: {request.downloadHandler.text}");
                    yield break;
                }

                string jsonResponse = request.downloadHandler.text;
                Debug.Log("[UpdateUserProfile:PUT] Raw Response: " + jsonResponse);

                ApiResponse response = JsonUtility.FromJson<ApiResponse>(jsonResponse);
                if (response != null && response.success)
                {
                    Debug.Log("[UpdateUserProfile:PUT] Success! Message: " + response.message);
                    Debug.Log("[UpdateUserProfile:PUT] New Total Coins: " + response.data.total_coins);
                    OnProfileFetched(response.data);
                }
                else
                {
                    Debug.LogWarning("[UpdateUserProfile:PUT] API reported success=false");
                }
            }

            UpdateCurrencyText();
        }

        public void BuyNewCoins(){
                int currentCoins = UMdata != null ? UMdata.total_coins : PlayerPrefs.GetInt("coin", 0);
    
    // Add the amount you want to give (e.g., 100 coins)
    int newTotal = currentCoins + 100;
    
    Debug.Log($"Current coins: {currentCoins}, Adding 100, New total: {newTotal}");
    
    StartCoroutine(UpdateUserProfileCoroutine_PUT(newTotal));
        }

    public IEnumerator GetUserProfileCoroutine()
    {


        string tokent = GetTokenFromURL();
        //Debug.Log("GetTokenFromURL: " + tokent);
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            // Add Bearer token authorization
            request.SetRequestHeader("Authorization", "Bearer " + tokent);
            Debug.Log("Req:" + request);
            // Send the request
            yield return request.SendWebRequest();

            // Check for errors
            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                /*Debug.Log("Error: " + request.error);
                Debug.Log("Response Code: " + request.responseCode);
                UMdata = userDatas;
                avatarUsernameText.text = "ranil";*/

                // Success - parse the response
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Raw Response: " + jsonResponse);

                // Parse the API response
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(jsonResponse);

                Debug.Log("response" + response);

                if (response != null && response.success)
                {
                    Debug.Log("Success! Message: " + response.message);
                    Debug.Log("Username: " + response.data.username);
                    Debug.Log("Phone: " + response.data.phone_number);
                    Debug.Log("Total Coins: " + response.data.total_coins);
                    Debug.Log("Account Status: " + response.data.status);
                    Debug.Log("KYC Status: " + response.data.kyc_status);

                    // Use the data as needed
                    OnProfileFetched(response.data);
                }
                else
                {
                    Debug.Log("API returned=false");
                    UMdata = userDatas;
                    playerName = "SupunTesting";
                    avatarUsernameText.text = LimitNameText(playerName);
                    PlayerPrefs.SetString("username", playerName);
                }

            }
            else
            {
                // Success - parse the response
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Raw Response: " + jsonResponse);

                // Parse the API response
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(jsonResponse);

                if (response.success)
                {
                    Debug.Log("Success! Message: " + response.message);
                    Debug.Log("Username: " + response.data.username);
                    Debug.Log("Phone: " + response.data.phone_number);
                    Debug.Log("Total Coins: " + response.data.total_coins);
                    Debug.Log("Account Status: " + response.data.status);
                    Debug.Log("KYC Status: " + response.data.kyc_status);

                    // Use the data as needed
                    OnProfileFetched(response.data);
                }
                else
                {
                    Debug.Log("API returned=false");
                    UMdata = userDatas;
                    playerName = "Mahinda";
                    avatarUsernameText.text = LimitNameText(playerName);
                    PlayerPrefs.SetString("username", playerName);
                }
            }
        }
    }

    private void OnProfileFetched(UserData userData)
    {
        // Handle the fetched user data here
        // Update UI, save to PlayerPrefs, etc.

        // Example: Display username in UI
        // usernameText.text = userData.username;
        coinText.text = userData.total_coins.ToString();

        UMdata = userData;
        Debug.Log("Profile fetched and stored.");
        playerName = UMdata.username;
        PlayerPrefs.SetString("username", playerName);
        avatarUsernameText.text = LimitNameText(UMdata.username);

        tempCoin = UMdata.total_coins;


        PlayerPrefs.SetInt("coin", tempCoin);

        PlayerPrefs.Save();
        PhotonNetwork.NickName = UMdata.username;



        UpdateCurrencyText();

    }

    public string GetTokenFromURL()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
    string url = Application.absoluteURL;
    Debug.Log("Full URL: " + url);
    
    if (!string.IsNullOrEmpty(url))
    {
        try
        {
            int queryStart = url.IndexOf('?');
            if (queryStart >= 0)
            {
                string queryString = url.Substring(queryStart + 1);
                Debug.Log("Query string: " + queryString);
                
                string[] parameters = queryString.Split('&');
                string token = null;
                
                foreach (string param in parameters)
                {
                    Debug.Log("Processing parameter: " + param);
                    
                    int equalIndex = param.IndexOf('=');
                    if (equalIndex > 0)
                    {
                        string key = param.Substring(0, equalIndex);
                        string value = param.Substring(equalIndex + 1);
                        
                        if (key == "token")
                        {
                            token = UnityEngine.Networking.UnityWebRequest.UnEscapeURL(value);
                            Debug.Log("Token found: " + token.Substring(0, Mathf.Min(20, token.Length)) + "...");
                        }
                        else if (key == "gameid" || key == "gameId" || key == "game_id")
                        {
                            GAME_ID = UnityEngine.Networking.UnityWebRequest.UnEscapeURL(value);
                            Debug.Log("Game ID from URL: " + GAME_ID);
                        }
                    }
                }
                
                if (token != null)
                {
                    return token;
                }
                
                Debug.LogWarning("Token parameter not found in URL");
            }
            else
            {
                Debug.LogWarning("No query string found in URL");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error parsing URL: " + e.Message);
        }
    }
    else
    {
        Debug.LogWarning("Application.absoluteURL is null or empty");
    }
#else
        Debug.Log("Not running in WebGL build - using test values");
        //GAME_ID = "d7d9c65e-a763-422c-8865-181f307e1ecf";
        //Debug.Log("Editor mode - GAME_ID set to: " + GAME_ID);
        return "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiJhYjkxMWJiYS05ZTIzLTRhYTYtYTMyYi0wMmE5YzFjODU3NjQiLCJyb2xlIjoidXNlciIsImlhdCI6MTc1ODI4MDYxNCwiZXhwIjoxNzU4Mjg0MjE0fQ.yJBZMa_v5246bg2cJ6ElFH4r-GBvt2zuLMYQgILb7";
#endif
        Debug.LogError("Failed to extract token from URL");
        return null;
    }

    // Optional: Method to retrieve stored token
    public string GetStoredToken()
    {
        if (PlayerPrefs.HasKey("auth_token"))
        {
            return PlayerPrefs.GetString("auth_token");
        }
        return null;
    }

    /// <summary>
    /// Setting money when player money changed.
    /// </summary>
    public void OnPlayerMoneyChanged() {

        playerMoney = BR_API.GetMoney();

    }

    private void FixedUpdate() {

        if (!currentPlayerCar)
            return;

        if (controlPlayerVehicle) {

            if (controlPlayerVehicleNow) {

                RCCP_Inputs inputs = new RCCP_Inputs();
                inputs.throttleInput = .35f;

                currentPlayerCar.CarController.canControl = true;
                currentPlayerCar.CarController.Inputs.OverrideInputs(inputs);

            } else {

                currentPlayerCar.CarController.canControl = false;
                currentPlayerCar.CarController.Inputs.DisableOverrideInputs();

            }

        } else {

            controlPlayerVehicleNow = false;

        }

        if (Mathf.Abs(currentPlayerCar.transform.position.z) >= 500f)
            currentPlayerCar.transform.position -= Vector3.forward * 500f;

    }

    private void Update() {

        //  Displaying money text.
        //currencyText.text = "$ " + playerMoney.ToString();

        //  Displaying race points text.
        racePointsText.text = (racePoints > 0 ? "+" : "") + racePoints.ToString();

        //  Displaying player name text.
        //playerNameText.text = BR_API.GetPlayerName();

        //  Displaying stats of the current vehicle. Engine, handling, brake, and maximum speed.
        if (currentPlayerCar) {

            if (currentPlayerCar.CarController.Engine)
                engine.value = Mathf.Lerp(engine.value, Mathf.Lerp(0f, 1f, (currentPlayerCar.CarController.Engine.maximumTorqueAsNM) / 800f), Time.deltaTime * 3f);
            else
                engine.value = 0f;

            if (currentPlayerCar.CarController.Stability)
                handling.value = Mathf.Lerp(handling.value, Mathf.Lerp(0f, 1f, currentPlayerCar.CarController.Stability.steerHelperStrength / .5f), Time.deltaTime * 3f);
            else
                handling.value = 0f;

            if (currentPlayerCar.CarController.AxleManager && currentPlayerCar.CarController.AxleManager.Axles[0])
                brake.value = Mathf.Lerp(brake.value, Mathf.Lerp(0f, 1f, currentPlayerCar.CarController.AxleManager.Axles[0].maxBrakeTorque / 8000f), Time.deltaTime * 3f);
            else
                brake.value = 0f;

            if (currentPlayerCar.CarController.Engine)
                speed.value = Mathf.Lerp(speed.value, Mathf.Lerp(0f, 1f, currentPlayerCar.CarController.Engine.maximumSpeed / 360f), Time.deltaTime * 3f);
            else
                speed.value = 0f;

            // Displaying upgraded stats of the current vehicle. Engine, handling, brake, and maximum speed.
            RCCP_Customizer customizer = currentPlayerCar.CarController.Customizer;
            RCCP_VehicleUpgrade_UpgradeManager upgrader = null;

            if (customizer)
                upgrader = currentPlayerCar.CarController.Customizer.UpgradeManager;

            if (customizer && upgrader && upgrader.Engine)
                engineMax.value = Mathf.Lerp(engineMax.value, Mathf.Lerp(0f, 1f, (upgrader.Engine.defEngine * upgrader.Engine.efficiency) / 800f), Time.deltaTime * 3f);
            else
                engineMax.value = 0f;

            if (customizer && upgrader && upgrader.Handling)
                handlingMax.value = Mathf.Lerp(handlingMax.value, Mathf.Lerp(0f, 1f, (upgrader.Handling.defHandling * upgrader.Handling.efficiency) / .5f), Time.deltaTime * 3f);
            else
                handlingMax.value = 0f;

            if (customizer && upgrader && upgrader.Brake)
                brakeMax.value = Mathf.Lerp(brakeMax.value, Mathf.Lerp(0f, 1f, (upgrader.Brake.defBrake * upgrader.Brake.efficiency) / 8000f), Time.deltaTime * 3f);
            else
                brakeMax.value = 0f;

            if (customizer && upgrader && upgrader.Speed)
                speedMax.value = Mathf.Lerp(speedMax.value, Mathf.Lerp(0f, 1f, (upgrader.Speed.defMaxSpeed * upgrader.Speed.efficiency) / 360f), Time.deltaTime * 3f);
            else
                speedMax.value = 0f;

        } else {

            //  Set stats of the vehicle to 0 if it's not found in the scene.
            engine.value = 0;
            handling.value = 0;
            brake.value = 0;

            engineMax.value = 0;
            handlingMax.value = 0;
            brakeMax.value = 0;

        }

        //  If loading is in progress, set value of the loading slider.
        if (async != null && !async.isDone)
            loadingBar.value = async.progress;

    }

    /// <summary>
    /// Creating all selectable vehicles at once. Disabling all of them.
    /// </summary>
    private void CreateCars() {

        for (int i = 0; i < BR_PlayerCars.Instance.playerCars.Length; i++) {

            //  Spawning.
            RCCP_CarController car = RCCP.SpawnRCC(BR_PlayerCars.Instance.playerCars[i].car.GetComponent<RCCP_CarController>(), spawnPoint.position, spawnPoint.rotation, true, false, true);
            car.Rigid.constraints = RigidbodyConstraints.FreezePositionX;

#if PHOTON_UNITY_NETWORKING

            //  We won't be needing photonview at the main menu, destroying it.
            if (car.TryGetComponent(out PhotonView pV))
                Destroy(pV);

            //  We won't be needing RCCP_PhotonSync at the main menu, destroying it.
            if (car.TryGetComponent(out RCCP_PhotonSync pS))
                Destroy(pS);

#endif

            //  We won't be needing RCCP_LOD at the main menu, destroying it.
            if (car.TryGetComponent(out RCCP_Lod LOD))
                Destroy(LOD.gameObject);

            //  Adding spawned vehicle to the list.
            instantiatedPlayerCars.Add(car.gameObject);

            //  Deactivating the spawned vehicle.
            car.gameObject.SetActive(false);

            //  Setting headlights of the vehicle.
            if (car.Lights)
                car.Lights.lowBeamHeadlights = headlightsEnabled;

        }

        //  Make sure spawned vehicles don't have photon view component with RCCP_PhotonSync.
        StartCoroutine(DoubleCheckForPhotonComponents());

    }

    /// <summary>
    /// Make sure spawned vehicles don't have photon view component with RCCP_PhotonSync.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoubleCheckForPhotonComponents() {

        yield return new WaitForEndOfFrame();

        for (int i = 0; i < instantiatedPlayerCars.Count; i++) {

#if PHOTON_UNITY_NETWORKING

            PhotonView pV = instantiatedPlayerCars[i].GetComponent<PhotonView>();
            RCCP_PhotonSync pS = instantiatedPlayerCars[i].GetComponent<RCCP_PhotonSync>();

            if (pV)
                Destroy(pV);

            if (pS)
                Destroy(pS);

#endif

        }

    }

    /// <summary>
    /// Only enables selected index of the vehicles. If we own the vehicle, buy button will disappear, and select button will appear.
    /// </summary>
    private void SpawnPlayer() {

        //  If price of the vehicle is set to 0, we own the vehicle.
        if (BR_PlayerCars.Instance.playerCars[currentPlayerCarIndex].price <= 0)
            BR_API.UnlockVehicle(currentPlayerCarIndex);

        //  If we own the vehicle, disable the buy button, enable the select and mod buttons.
        //  If we don't own the vehicle, enable the buy button, disable the select and mod buttons.
        if (BR_API.IsOwnedVehicle(currentPlayerCarIndex)) {

            if (priceText)
                priceText.text = "";

            buyCarButton.SetActive(false);

            if (BR_API.GetVehicle() != currentPlayerCarIndex) {

                selectCarButton.SetActive(true);
                selectedButton.SetActive(false);

            } else {

                selectCarButton.SetActive(false);
                selectedButton.SetActive(true);

            }

            modCarPanel.SetActive(true);

        } else {

            buyCarButton.SetActive(true);
            selectCarButton.SetActive(false);
            selectedButton.SetActive(false);
            modCarPanel.SetActive(false);

            if (priceText)
                priceText.text = BR_PlayerCars.Instance.playerCars[currentPlayerCarIndex].price.ToString("F0");

        }

        //  Controlling the locked image.
        lockedImage.SetActive(buyCarButton.activeSelf);

        // Disabling all instantiated vehicles.
        for (int i = 0; i < instantiatedPlayerCars.Count; i++)
            instantiatedPlayerCars[i].SetActive(false);

        // And enabling only selected index.
        instantiatedPlayerCars[currentPlayerCarIndex].SetActive(true);

        // Set current vehicle, load stats, and let mod handler script can proccess this vehicle.
        currentPlayerCar = instantiatedPlayerCars[currentPlayerCarIndex].GetComponent<BR_PlayerManager>();
        RCCP_SceneManager.Instance.activePlayerVehicle = currentPlayerCar.CarController;

        currentPlayerCar.transform.position = spawnPoint.position;
        currentPlayerCar.transform.rotation = spawnPoint.rotation;

        // Loads the player's customization.
        LoadCustomization();

        //  Resetting the modder.
        BR_ModHandler.Instance.ChooseClass(null);

#if PHOTON_UNITY_NETWORKING

        //  We won't be needing photonview at the main menu, destroying it.
        if (currentPlayerCar.TryGetComponent(out PhotonView pV))
            Destroy(pV);

        //  We won't be needing RCCP_PhotonSync at the main menu, destroying it.
        if (currentPlayerCar.TryGetComponent(out RCCP_PhotonSync pS))
            Destroy(pS);

#endif

        //  Loading the vehicle customization.
        StartCoroutine(nameof(LoadCustomizer));

    }

    /// <summary>
    /// Loading the vehicle customization with delay.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadCustomizer() {
        // todo
        yield return new WaitForSeconds(.1f);
        currentPlayerCar.CarController.Customizer.Initialize();

    }

    /// <summary>
    /// Purchases current vehicle.
    /// </summary>
    public void BuyCar() {

        //  If currency is enough, save it and consume money...
        if (BR_API.GetMoney() >= BR_PlayerCars.Instance.playerCars[currentPlayerCarIndex].price) {

            BR_API.UnlockVehicle(currentPlayerCarIndex);
            BR_API.ConsumeMoney(BR_PlayerCars.Instance.playerCars[currentPlayerCarIndex].price);

        } else {

            //  If money is not enough, inform the player.
            BR_UI_Informer.Instance.Info("NOT ENOUGH MONEY! YOU HAVE TO EARN " + (BR_PlayerCars.Instance.playerCars[currentPlayerCarIndex].price - BR_API.GetMoney()).ToString() + " MORE MONEY TO PURCHASE THIS VEHICLE!");
            return;

        }

        //  Spawn the vehicle again.
        SpawnPlayer();

    }

    /// <summary>
    /// Previous index while selecting the vehicles.
    /// </summary>
    public void SelectPrevious() {

        if (currentPlayerCarIndex > 0)
            currentPlayerCarIndex--;
        else
            currentPlayerCarIndex = BR_PlayerCars.Instance.playerCars.Length - 1;

        SpawnPlayer();

    }

    /// <summary>
    /// Next index while selecting the vehicles.
    /// </summary>
    public void SelectNext() {

        if (currentPlayerCarIndex < BR_PlayerCars.Instance.playerCars.Length - 1)
            currentPlayerCarIndex++;
        else
            currentPlayerCarIndex = 0;

        SpawnPlayer();

    }

    /// <summary>
    /// Selects the player vehicle.
    /// </summary>
    public void SelectCar( int index) {

        //BR_API.SetVehicle(currentPlayerCarIndex);
        BR_API.SetVehicle(index);
        SpawnPlayer();

    }

    /// <summary>
    /// Opens up the target UI panel.
    /// </summary>
    /// <param name="activeMenu"></param>
    public void OpenMenu(GameObject activeMenu) {

        if (mainMenu)
            mainMenu.SetActive(false);

        if (carSelectMenu)
            carSelectMenu.SetActive(false);

        if (onlineMenu)
            onlineMenu.SetActive(false);

        if (loadingMenu)
            loadingMenu.SetActive(false);

        if (welcomeMenu)
            welcomeMenu.SetActive(false);

        if (storeMenu)
            storeMenu.SetActive(false);

        if (creditsMenu)
            creditsMenu.SetActive(false);

        if (selectionModeMenu)
            selectionModeMenu.SetActive(false);

        if (selectionSceneOfflineMenu)
            selectionSceneOfflineMenu.SetActive(false);

        if (purchaseConfirmationMenu)
            purchaseConfirmationMenu.SetActive(false);

        if (activeMenu != null)
            activeMenu.SetActive(true);

        //  If it's main menu, enable the vehicle render camera, and background. Otherwise, disable the vehicle render camera and backgrounds.
        if (carSelectMenu && Equals(activeMenu, carSelectMenu)) {

            SpawnPlayer();

        }

        //  If it's main menu, enable the vehicle render camera, and background. Otherwise, disable the vehicle render camera and backgrounds.
        if (mainMenu && Equals(activeMenu, mainMenu)) {

            if (vehicleRenderCamera)
                vehicleRenderCamera.SetActive(true);

            if (vehicleBackgroundCanvas)
                vehicleBackgroundCanvas.SetActive(true);

            if (generalBackgroundImage)
                generalBackgroundImage.SetActive(true);

            if (controlPlayerVehicle)
                controlPlayerVehicleNow = true;

        } else {

            vehicleRenderCamera.SetActive(false);

            if (controlPlayerVehicle)
                controlPlayerVehicleNow = false;

            //  If it's car select menu, not the main menu , enable the vehicle background and disable the general background canvas.
            if (carSelectMenu && Equals(activeMenu, carSelectMenu)) {

                if (vehicleBackgroundCanvas)
                    vehicleBackgroundCanvas.SetActive(true);

                if (generalBackgroundImage)
                    generalBackgroundImage.SetActive(false);

            } else {

                if (vehicleBackgroundCanvas)
                    vehicleBackgroundCanvas.SetActive(false);

                if (generalBackgroundImage)
                    generalBackgroundImage.SetActive(true);

            }

        }

        //  If it's main menu, enable the vehicle render camera, and background. Otherwise, disable the vehicle render camera and backgrounds.
        if (storeMenu && Equals(activeMenu, storeMenu))
            storeConfirmationButtons.SetActive(false);

    }

    /// <summary>
    /// Selects the scene and saves index of it.
    /// </summary>
    /// <param name="sceneIndex"></param>
    public void SelectScene(int sceneIndex) {

        BR_API.SetScene(sceneIndex);

    }

    /// <summary>
    /// Opens the selected scene with given index.
    /// </summary>
    /// <param name="sceneIndex"></param>
    public void OpenScene(int sceneIndex) {

        BR_API.SetScene(sceneIndex);
        async = SceneManager.LoadSceneAsync(sceneIndex);
        OpenMenu(loadingMenu);

    }

    /// <summary>
    /// Opens the selected scene with latest saved one.
    /// </summary>
    /// <param name="sceneIndex"></param>
    public void OpenScene() {

        int sceneIndex = BR_API.GetScene();
        async = SceneManager.LoadSceneAsync(sceneIndex);
        OpenMenu(loadingMenu);

    }

    /// <summary>
    /// Selects the online / offline mode with selector.
    /// </summary>
    public void SelectMode(BR_UI_HorizontalSelector selector) {

        BR_API.SetMultiplayer(selector.value == 0);

    }

    /// <summary>
    /// Selects the online / offline mode.
    /// </summary>
    public void SelectMode(bool state) {

        BR_API.SetMultiplayer(state);

    }

    /// <summary>
    /// Selects the game type with dropdown.
    /// </summary>
    public void SelectGameType(TMP_Dropdown dropdown) {

        BR_API.SetGameType(dropdown.value);

    }

    /// <summary>
    /// Selects the game type with selector.
    /// </summary>
    public void SelectGameType(BR_UI_HorizontalSelector selector) {

        BR_API.SetGameType(selector.value);

    }

    /// <summary>
    /// Selects the game type.
    /// </summary>
    public void SelectGameType(int index) {

        BR_API.SetGameType(index);

    }

    /// <summary>
    /// Selects the bots.
    /// </summary>
    public void SelectBots(BR_UI_HorizontalSelector selector) {

        BR_API.SetBots(selector.value == 1);

    }

    /// <summary>
    /// Selects the bots.
    /// </summary>
    public void SelectBots(bool state) {

        BR_API.SetBots(state);

    }

    /// <summary>
    /// Selects the bots amount.
    /// </summary>
    public void SelectBotsAmount(BR_UI_HorizontalSelector selector) {

        BR_API.SetBotsAmount(selector.value + 1);

    }

    /// <summary>
    /// Selects the laps amount.
    /// </summary>
    public void SelectLapsAmount(BR_UI_HorizontalSelector selector) {

        BR_API.SetLapsAmount(selector.value + 1);

    }

    /// <summary>
    /// Selects and spawns the last selected vehicle as the player vehicle.
    /// </summary>
    public void SelectDefaultVehicle() {

        currentPlayerCarIndex = BR_API.GetVehicle();
        BR_API.SetVehicle(currentPlayerCarIndex);
        SpawnPlayer();

    }

    /// <summary>
    /// Closes the game.
    /// </summary>
    public void Quit() {

        System.Diagnostics.Process.GetCurrentProcess().Kill();

    }

    /// <summary>
    /// Deletes all save data and restarts the scene.
    /// </summary>
    public void DeletePlayerPrefs() {

        BR_API.Delete();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    /// <summary>
    /// Adds money for testing purposes.
    /// </summary>
    public void AddMoney() {

        BR_API.AddMoney(10000);

    }

    /// <summary>
    /// Sets the player name with input field.
    /// </summary>
    /// <param name="inputField"></param>
    public void SetPlayerName(TMP_InputField inputField) {

        //  Limiting the input at least 4 characters.
        if (inputField.text.Length < 4) {

            BR_UI_Informer.Instance.Info("4 CHARACTERS NEEDED AT LEAST");
            return;

        }

        //  Setting the player name.
        BR_API.SetPlayerName(inputField.text);

        //  Opening the main menu.
        OpenMenu(mainMenu);

    }

    private void OnDisable() {

        //  Not listening.
        BR_API.OnPlayerMoneyChanged -= OnPlayerMoneyChanged;

    }

    /// <summary>
    /// Opens target URL to be used on about us, privacy policy, and more.
    /// </summary>
    /// <param name="URL"></param>
    public void OpenURL(string URL) {

        Application.OpenURL(URL);

    }

    /// <summary>
    /// Attempt to purchase the latest selected store item.
    /// </summary>
    /// <param name="storeItemButton"></param>
    public void AttemptPurchaseStoreItem(BR_UI_StoreItemButton storeItemButton) {

        //  Enabling the confirmation panel.
        if (purchaseConfirmationMenu)
            purchaseConfirmationMenu.SetActive(true);

        //  Assigning the last selexted store item.
        lastSelectedStoreItem = storeItemButton;

        //  Prize text.
        storeItemPriceText.text = "$ " + lastSelectedStoreItem.price.ToString();

        //  Enabling the confirmation buttons.
        if (storeConfirmationButtons)
            storeConfirmationButtons.SetActive(true);

    }

    /// <summary>
    /// Purchases the latest selected store item.
    /// </summary>
    public void ConfirmPurchaseStoreItem() {

        //  Disabling the confirmation panel.
        if (purchaseConfirmationMenu)
            purchaseConfirmationMenu.SetActive(false);

        //  Prize text.
        storeItemPriceText.text = "";

        //  Disabling the confirmation buttons.
        if (storeConfirmationButtons)
            storeConfirmationButtons.SetActive(false);

        //  Return if last selected store item is null.
        if (!lastSelectedStoreItem)
            return;

        //  Adding prize money.
        BR_API.AddMoney(lastSelectedStoreItem.amount);

        //  Resetting last selected store item.
        lastSelectedStoreItem = null;

    }

    /// <summary>
    /// Cancels to purchase the latest selected store item.
    /// </summary>
    public void CancelPurchaseStoreItem() {

        //  Disabling the confirmation panel.
        if (purchaseConfirmationMenu)
            purchaseConfirmationMenu.SetActive(false);

        //  Prize text.
        storeItemPriceText.text = "";

        //  Disabling the confirmation buttons.
        if (storeConfirmationButtons)
            storeConfirmationButtons.SetActive(false);

        //  Resetting last selected store item.
        lastSelectedStoreItem = null;

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


    //Vs setup

           private void setupScreens()
        {
            //onlineMenu.SetActive(false);
            //NoOpponentScreen.SetActive(false);
            vsBackBtn.gameObject.SetActive(false);
            vsBackBtn.interactable = true;
            vsUsersParent.SetActive(false);
            vsScreen.SetActive(true);
        }

        public void VsBackBtn()
        {
            vsBackBtn.interactable = false;
            vsMsgText.text = "Leaveing room...";
            if (noOpponentFound != null)
            {
                StopCoroutine(noOpponentFound);
            }
            //ResetBotSettings();
            PhotonNetwork.LeaveRoom();
        }

        public void VsOnLeftRoom()
        {
            vsBackBtn.gameObject.SetActive(false);
            vsBackBtn.interactable = true;
            vsUsersParent.SetActive(false);
            vsScreen.SetActive(false);
            //NoOpponentScreen.SetActive(false);
            //roomScreen.SetActive(true);
        }

        public void VsJoinedRoom()
        {
            
            //vsTotalBetText.text = PhotonController.Instance.roomEntryPice.ToString("###,###,###")+"LKR";

            //vsHomeUserBetText.text = PhotonController.Instance.roomEntryPice.ToString("###,###,###");
            //vsAwayUserBetText.text = PhotonController.Instance.roomEntryPice.ToString("###,###,###");


            vsBackBtn.gameObject.SetActive(true);
            vsBackBtn.interactable = true;
            vsMsgText.text = "Waiting opponent...";
            vsUsersParent.SetActive(true);
            
            //AudioManager.Instance.PlayRollingSound();
            OpponentStatus = 0;
            if (PhotonNetwork.PlayerList.Length == 1)
            {
                
                vsHomeUsernameText.text = PhotonNetwork.PlayerList[0].NickName;
                /*vsHomeUsernameText.text = PhotonNetwork.PlayerList[0].NickName;
                Debug.Log("UsernameAt1" + PhotonNetwork.PlayerList[0].NickName);
                vsUsersParent.transform.GetChild(0).GetComponent<Image>().sprite = avatars[(int)PhotonNetwork.PlayerList[0].CustomProperties["avatar"]];*/

                if (noOpponentFound == null)
                {
                    noOpponentFound = NoOpponentCountdown();
                }
                else
                {
                    StopCoroutine(noOpponentFound);
                    noOpponentFound = NoOpponentCountdown();
                }

                StartCoroutine(noOpponentFound);
            }
            else if (PhotonNetwork.PlayerList.Length == 2)
            {
               
                vsHomeUsernameText.text = PhotonNetwork.PlayerList[1].NickName;


                if (OpponentFound == null)
                {
                    OpponentFound = GowithOpponentCountdown();
                }
                else
                {
                    StopCoroutine(OpponentFound);
                    OpponentFound = GowithOpponentCountdown();
                }

                StartCoroutine(OpponentFound);
            }
        }

        IEnumerator GowithOpponentCountdown()
        {
            //dioManager.Instance.StopRollingSound();
            //vsHomeUsernameText.text = PhotonNetwork.PlayerList[0].NickName;
            //vsTotalBetText.text = "";
            yield return new WaitForSeconds(3f);
            //vsAwayUsernameText.text = PhotonNetwork.PlayerList[0].NickName;
            //vsTotalBetText.text = (PhotonController.Instance.roomEntryPice * 2).ToString("###,###,###");
            SetOS();
            Debug.Log("Opponent found");

            //Debug.Log("UsernameAt2" + PhotonNetwork.PlayerList[0].NickName);
            //vsAwayUsernameText.text = PhotonNetwork.PlayerList[1].NickName;

            //vsUsersParent.transform.GetChild(0).GetComponent<Image>().sprite = avatars[(int)PhotonNetwork.PlayerList[0].CustomProperties["avatar"]];
            //vsUsersParent.transform.GetChild(1).GetComponent<Image>().sprite = avatars[(int)PhotonNetwork.PlayerList[1].CustomProperties["avatar"]];
            yield return new WaitForSeconds(3f);
            VsStartMatch();


            /*
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                {
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    PhotonNetwork.CurrentRoom.IsVisible = false;

                    vsHomeUsernameText.text = PhotonNetwork.PlayerList[0].NickName;
                    Debug.Log("UsernameAt2" + PhotonNetwork.PlayerList[0].NickName);
                    vsAwayUsernameText.text = PhotonNetwork.PlayerList[1].NickName;
                    VsStartMatch();
                }

                */
        }

        IEnumerator NoOpponentCountdown()
        {
            yield return new WaitForSeconds(30f);
            //AudioManager.Instance.StopRollingSound();
            Debug.Log("No opponent found");
            //NoOpponentScreen.SetActive(true);
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }

public void WaitForOpponentBtn()
        {
            if (noOpponentFound == null)
            {
                noOpponentFound = KeepWaiting();
            }
            else
            {
                StopCoroutine(noOpponentFound);
                noOpponentFound = KeepWaiting();
            }

            StartCoroutine(noOpponentFound);
        }

        IEnumerator KeepWaiting()
        {
            PhotonNetwork.LeaveRoom();
            //ResetBotSettings();

            //waitingPanel.SetActive(true);
            yield return new WaitForSeconds(5f);

            //waitingPanel.SetActive(false);

            //string roomTmp = PlayerPrefs.GetString("roomName");
            //RoomsBtn(roomTmp);
        }

        public void leaveBtn()
        {
            Debug.Log("Leeaving RRoomMM4");
            //waitingPanel.SetActive(false);
            //NoOpponentScreen.SetActive(false);
            if (noOpponentFound != null)
            {
                StopCoroutine(noOpponentFound);
            }

            PhotonNetwork.LeaveRoom();
        }

         public void VsOnPlayerJoinedRoom()
        {
                /* if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                {
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    PhotonNetwork.CurrentRoom.IsVisible = false;

                    if (OpponentJoined == null)
                    {
                        OpponentJoined = OpponentJoinedCountdown();
                    }
                    else
                    {
                        StopCoroutine(OpponentJoined);
                        OpponentJoined = OpponentJoinedCountdown();
                    }

                    StartCoroutine(OpponentJoined);
                }*/

                     if (OpponentJoined == null)
                    {
                        OpponentJoined = OpponentJoinedCountdown();
                    }
                    else
                    {
                        StopCoroutine(OpponentJoined);
                        OpponentJoined = OpponentJoinedCountdown();
                    }

                    StartCoroutine(OpponentJoined);

        }

        IEnumerator OpponentJoinedCountdown()
        {
            SetOS();
            
            /*vsHomeUsernameText.text = PhotonNetwork.PlayerList[0].NickName;
            Debug.Log("UsernameAt2" + PhotonNetwork.PlayerList[0].NickName);
            vsAwayUsernameText.text = PhotonNetwork.PlayerList[1].NickName;
    */

            yield return new WaitForSeconds(5f);
            VsStartMatch();
        }

        void VsStartMatch()
        {
            if (PhotonNetwork.PlayerList.Length == 2)
            {
                if (noOpponentFound != null)
                {
                    StopCoroutine(noOpponentFound);
                }
                if (OpponentFound != null)
                {
                    StopCoroutine(OpponentFound);
                }
                if (OpponentJoined != null)
                {
                    StopCoroutine(OpponentJoined);
                }
            }

            int currentCoin = tempCoin;
            //int newCoin = currentCoin - PhotonController.Instance.roomEntryPice;
            int newCoin = currentCoin - BR_PhotonLobbyManager.Instance.roomEntryPice;

            StartCoroutine(UpdateUserProfileCoroutine_PUT(newCoin));

            //PlayerPrefs.SetInt("coin", PlayerPrefs.GetInt("coin") - PhotonController.Instance.roomEntryPice);
            PlayerPrefs.SetInt("coin", PlayerPrefs.GetInt("coin"));
            PlayerPrefs.Save();

            vsBackBtn.gameObject.SetActive(false);

            
            

            LeanTween.value(BR_PhotonLobbyManager.Instance.roomEntryPice, 0, 1f).setOnUpdate((float val) =>
            {
                vsHomeUserBetText.text = "" + (int)val;
                vsAwayUserBetText.text = "" + (int)val;
            });

            LeanTween.value(currentCoin, newCoin, 1f).setOnUpdate((float val) =>
            {
                if (val < 1)
                {
                    coinText.text = "0";
                }
                else
                {
                    coinText.text = FormatCurrency((int)val);
                }
            });

            /*LeanTween.value(0, PhotonController.Instance.roomEntryPice * 2, 1f).setOnUpdate((float val) =>
            {
                vsTotalBetText.text = "" + (int)val;
            }).setOnComplete(() =>
            {
                UpdateCurrencyText();
                if (PhotonNetwork.IsMasterClient)
                {
                    SceneManager.LoadScene("Game");
                }
            });*/

            if (PhotonNetwork.IsMasterClient)
            {
                //SceneManager.LoadScene("BR_Scene_1");
                PhotonNetwork.LoadLevel(BR_API.GetScene());
            }
        }

        private int _opponentStatus;

        public int OpponentStatus
        {
            get { return _opponentStatus; }
            set
            {
                _opponentStatus = value;

                if (_opponentStatus == 0)
                {
                    dp.Play("0");
                    coinflow.Play("0");
                    coinflow1.Play("0");
                    numlaod.Play("0");
                    nameload.Play("0");
                    coinBonus.Play("0");
                    //vsTotalBetText.text = PhotonController.Instance.roomEntryPice.ToString("###,###,###");
                    vsAwayUsernameText.text = "";
                }
                else if (_opponentStatus == 1)
                {
                    //AudioManager.Instance.PlayCoinCollectSound();
                    dp.Play("1");
                    coinflow.Play("1");
                    coinflow1.Play("1");
                    numlaod.Play("1");
                    nameload.Play("0");
                    coinBonus.Play("1");
                    //vsTotalBetText.text = "";
                    LeanTween.value(BR_PhotonLobbyManager.Instance.roomEntryPice, BR_PhotonLobbyManager.Instance.roomEntryPice * 2, 1f).setOnUpdate((float val) =>
                    {
                        vsTotalBetText.text = "" + (int)val;
                    }).setOnComplete(() =>
                    {
                        UpdateCurrencyText();

                    });



                    /*if (PhotonNetwork.PlayerList.Length == 1)
                        vsAwayUsernameText.text = "Name";
                    else if (PhotonNetwork.PlayerList.Length == 2)
                        vsAwayUsernameText.text = PhotonNetwork.PlayerList[1].NickName;
                    */


                    /*vsHomeUsernameText.text = PhotonNetwork.PlayerList[0].NickName;
                    Debug.Log("UsernameAt2" + PhotonNetwork.PlayerList[0].NickName);
                    vsAwayUsernameText.text = PhotonNetwork.PlayerList[1].NickName;
            */



                }
                else if (_opponentStatus == 2)
                {
                    dp.Play("1");
                    coinflow.Play("0");
                    coinflow1.Play("0");
                    numlaod.Play("0");
                    nameload.Play("1");
                    coinBonus.Play("1");
                    vsAwayUsernameText.text = "Name";
                    //vsTotalBetText.text = (PhotonController.Instance.roomEntryPice * 2).ToString("###,###,###");
                }
                else if (_opponentStatus == 3)
                {
                    dp.Play("1");
                    coinflow.Play("0");
                    coinflow1.Play("0");
                    numlaod.Play("0");
                    nameload.Play("1");
                    coinBonus.Play("1");
                }
            }
        }

        public void endNumberload()
        {
            OpponentS = 2;
            OpponentStatus = 2;
        }

            public void SetOS()
        {
                OpponentStatus = 1;
        }

    
}
