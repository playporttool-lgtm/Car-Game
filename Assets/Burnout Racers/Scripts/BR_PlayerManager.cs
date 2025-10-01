//----------------------------------------------
//			Burnout Racers
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;

#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
#endif

#if PHOTON_UNITY_NETWORKING

/// <summary>
/// Main racer observer. Must be attached to all vehicles. Compatible with player, bots, and networked players. 
/// </summary>
[RequireComponent(typeof(RCCP_CarController))]
[RequireComponent(typeof(BR_RacePositioner))]
public class BR_PlayerManager : MonoBehaviourPunCallbacks, IPunObservable {

    private RCCP_CarController _carController;

    /// <summary>
    /// Main car controller.
    /// </summary>
    public RCCP_CarController CarController {

        get {

            if (_carController == null)
                _carController = GetComponent<RCCP_CarController>();

            return _carController;

        }

    }

    private BR_RacePositioner _racePositioner;

    /// <summary>
    /// Race positioner that contains race position and total laps.
    /// </summary>
    public BR_RacePositioner RacePositioner {

        get {

            if (_racePositioner == null)
                _racePositioner = GetComponent<BR_RacePositioner>();

            return _racePositioner;

        }

    }

    /// <summary>
    /// Nickname of the vehicle. This will be assigned by the BR_GameplayManager. If it's a player vehicle, local nickname will be assigned.
    /// If it's a bot vehicle, random bot nickname will be assigned.
    /// If it's a network player, network player's name will be assigned.
    /// </summary>
    public string nickName = "";

    /// <summary>
    /// Indicates if the race has started, taken from the BR_GameplayManager.
    /// </summary>
    public bool raceStarted = false;

    /// <summary>
    /// Indicates if the vehicle can finish the race after completing the last lap.
    /// </summary>
    public bool canFinishRace = false;

    /// <summary>
    /// Vehicle's race position taken from the race positioner component.
    /// </summary>
    public int racePosition = -1;

    /// <summary>
    /// Vehicle's total laps taken from the race positioner component.
    /// </summary>
    public int totalLaps = 0;

    /// <summary>
    /// Vehicle's total race time.
    /// </summary>
    public float raceTime = 0f;

    private void Awake() {

        raceStarted = false;

    }

    public override void OnEnable() {

        base.OnEnable();

        //  Checking the ground gap when the vehicle spawned.
        CheckGroundGap();

    }

    private void Update() {

        //  If current scene is not a gameplay scene, return with initial values.
        if (!BR_GameplayManager.Instance) {

            raceTime = 0f;
            racePosition = -1;
            totalLaps = 0;

            return;

        }

        //  If it's online mode, and we're connected...
        if (!BR_GameplayManager.Instance.offlineMode) {

            //  Return if not connected yet.
            if (!PhotonNetwork.IsConnectedAndReady)
                return;

            //  Return if photonView of the vehicle is not ours.
            if (PhotonNetwork.InRoom && photonView && !photonView.IsMine)
                return;

        }

        //  Race started yet?
        raceStarted = CarController.canControl;

        if (raceStarted) {

            //  If race started, run the timer.
            raceTime += Time.deltaTime;

            //  Getting race position from the race positioner component.
            racePosition = RacePositioner.racePosition;

            //  Getting total laps from the race positioner component.
            totalLaps = RacePositioner.lap;

            //  If total laps achieved, finish the race for this vehicle.
            if (BR_GameplayManager.Instance.targetLap > 0 && totalLaps >= BR_GameplayManager.Instance.targetLap)
                canFinishRace = true;
            else
                canFinishRace = false;

        }

    }

    /// <summary>
    /// Eliminates ground gap distance when spawned. Checking the ground gap when the vehicle is spawned.
    /// </summary>
    private void CheckGroundGap() {

        WheelCollider wheel = GetComponentInChildren<WheelCollider>();
        float distancePivotBetweenWheel = Vector3.Distance(new Vector3(0f, transform.position.y, 0f), new Vector3(0f, wheel.transform.position.y, 0f));

        RaycastHit hit;

        if (Physics.Raycast(wheel.transform.position, -Vector3.up, out hit, 10f))
            transform.position = new Vector3(transform.position.x, hit.point.y + distancePivotBetweenWheel + (wheel.radius) + (wheel.suspensionDistance / 2f), transform.position.z);

    }

    /// <summary>
    /// Syncing values over the network.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="info"></param>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {

