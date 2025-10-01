//----------------------------------------------
//			Burnout Racers
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
//
//----------------------------------------------

using System.Collections.Generic;
using UnityEngine;

#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
#endif

#if PHOTON_UNITY_NETWORKING

/// <summary>
/// Race positioner must be attached to the root of every racer vehicle. BR_RacePositionManager will be observing and tracking them.
/// </summary>
[RequireComponent(typeof(RCCP_CarController))]
[RequireComponent(typeof(BR_PlayerManager))]
public class BR_RacePositioner : MonoBehaviour, IPunObservable {

    /// <summary>
    /// Race position manager instance.
    /// </summary>
    public BR_RacePositionManager RacePositionSystem {

        get {

            if (racePositionSystem == null)
                racePositionSystem = BR_RacePositionManager.Instance;

            return racePositionSystem;

        }

    }
    private BR_RacePositionManager racePositionSystem;

    /// <summary>
    /// Player manager instance.
    /// </summary>
    public BR_PlayerManager PlayerManager {

        get {

            if (playerManager == null)
                playerManager = GetComponent<BR_PlayerManager>();

            return playerManager;

        }

    }
    private BR_PlayerManager playerManager;

    /// <summary>
    /// PhotonView component.
    /// </summary>
    private PhotonView photonView;

    /// <summary>
    /// Current race position.
    /// </summary>
    public int racePosition = 1;

    /// <summary>
    /// Current lap.
    /// </summary>
    public int lap = 0;

    /// <summary>
    /// Total waypoints passed.
    /// </summary>
    public int currentWaypointIndex = 0;

    /// <summary>
    /// Current waypoint index.
    /// </summary>
    public int m_currentWaypointIndex = 0;

    /// <summary>
    /// Distance to the next waypoint.
    /// </summary>
    public float distanceToNextWaypoint = 0f;

    /// <summary>
    /// Timer to send the RPC for synchronizing race positions.
    /// </summary>
    private float timerToSendRPC = .5f;

    private void Awake() {

        photonView = GetComponent<PhotonView>();

    }

    private void OnEnable() {

        // Registering this vehicle in the race position system.
        if (RacePositionSystem != null)
            RacePositionSystem.RegisterVehicle(this);

    }

    /// <summary>
    /// Sets the race position and synchronizes it over the network.
    /// </summary>
    /// <param name="position"></param>
    public void SetRacePosition(int position) {

        racePosition = position;

        timerToSendRPC -= Time.deltaTime;

        if (timerToSendRPC < 0) {

            timerToSendRPC = .5f;

            if (PhotonNetwork.InRoom && photonView)
                photonView.RPC("SetRacePositionRPC", RpcTarget.All, position);

        }

    }

    /// <summary>
    /// Sets the race position using an RPC method.
    /// </summary>
    /// <param name="position"></param>
    [PunRPC]
    public void SetRacePositionRPC(int position) {

        racePosition = position;

    }

    /// <summary>
    /// Gets the current race position.
    /// </summary>
    /// <returns></returns>
    public int GetRacePosition() {

        return racePosition;

    }

    /// <summary>
    /// Gets the overall progress in the race based on waypoints.
    /// </summary>
    /// <param name="waypoints"></param>
    /// <returns></returns>
    public float GetProgress(List<BR_Waypoint> waypoints) {

        if (waypoints == null)
            return 0f;

        if (waypoints.Count < 2)
            return 0f;

        if (m_currentWaypointIndex > waypoints.Count)
            return 0f;

        if (waypoints[m_currentWaypointIndex] == null)
            return 0f;

        distanceToNextWaypoint = Vector3.Distance(transform.position, waypoints[m_currentWaypointIndex].transform.position);
        return currentWaypointIndex - distanceToNextWaypoint / 1000f; // Adjusting distance impact on progress.

    }

