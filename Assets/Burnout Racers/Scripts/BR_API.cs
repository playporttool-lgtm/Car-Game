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

/// <summary>
/// Get, add, consume money. Lock, unlock vehicles, get player name, event on money change, and more...
/// </summary>
public class BR_API {

    /// <summary>
    /// Event when player money changes...
    /// </summary>
    public delegate void onPlayerMoneyChanged();
    public static event onPlayerMoneyChanged OnPlayerMoneyChanged;

    /// <summary>
    /// First gameplay passed. Used to display welcome screen.
    /// </summary>
    public static void SetFirstGameplay() {

        BR_SaveGameManager.saveData.firstGameplay = false;

    }

    /// <summary>
    /// Is this the first gameplay?
    /// </summary>
    /// <returns></returns>
    public static bool IsFirstGameplay() {

        Load();
        return BR_SaveGameManager.saveData.firstGameplay;

    }

    /// <summary>
    /// Is multiplayer mode selected?
    /// </summary>
    /// <returns></returns>
    public static bool IsMultiplayer() {

        Load();
        return RCCP_PlayerPrefsX.GetBool("Multiplayer", false);

    }

    /// <summary>
    /// Sets the multiplayer mode.
    /// </summary>
    /// <returns></returns>
    public static void SetMultiplayer(bool state) {

        Load();
        RCCP_PlayerPrefsX.SetBool("Multiplayer", state);

    }

    /// <summary>
    /// Sets the bots on or off.
    /// </summary>
    /// <param name="state"></param>
    public static void SetBots(bool state) {

        RCCP_PlayerPrefsX.SetBool("Bots", state);

    }

    /// <summary>
    /// Gets the latest bot selection state.
    /// </summary>
    /// <param name="state"></param>
    public static bool GetBots() {

        return RCCP_PlayerPrefsX.GetBool("Bots", false);

    }

    /// <summary>
    /// Sets the bots amount.
    /// </summary>
    /// <param name="amount"></param>
    public static void SetBotsAmount(int amount) {

        PlayerPrefs.SetInt("BotsAmount", amount);

    }

    /// <summary>
    /// Gets the latest selected bots amount.
    /// </summary>
    /// <returns></returns>
    public static int GetBotsAmount() {

        return PlayerPrefs.GetInt("BotsAmount", 3);

    }

    /// <summary>
    /// Sets the taarget lap amount.
    /// </summary>
    /// <param name="amount"></param>
    public static void SetLapsAmount(int amount) {

        Load();
        BR_SaveGameManager.saveData.targetLaps = amount;
        Save();

    }

    /// <summary>
    /// Gets the latest selected laps amount.
    /// </summary>
    /// <returns></returns>
    public static int GetLapsAmount() {

        Load();
        return BR_SaveGameManager.saveData.targetLaps;

    }

    /// <summary>
    /// Sets player name.
    /// </summary>
    /// <param name="newPlayerName"></param>
    public static void SetPlayerName(string newPlayerName) {

        Load();
        BR_SaveGameManager.saveData.playerName = newPlayerName;
        BR_SaveGameManager.saveData.firstGameplay = false;
        Save();

    }

    /// <summary>
    /// Gets player name.
    /// </summary>
    public static string GetPlayerName() {

        Load();
        return BR_SaveGameManager.saveData.playerName;

    }

    /// <summary>
    /// Gets the current money as int.
    /// </summary>
    /// <returns></returns>
    public static int GetMoney() {

        Load();
        return BR_SaveGameManager.saveData.playerMoney;

    }

    /// <summary>
    /// Consumes money.
    /// </summary>
    /// <param name="consume"></param>
    public static void ConsumeMoney(int consume) {

        Load();
        BR_SaveGameManager.saveData.playerMoney -= consume;
        BR_SaveGameManager.saveData.firstGameplay = false;
        Save();

        if (OnPlayerMoneyChanged != null)
            OnPlayerMoneyChanged();

    }

    /// <summary>
    /// Adds money.
    /// </summary>
    /// <param name="add"></param>
    public static void AddMoney(int add) {

        Load();
        BR_SaveGameManager.saveData.playerMoney += add;
        BR_SaveGameManager.saveData.firstGameplay = false;
        Save();

        if (OnPlayerMoneyChanged != null)
            OnPlayerMoneyChanged();

    }