        if (stream.IsWriting) {

            stream.SendNext(nickName);
            stream.SendNext(raceTime);
            stream.SendNext(racePosition);
            stream.SendNext(totalLaps);
            stream.SendNext(raceStarted);

        } else if (stream.IsReading) {

            nickName = (string)stream.ReceiveNext();
            raceTime = (float)stream.ReceiveNext();
            racePosition = (int)stream.ReceiveNext();
            totalLaps = (int)stream.ReceiveNext();
            raceStarted = (bool)stream.ReceiveNext();

        }

    }

    public void OnTriggerEnter(Collider other) {

        if (!canFinishRace || !raceStarted)
            return;

        BR_RaceFinisher raceFinisher = other.GetComponentInParent<BR_RaceFinisher>();

        if (!raceFinisher)
            return;

        FinishRace();

    }

    /// <summary>
    /// Finishes the race for this vehicle.
    /// </summary>
    public void FinishRace() {

        BR_GameplayManager.Instance.RacerFinishedRace(this);
        CarController.canControl = false;
        raceStarted = false;

    }

    ///// <summary>
    ///// When collided.
    ///// </summary>
    ///// <param name="collision"></param>
    //private void OnCollisionEnter(Collision collision) {

    //    //  Damping the linear velocity.
    //    Vector3 velocity = transform.InverseTransformDirection(CarController.Rigid.linearVelocity);
    //    velocity.x *= .8f;
    //    velocity.y *= .3f;
    //    CarController.Rigid.linearVelocity = transform.TransformDirection(velocity);

    //    //  Damping the angular velocity.
    //    Vector3 angularVelocity = CarController.Rigid.angularVelocity;
    //    angularVelocity *= .4f;
    //    CarController.Rigid.angularVelocity = angularVelocity;

    //}

    private void Reset() {

        if (!gameObject.GetComponent<RCCP_PhotonSync>())
            gameObject.AddComponent<RCCP_PhotonSync>();

        if (!gameObject.GetComponent<PhotonView>())
            gameObject.AddComponent<PhotonView>();

    }

    private void OnValidate() {

        RCCP_Lod lodComponent = GetComponentInChildren<RCCP_Lod>(false);

        if (lodComponent)
            lodComponent.gameObject.SetActive(false);

        RCCP_AI aiComponent = GetComponentInChildren<RCCP_AI>(false);

        if (aiComponent != null) {

            RCCP_Customizer customizerComponent = GetComponentInChildren<RCCP_Customizer>(false);

            if (customizerComponent)
                customizerComponent.gameObject.SetActive(false);

        }

    }

}

#else

/// <summary>
/// Main player observer. Must be attached to all vehicles. Compatible with player, bots, and networked players. 
/// </summary>
[RequireComponent(typeof(RCCP_CarController))]
[RequireComponent(typeof(BR_RacePositioner))]
public class BR_PlayerManager : MonoBehaviour {

    private RCCP_CarController _carController;

    /// <summary>
    /// Main car controller.
    /// </summary>
    public RCCP_CarController CarController {

        get {

            if (_carController == null)
                _carController = GetComponent<RCCP_CarController>();

            return _carController;

        }

    }

    private BR_RacePositioner _racePositioner;

    /// <summary>
    /// Race positioner that contains race position and total laps.
    /// </summary>
    public BR_RacePositioner RacePositioner {

        get {

            if (_racePositioner == null)
                _racePositioner = GetComponent<BR_RacePositioner>();

            return _racePositioner;

        }

    }

    /// <summary>
    /// Nickname of the vehicle. This will be assigned by the BR_GameplayManager. If it's a player vehicle, local nickname will be assigned.
    /// If it's a bot vehicle, random bot nickname will be assigned.
    /// If it's a network player, network player's name will be assigned.
    /// </summary>
    public string nickName = "";

    /// <summary>
    /// Race started bool taken from the BR_GameplayManager.
    /// </summary>
    public bool raceStarted = false;

    /// <summary>
    /// Can finish the race after completing the last lap?
    /// </summary>
    public bool canFinishRace = false;

    /// <summary>
    /// Vehicle's race position taken from the race positioner component.
    /// </summary>
    public int racePosition = -1;

    /// <summary>
    /// Vehicle's total laps taken from the race positioner component.
    /// </summary>
    public int totalLaps = 0;

    /// <summary>
    /// Vehicle's total race time.
    /// </summary>
    public float raceTime = 0f;

    private void Awake() {

        raceStarted = false;

    }

    public void OnEnable() {

        //  Checking the ground gap when the vehicle spawned.
        CheckGroundGap();

    }

    private void Update() {

        //  If current scene is not a gameplay scene, return with initial values.
        if (!BR_GameplayManager.Instance) {

            raceTime = 0f;
            racePosition = -1;
            totalLaps = 0;

            return;

        }

        //  Race started yet?
        raceStarted = CarController.canControl;

        if (raceStarted) {

            //  If race started, run the timer.
            raceTime += Time.deltaTime;

            //  Getting race position from the race positioner component.
            racePosition = RacePositioner.racePosition;

            //  Getting total laps from the race positioner component.
            totalLaps = RacePositioner.lap;

            //  If total laps achieved, finish the race for this vehicle.
            if (BR_GameplayManager.Instance.targetLap > 0 && totalLaps >= BR_GameplayManager.Instance.targetLap)
                canFinishRace = true;
            else
                canFinishRace = false;

        }

    }

    /// <summary>
    /// Eliminates ground gap distance when spawned. Checking the ground gap when the vehicle is spawned.
    /// </summary>
    private void CheckGroundGap() {

        WheelCollider wheel = GetComponentInChildren<WheelCollider>();
        float distancePivotBetweenWheel = Vector3.Distance(new Vector3(0f, transform.position.y, 0f), new Vector3(0f, wheel.transform.position.y, 0f));

        RaycastHit hit;

        if (Physics.Raycast(wheel.transform.position, -Vector3.up, out hit, 10f))
            transform.position = new Vector3(transform.position.x, hit.point.y + distancePivotBetweenWheel + (wheel.radius) + (wheel.suspensionDistance / 2f), transform.position.z);

    }

    public void OnTriggerEnter(Collider other) {

        if (!canFinishRace || !raceStarted)
            return;

        BR_RaceFinisher raceFinisher = other.GetComponentInParent<BR_RaceFinisher>();

        if (!raceFinisher)
            return;

        FinishRace();

    }

    /// <summary>
    /// Finishes the race for this vehicle.
    /// </summary>
    public void FinishRace() {

        BR_GameplayManager.Instance.RacerFinishedRace(this);
        CarController.canControl = false;
        raceStarted = false;

    }

}

#endif