    private void Update() {

        //  If photonview is not ours, return.
        if (photonView && !photonView.IsMine)
            return;

        //  If no race position manager found, return.
        if (RacePositionSystem == null)
            return;

        if (m_currentWaypointIndex > (RacePositionSystem.waypointContainer.waypoints.Count - 1))
            m_currentWaypointIndex = RacePositionSystem.waypointContainer.waypoints.Count - 1;

        //  If the vehicle gets closer to the next waypoint, increment the waypoint index.
        if (Vector3.Distance(transform.position, RacePositionSystem.waypointContainer.waypoints[m_currentWaypointIndex].transform.position) < RacePositionSystem.waypointContainer.waypoints[m_currentWaypointIndex].radius) {

            m_currentWaypointIndex = (m_currentWaypointIndex + 1) % RacePositionSystem.waypointContainer.waypoints.Count;

            if (m_currentWaypointIndex == 0)
                lap++;

        }

        if (m_currentWaypointIndex > (RacePositionSystem.waypointContainer.waypoints.Count - 1))
            m_currentWaypointIndex = RacePositionSystem.waypointContainer.waypoints.Count - 1;

        //  If the vehicle gets closer to the next waypoint, increase the current waypoint index.
        if (Vector3.Distance(transform.position, RacePositionSystem.waypointContainer.waypoints[currentWaypointIndex - ((RacePositionSystem.waypointContainer.waypoints.Count - 1) * lap)].transform.position) < RacePositionSystem.waypointContainer.waypoints[currentWaypointIndex - ((RacePositionSystem.waypointContainer.waypoints.Count - 1) * lap)].radius)
            currentWaypointIndex += 1;

    }

    /// <summary>
    /// Checks if the vehicle is going the wrong way.
    /// </summary>
    /// <returns></returns>
    public bool CheckWrongWay() {

        if (!PlayerManager.CarController.canControl)
            return false;

        if (RacePositionSystem == null)
            return false;

        if (!RacePositionSystem.waypointContainer)
            return false;

        if (RacePositionSystem.waypointContainer.waypoints == null)
            return false;

        if (RacePositionSystem.waypointContainer.waypoints.Count < 1)
            return false;

        if (m_currentWaypointIndex > RacePositionSystem.waypointContainer.waypoints.Count)
            return false;

        if (m_currentWaypointIndex > (RacePositionSystem.waypointContainer.waypoints.Count - 1))
            m_currentWaypointIndex = RacePositionSystem.waypointContainer.waypoints.Count - 1;

        if (Vector3.Dot((RacePositionSystem.waypointContainer.waypoints[m_currentWaypointIndex].transform.position - transform.position).normalized, transform.forward) < 0f)
            return true;

        return false;

    }

    private void OnDisable() {

        //  Unregister the vehicle from the race position system.
        if (RacePositionSystem != null)
            RacePositionSystem.DeRegisterVehicle(this);

    }

    /// <summary>
    /// Handles synchronization of variables across the network.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="info"></param>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {

        if (stream.IsWriting) {

            stream.SendNext(currentWaypointIndex);
            stream.SendNext(m_currentWaypointIndex);
            stream.SendNext(distanceToNextWaypoint);
            stream.SendNext(lap);

        } else if (stream.IsReading) {

            currentWaypointIndex = (int)stream.ReceiveNext();
            m_currentWaypointIndex = (int)stream.ReceiveNext();
            distanceToNextWaypoint = (float)stream.ReceiveNext();
            lap = (int)stream.ReceiveNext();

        }

    }

}

#else

/// <summary>
/// Race positioner must be attached to the root of every racer vehicle. BR_RacePositionManager will be managing them.
/// </summary>
[RequireComponent(typeof(RCCP_CarController))]
[RequireComponent(typeof(BR_PlayerManager))]
public class BR_RacePositioner : MonoBehaviour {

    /// <summary>
    /// Race position manager instance.
    /// </summary>
    public BR_RacePositionManager RacePositionSystem {

        get {

            if (racePositionSystem == null)
                racePositionSystem = BR_RacePositionManager.Instance;

            return racePositionSystem;

        }

    }
    private BR_RacePositionManager racePositionSystem;