    /// <summary>
    /// Unlocks the target car.
    /// </summary>
    /// <param name="carIndex"></param>
    public static void UnlockVehicle(int carIndex) {

        Load();

        if (!BR_SaveGameManager.saveData.ownedVehicles.Contains(carIndex))
            BR_SaveGameManager.saveData.ownedVehicles.Add(carIndex);

        BR_SaveGameManager.saveData.firstGameplay = false;

        Save();

    }

    /// <summary>
    /// Unlocks all cars.
    /// </summary>
    public static void UnlockAllVehicles() {

        for (int i = 0; i < BR_PlayerCars.Instance.playerCars.Length; i++) {

            if (BR_PlayerCars.Instance.playerCars[i] != null)
                UnlockVehicle(i);

        }

    }

    /// <summary>
    /// Locks the target car.
    /// </summary>
    /// <param name="carIndex"></param>
    public static void LockVehicle(int carIndex) {

        Load();

        if (BR_SaveGameManager.saveData.ownedVehicles.Contains(carIndex))
            BR_SaveGameManager.saveData.ownedVehicles.RemoveAt(carIndex);

        BR_SaveGameManager.saveData.firstGameplay = false;

        Save();

    }

    /// <summary>
    /// Is this car owned by the player?
    /// </summary>
    /// <param name="carIndex"></param>
    /// <returns></returns>
    public static bool IsOwnedVehicle(int carIndex) {

        Load();

        if (BR_SaveGameManager.saveData.ownedVehicles.Contains(carIndex))
            return true;

        return false;

    }

    /// <summary>
    /// Gets the latest selected player vehicle.
    /// </summary>
    /// <returns></returns>
    public static int GetVehicle() {

        //UnlockVehicle(BR_Settings.Instance.defaultSelectedVehicleIndex);
        Load();
        return BR_SaveGameManager.saveData.selectedVehicle;

    }

    /// <summary>
    /// Sets the target vehicle as player vehicle.
    /// </summary>
    /// <param name="vehicleIndex"></param>
    public static void SetVehicle(int vehicleIndex) {

        Load();
        BR_SaveGameManager.saveData.selectedVehicle = vehicleIndex;
        BR_SaveGameManager.saveData.firstGameplay = false;
        Save();

    }

    /// <summary>
    /// Sets the game type.
    /// </summary>
    /// <param name="gameTypeIndex"></param>
    public static void SetGameType(int gameTypeIndex) {

        Load();
        BR_SaveGameManager.saveData.selectedGameType = gameTypeIndex;
        Save();

    }

    /// <summary>
    /// Gets the total race points as an int.
    /// </summary>
    /// <returns></returns>
    public static int GetRacePoints() {

        Load();
        return BR_SaveGameManager.saveData.racePoints;

    }

    /// <summary>
    /// Consumes race points.
    /// </summary>
    /// <param name="consume"></param>
    public static void ConsumeRacePoints(int consume) {

        Load();
        BR_SaveGameManager.saveData.racePoints -= consume;
        Save();

    }

    /// <summary>
    /// Adds race points.
    /// </summary>
    /// <param name="add"></param>
    public static void AddRacePoints(int add) {

        Load();
        BR_SaveGameManager.saveData.racePoints += add;
        Save();

    }

    /// <summary>
    /// Sets the target scene to load.
    /// </summary>
    /// <param name="sceneIndex"></param>
    public static void SetScene(int sceneIndex) {

        Load();
        BR_SaveGameManager.saveData.selectedScene = sceneIndex;
        BR_SaveGameManager.saveData.firstGameplay = false;
        Save();

    }

    /// <summary>
    /// Gets the latest selected scene.
    /// </summary>
    /// <returns></returns>
    public static int GetScene() {

        Load();
        return BR_SaveGameManager.saveData.selectedScene;

    }

    /// <summary>
    /// Loads the latest selected scene.
    /// </summary>
    public static void StartScene() {

        UnityEngine.SceneManagement.SceneManager.LoadScene(GetScene());

    }

    /// <summary>
    /// Get the game type. 0 is race, 1 is practice.
    /// </summary>
    /// <returns></returns>
    public static int GetGameType() {

        Load();
        return BR_SaveGameManager.saveData.selectedGameType;

    }

