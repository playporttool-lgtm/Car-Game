//----------------------------------------------
//			Burnout Racers
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
//
//----------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
#endif

#if PHOTON_UNITY_NETWORKING

/// <summary>
/// Race position manager. It will track all racers positions.
/// </summary>
public class BR_RacePositionManager : MonoBehaviourPun {

    private static BR_RacePositionManager instance;

    /// <summary>
    /// Instance.
    /// </summary>
    public static BR_RacePositionManager Instance {

        get {

            if (instance == null)
                instance = FindFirstObjectByType<BR_RacePositionManager>();

            return instance;

        }

    }

    /// <summary>
    /// Gameplay manager instance.
    /// </summary>
    public BR_GameplayManager GameplayManager {

        get {

            if (gameplayManager == null)
                gameplayManager = BR_GameplayManager.Instance;

            return gameplayManager;

        }

    }
    private BR_GameplayManager gameplayManager;

    /// <summary>
    /// List of all racers.
    /// </summary>
    private List<BR_RacePositioner> racers = new List<BR_RacePositioner>(); // List of the racers.

    /// <summary>
    /// Waypoint container for the race.
    /// </summary>
    public BR_AIWaypointsContainer waypointContainer;     //  Waypoint container.

    /// <summary>
    /// Registers the target vehicle.
    /// </summary>
    /// <param name="registerVehicle"></param>
    public void RegisterVehicle(BR_RacePositioner registerVehicle) {

        if (!racers.Contains(registerVehicle))
            racers.Add(registerVehicle);

        //  Checking the list for null elements.
        CheckList();

        //  If we're not the master client, don't update car positions.
        if (photonView) {

            if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
                return;

        }

        //  Updating all racers' positions.
        UpdateCarPositions();

    }

    /// <summary>
    /// Deregisters the target vehicle.
    /// </summary>
    /// <param name="registerVehicle"></param>
    public void DeRegisterVehicle(BR_RacePositioner registerVehicle) {

        if (racers.Contains(registerVehicle))
            racers.Remove(registerVehicle);

        //  Checking the list for null elements.
        CheckList();

        //  If we're not the master client, don't update car positions.
        if (photonView) {

            if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
                return;

        }

        //  Updating all racers' positions.
        UpdateCarPositions();

    }

    private void Start() {

        //  Finding all racers at the start.
        racers = FindObjectsByType<BR_RacePositioner>(FindObjectsSortMode.None).ToList();

        //  Checking the list for null elements.
        CheckList();

        //  If we're not the master client, don't update car positions.
        if (photonView) {

            if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
                return;

        }

        //  Updating all racers' positions.
        UpdateCarPositions();

    }

    private void Update() {

        //  Checking the list for null elements.
        CheckList();

        if (!GameplayManager.raceStarted)
            return;

        //  If we're not the master client, don't update car positions.
        if (photonView) {

            if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
                return;

        }

        //  Updating all racers' positions.
        UpdateCarPositions();

    }

    /// <summary>
    /// Checking the list for null elements.
    /// </summary>
    private void CheckList() {

        for (int i = 0; i < racers.Count; i++) {

            if (racers[i] == null)
                racers.RemoveAt(i);

        }

    }

    /// <summary>
    /// Updating all racers' positions.
    /// </summary>
    private void UpdateCarPositions() {

        if (waypointContainer == null || waypointContainer.waypoints == null || waypointContainer.waypoints.Count < 1)
            return;

        if (racers == null || racers.Count < 1)
            return;

        for (int i = 0; i < racers.Count; i++) {

            if (racers[i] == null)
                return;

        }

        // Sort the car controllers by their progress along the waypoints
        racers.Sort((car1, car2) => car2.GetProgress(waypointContainer.waypoints).CompareTo(car1.GetProgress(waypointContainer.waypoints)));

        // Update the position for each car
        for (int i = 0; i < racers.Count; i++) {

            if (racers[i] != null)
                racers[i].SetRacePosition(i + 1); // Positions are 1-based

        }

    }

}

#else

/// <summary>
/// Race position manager. It will track all racers positions.
/// </summary>
public class BR_RacePositionManager : MonoBehaviour {

    private static BR_RacePositionManager instance;

    /// <summary>
    /// Instance.
    /// </summary>
    public static BR_RacePositionManager Instance {

        get {

            if (instance == null)
                instance = FindFirstObjectByType<BR_RacePositionManager>();

            return instance;

        }

    }

    /// <summary>
    /// Gameplay manager instance.
    /// </summary>
    public BR_GameplayManager GameplayManager {

        get {

            if (gameplayManager == null)
                gameplayManager = BR_GameplayManager.Instance;

            return gameplayManager;

        }

    }
    private BR_GameplayManager gameplayManager;

    /// <summary>
    /// List of all racers.
    /// </summary>
    private List<BR_RacePositioner> racers = new List<BR_RacePositioner>(); // List of the racers.

    /// <summary>
    /// Waypoint container for the race.
    /// </summary>
    public BR_AIWaypointsContainer waypointContainer;     //  Waypoint container.

    /// <summary>
    /// Registers the target vehicle.
    /// </summary>
    /// <param name="registerVehicle"></param>
    public void RegisterVehicle(BR_RacePositioner registerVehicle) {

        if (!racers.Contains(registerVehicle))
            racers.Add(registerVehicle);

        //  Checking the list for null elements.
        CheckList();

        //  Updating all racers' positions.
        UpdateCarPositions();

    }

    /// <summary>
    /// Deregisters the target vehicle.
    /// </summary>
    /// <param name="registerVehicle"></param>
    public void DeRegisterVehicle(BR_RacePositioner registerVehicle) {

        if (racers.Contains(registerVehicle))
            racers.Remove(registerVehicle);

        //  Checking the list for null elements.
        CheckList();

        //  Updating all racers' positions.
        UpdateCarPositions();

    }

    private void Start() {

        //  Finding all racers at the start.
        racers = FindObjectsByType<BR_RacePositioner>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();

        //  Checking the list for null elements.
        CheckList();

        //  Updating all racers' positions.
        UpdateCarPositions();

    }

    private void Update() {

        //  Checking the list for null elements.
        CheckList();

        if (!GameplayManager.raceStarted)
            return;

        //  Updating all racers' positions.
        UpdateCarPositions();

    }

    /// <summary>
    /// Checking the list for null elements.
    /// </summary>
    private void CheckList() {

        for (int i = 0; i < racers.Count; i++) {

            if (racers[i] == null)
                racers.RemoveAt(i);

        }

    }

    /// <summary>
    /// Updating all racers' positions.
    /// </summary>
    private void UpdateCarPositions() {

        if (waypointContainer == null || waypointContainer.waypoints == null || waypointContainer.waypoints.Count < 1)
            return;

        if (racers == null || racers.Count < 1)
            return;

        for (int i = 0; i < racers.Count; i++) {

            if (racers[i] == null)
                return;

        }

        // Sort the car controllers by their progress along the waypoints
        racers.Sort((car1, car2) => car2.GetProgress(waypointContainer.waypoints).CompareTo(car1.GetProgress(waypointContainer.waypoints)));

        // Update the position for each car
        for (int i = 0; i < racers.Count; i++) {

            if (racers[i] != null)
                racers[i].SetRacePosition(i + 1); // Positions are 1-based

        }

    }

}

#endif