    /// <summary>
    /// Player manager instance.
    /// </summary>
    public BR_PlayerManager PlayerManager {

        get {

            if (playerManager == null)
                playerManager = GetComponent<BR_PlayerManager>();

            return playerManager;

        }

    }
    private BR_PlayerManager playerManager;

    /// <summary>
    /// Current race position.
    /// </summary>
    public int racePosition = 1;

    /// <summary>
    /// Current lap.
    /// </summary>
    public int lap = 0;

    /// <summary>
    /// Total waypoints passed.
    /// </summary>
    public int currentWaypointIndex = 0;

    /// <summary>
    /// Current waypoint index.
    /// </summary>
    public int m_currentWaypointIndex = 0;

    /// <summary>
    /// Distance to the next waypoint.
    /// </summary>
    public float distanceToNextWaypoint = 0f;

    private void OnEnable() {

        // Registering this vehicle in the race position system.
        if (RacePositionSystem != null)
            RacePositionSystem.RegisterVehicle(this);

    }

    /// <summary>
    /// Sets the race position.
    /// </summary>
    /// <param name="position"></param>
    public void SetRacePosition(int position) {

        racePosition = position;

    }

    /// <summary>
    /// Get the current race position.
    /// </summary>
    /// <returns></returns>
    public int GetRacePosition() {

        return racePosition;

    }

    /// <summary>
    /// Gets the overall progress in the race based on waypoints.
    /// </summary>
    /// <param name="waypoints"></param>
    /// <returns></returns>
    public float GetProgress(List<BR_Waypoint> waypoints) {

        distanceToNextWaypoint = Vector3.Distance(transform.position, waypoints[m_currentWaypointIndex].transform.position);
        return currentWaypointIndex - distanceToNextWaypoint / 1000f; // Adjusting distance impact on progress.

    }

    private void Update() {

        //  If no race position manager found, return.
        if (RacePositionSystem == null)
            return;

        //  If the vehicle gets closer to the next waypoint, increment the waypoint index.
        if (Vector3.Distance(transform.position, RacePositionSystem.waypointContainer.waypoints[m_currentWaypointIndex].transform.position) < 20f) {

            m_currentWaypointIndex = (m_currentWaypointIndex + 1) % RacePositionSystem.waypointContainer.waypoints.Count;

            if (m_currentWaypointIndex == 0)
                lap++;

        }

        //  If the vehicle gets closer to the next waypoint, increase the current waypoint index.
        if (Vector3.Distance(transform.position, RacePositionSystem.waypointContainer.waypoints[currentWaypointIndex - ((RacePositionSystem.waypointContainer.waypoints.Count - 1) * (lap + 0))].transform.position) < 20f)
            currentWaypointIndex += 1;

    }

    /// <summary>
    /// Checks if the vehicle is going the wrong way.
    /// </summary>
    /// <returns></returns>
    public bool CheckWrongWay() {

        if (!PlayerManager.CarController.canControl)
            return false;

        if (RacePositionSystem == null)
            return false;

        if (!RacePositionSystem.waypointContainer)
            return false;

        if (RacePositionSystem.waypointContainer.waypoints == null)
            return false;

        if (RacePositionSystem.waypointContainer.waypoints.Count < 1)
            return false;

        if (m_currentWaypointIndex > RacePositionSystem.waypointContainer.waypoints.Count)
            return false;

        if (Vector3.Dot((RacePositionSystem.waypointContainer.waypoints[m_currentWaypointIndex].transform.position - transform.position).normalized, transform.forward) < 0f)
            return true;

        return false;

    }

    private void OnDisable() {

        //  Unregister the vehicle from the race position system.
        if (RacePositionSystem != null)
            RacePositionSystem.DeRegisterVehicle(this);

    }

}

#endif