    /// <summary>
    /// Gets the current scene's total time.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    public static float GetBestCurrentTimeTotal(string sceneName) {

        return PlayerPrefs.GetFloat("CurrentTime_Total" + sceneName, -1f);

    }

    /// <summary>
    /// Sets the current scene's total time.
    /// </summary>
    /// <param name="timer"></param>
    /// <param name="sceneName"></param>
    public static void SetBestCurrentTimeTotal(float timer, string sceneName) {

        float currentBest = GetBestCurrentTimeTotal(sceneName);

        if (currentBest < 0) {

            PlayerPrefs.SetFloat("CurrentTime_Total" + sceneName, timer);
            return;

        }

        PlayerPrefs.SetFloat("CurrentTime_Total" + sceneName, timer);

    }

    /// <summary>
    /// Gets the current scene's lap time.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    public static float GetBestCurrentLapTime(string sceneName) {

        return PlayerPrefs.GetFloat("CurrentTime_Lap" + sceneName, -1f);

    }

    /// <summary>
    /// Sets the current scene's lap time.
    /// </summary>
    /// <param name="timer"></param>
    /// <param name="sceneName"></param>
    public static void SetBestCurrentLapTime(float timer, string sceneName) {

        float currentBest = GetBestCurrentLapTime(sceneName);

        if (currentBest < 0) {

            PlayerPrefs.SetFloat("CurrentTime_Lap" + sceneName, timer);
            return;

        }

        if (timer < currentBest)
            PlayerPrefs.SetFloat("CurrentTime_Lap" + sceneName, timer);

    }

    /// <summary>
    /// Gets the current scene's best lap time.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    public static float GetBestLapTime(string sceneName) {

        return PlayerPrefs.GetFloat("Time_Lap" + sceneName, -1f);

    }

    /// <summary>
    /// Sets the current scene's best lap time.
    /// </summary>
    /// <param name="timer"></param>
    /// <param name="sceneName"></param>
    public static void SetBestLapTime(float timer, string sceneName) {

        float currentBest = GetBestLapTime(sceneName);

        if (currentBest < 0) {

            PlayerPrefs.SetFloat("Time_Lap" + sceneName, timer);
            return;

        }

        if (timer < currentBest)
            PlayerPrefs.SetFloat("Time_Lap" + sceneName, timer);

    }

    /// <summary>
    /// Gets the current scene's total time.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    public static float GetBestTimeTotal(string sceneName) {

        return PlayerPrefs.GetFloat("Time_Total" + sceneName, -1f);

    }

    /// <summary>
    /// Sets the current scene's total time.
    /// </summary>
    /// <param name="timer"></param>
    /// <param name="sceneName"></param>
    public static void SetBestTimeTotal(float timer, string sceneName) {

        float currentBest = GetBestTimeTotal(sceneName);

        if (currentBest < 0) {

            PlayerPrefs.SetFloat("Time_Total" + sceneName, timer);
            return;

        }

        if (timer < currentBest)
            PlayerPrefs.SetFloat("Time_Total" + sceneName, timer);

    }

    /// <summary>
    /// Resets the current scene's current total time.
    /// </summary>
    /// <param name="sceneName"></param>
    public static void ResetBestCurrentTimeTotal(string sceneName) {

        PlayerPrefs.DeleteKey("CurrentTime_Total" + sceneName);

    }

    /// <summary>
    /// Resets the current scene's current lap time.
    /// </summary>
    /// <param name="sceneName"></param>
    public static void ResetBestCurrentLapTime(string sceneName) {

        PlayerPrefs.DeleteKey("CurrentTime_Lap" + sceneName);

    }

    /// <summary>
    /// Saves the player progress.
    /// </summary>
    public static void Save() {

        BR_SaveGameManager.Save();

    }

    /// <summary>
    /// Loads the player progress.
    /// </summary>
    public static void Load() {

        BR_SaveGameManager.Load();

        if (BR_SaveGameManager.saveData.firstGameplay) {

            BR_SaveGameManager.saveData.playerMoney = BR_Settings.Instance.startMoney;

        }

    }

    /// <summary>
    /// Deletes the player progress.
    /// </summary>
    public static void Delete() {

        BR_SaveGameManager.Delete();
        PlayerPrefs.DeleteAll();

    }

}
