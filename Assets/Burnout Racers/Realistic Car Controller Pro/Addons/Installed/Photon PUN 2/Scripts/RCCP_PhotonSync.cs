//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2025 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

#if RCCP_PHOTON && PHOTON_UNITY_NETWORKING
using UnityEngine;
using Photon.Pun;
using TMPro;

public class RCCP_PhotonSync : MonoBehaviourPunCallbacks, IPunObservable {

    [Header("Photon Sync Settings")]
    [Tooltip("How many times per second to send network updates.\nDefault = PhotonNetwork.SerializationRate.")]
    public int sendRate = 40;

    [Tooltip("Distance threshold beyond which remote vehicles teleport instead of interpolating.")]
    public float teleportDistanceThreshold = 4f;

    [Header("Advanced Interpolation Settings")]
    [Tooltip("Lag compensation time in seconds. Higher values = smoother but more latency.")]
    [Range(0.05f, 0.5f)]
    public float lagCompensationTime = 0.05f;

    [Tooltip("Position interpolation speed multiplier.")]
    [Range(5f, 30f)]
    public float positionInterpolationSpeed = 10f;

    [Tooltip("Rotation interpolation speed multiplier.")]
    [Range(5f, 30f)]
    public float rotationInterpolationSpeed = 10f;

    [Tooltip("Enable extrapolation for smoother movement prediction.")]
    public bool useExtrapolation = true;

    [Tooltip("Maximum extrapolation time in seconds.")]
    [Range(0.02f, 0.2f)]
    public float maxExtrapolationTime = .1f;

    [Header("Velocity Synchronization")]
    [Tooltip("Enable velocity-based smoothing for more accurate physics.")]
    public bool useVelocitySmoothing = true;

    [Tooltip("Velocity interpolation damping factor.")]
    [Range(0.1f, 2f)]
    public float velocityDampening = .8f;

    /// <summary>
    /// Reference to the parent Realistic Car Controller component.
    /// </summary>
    private RCCP_CarController carController;

    /// <summary>
    /// Cached inputs module of the car controller.
    /// </summary>
    private RCCP_Input inputsModule;

    /// <summary>
    /// Cached engine module of the car controller.
    /// </summary>
    private RCCP_Engine engineModule;

    /// <summary>
    /// Cached clutch module of the car controller.
    /// </summary>
    private RCCP_Clutch clutchModule;

    /// <summary>
    /// Cached gearbox module of the car controller.
    /// </summary>
    private RCCP_Gearbox gearboxModule;

    /// <summary>
    /// Cached differential module of the car controller.
    /// </summary>
    private RCCP_Differential differentialModule;

    // Enhanced networking state variables

    /// <summary>
    /// Timestamp of last network update, used for interpolation.
    /// </summary>
    private float lastUpdateTime = 0f;

    /// <summary>
    /// Network lag between updates.
    /// </summary>
    private float networkLag = 0f;

    /// <summary>
    /// Buffer of recent network states for interpolation.
    /// </summary>
    private NetworkState[] stateBuffer;

    /// <summary>
    /// Current buffer index.
    /// </summary>
    private int bufferIndex = 0;

    /// <summary>
    /// Number of buffered states.
    /// </summary>
    private const int BUFFER_SIZE = 10;

    /// <summary>
    /// Current interpolated velocity for smoother movement.
    /// </summary>
    private Vector3 interpolatedVelocity = Vector3.zero;

    /// <summary>
    /// Current interpolated angular velocity for smoother rotation.
    /// </summary>
    private Vector3 interpolatedAngularVelocity = Vector3.zero;

    /// <summary>
    /// Target position received from network with lag compensation.
    /// </summary>
    private Vector3 targetPosition = Vector3.zero;

    /// <summary>
    /// Target rotation received from network with lag compensation.
    /// </summary>
    private Quaternion targetRotation = Quaternion.identity;

    /// <summary>
    /// Last received network velocity.
    /// </summary>
    private Vector3 networkVelocity = Vector3.zero;

    /// <summary>
    /// Last received network angular velocity.
    /// </summary>
    private Vector3 networkAngularVelocity = Vector3.zero;

    // Inputs and state overrides

    private float gasInput = 0f;
    private float brakeInput = 0f;
    private float steerInput = 0f;
    private float handbrakeInput = 0f;
    private float boostInput = 0f;
    private float clutchInput = 0f;
    private float engineRPM = 0f;
    private int gear = 0;
    private bool changingGear = false;
    private bool engineStarting = false;
    private bool engineRunning = false;
    private int direction = 0;

    // Drivetrain outputs

    private float differentialOutputLeft = 0f;
    private float differentialOutputRight = 0f;

    // Lights state

    private bool lowBeamHeadLightsOn = false;
    private bool highBeamHeadLightsOn = false;
    private bool indicatorsLeft = false;
    private bool indicatorsRight = false;
    private bool indicatorsAll = false;

    /// <summary>
    /// Structure to hold networked state information.
    /// </summary>
    private struct NetworkState {

        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;
        public Vector3 angularVelocity;
        public float timestamp;

    }

    /// <summary>
    /// Returns true if this PhotonView is owned by the local client.
    /// </summary>
    public bool IsMine => photonView.IsMine;

    private void Awake() {

        carController = GetComponentInParent<RCCP_CarController>(true);

        inputsModule = carController.Inputs;
        engineModule = carController.Engine;
        clutchModule = carController.Clutch;
        gearboxModule = carController.Gearbox;
        differentialModule = carController.Differential;

        // Initialize state buffer
        stateBuffer = new NetworkState[BUFFER_SIZE];

        for (int i = 0; i < BUFFER_SIZE; i++) {

            stateBuffer[i] = new NetworkState {
                position = transform.position,
                rotation = transform.rotation,
                velocity = Vector3.zero,
                angularVelocity = Vector3.zero,
                timestamp = Time.time
            };

        }

    }

    public void Start() {

        if (!photonView.ObservedComponents.Contains(this))
            photonView.ObservedComponents.Add(this);

        photonView.Synchronization = ViewSynchronization.Unreliable;
        PhotonNetwork.SerializationRate = sendRate;

    }

    public override void OnEnable() {

        base.OnEnable();
        GetInitialValues();

    }

    /// <summary>
    /// Captures the initial network state and transform for a newly enabled object.
    /// </summary>
    private void GetInitialValues() {

        targetPosition = transform.position;
        targetRotation = transform.rotation;

        gasInput = carController.throttleInput_V;
        brakeInput = carController.brakeInput_V;
        steerInput = carController.steerInput_V;
        handbrakeInput = carController.handbrakeInput_V;
        boostInput = carController.nosInput_V;
        clutchInput = carController.clutchInput_V;

        engineRPM = carController.engineRPM;
        direction = carController.direction;
        engineStarting = carController.engineStarting;
        engineRunning = carController.engineRunning;

        gear = carController.currentGear;
        gear = MapGearState(carController.Gearbox.currentGearState.gearState);

        differentialOutputLeft = carController.Differential.outputLeft;
        differentialOutputRight = carController.Differential.outputRight;

        lowBeamHeadLightsOn = carController.lowBeamLights;
        highBeamHeadLightsOn = carController.highBeamLights;
        indicatorsLeft = carController.indicatorsLeftLights;
        indicatorsRight = carController.indicatorsRightLights;
        indicatorsAll = carController.indicatorsAllLights;

    }

    /// <summary>
    /// Called every physics step to apply local or remote control.
    /// </summary>
    private void FixedUpdate() {

        if (!PhotonNetwork.IsConnectedAndReady)
            return;

        if (!carController)
            return;

        if (carController.OtherAddonsManager.AI != null)
            carController.externalControl = true;
        else
            carController.externalControl = !IsMine;

        if (!IsMine) {

            ApplyRemoteMovement();
            ApplyRemoteInputsAndState();

        }

    }

    /// <summary>
    /// Applies enhanced movement interpolation for remote vehicles.
    /// </summary>
    private void ApplyRemoteMovement() {

        // Calculate time difference since last update
        float timeSinceUpdate = Time.time - lastUpdateTime;

        // Check for teleportation threshold
        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance > teleportDistanceThreshold) {

            // Teleport if too far
            transform.position = targetPosition;
            transform.rotation = targetRotation;
            carController.Rigid.linearVelocity = networkVelocity;
            carController.Rigid.angularVelocity = networkAngularVelocity;

        } else {

            // Enhanced interpolation with lag compensation
            Vector3 predictedPosition = targetPosition;
            Quaternion predictedRotation = targetRotation;

            // Apply extrapolation if enabled and we have recent data
            if (useExtrapolation && timeSinceUpdate < maxExtrapolationTime) {

                float extrapolationTime = Mathf.Min(timeSinceUpdate, maxExtrapolationTime);
                predictedPosition += networkVelocity * extrapolationTime;

                // Apply angular velocity extrapolation
                if (networkAngularVelocity.magnitude > 0.1f) {

                    Vector3 angularDisplacement = networkAngularVelocity * extrapolationTime * Mathf.Rad2Deg;
                    predictedRotation *= Quaternion.Euler(angularDisplacement);

                }

            }

            // Smooth position interpolation
            if (useVelocitySmoothing) {

                // Use velocity-based smoothing for more accurate physics
                Vector3 velocityDifference = networkVelocity - carController.Rigid.linearVelocity;
                interpolatedVelocity = Vector3.Lerp(interpolatedVelocity, velocityDifference, Time.fixedDeltaTime * velocityDampening);

                transform.position = Vector3.SmoothDamp(
                    transform.position,
                    predictedPosition,
                    ref interpolatedVelocity,
                    lagCompensationTime,
                    positionInterpolationSpeed
                );

            } else {

                // Standard position interpolation
                transform.position = Vector3.Lerp(
                    transform.position,
                    predictedPosition,
                    Time.fixedDeltaTime * positionInterpolationSpeed
                );

            }

            // Smooth rotation interpolation
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                predictedRotation,
                Time.fixedDeltaTime * rotationInterpolationSpeed
            );

            // Apply velocity smoothing to rigidbody
            if (useVelocitySmoothing) {

                carController.Rigid.linearVelocity = Vector3.Lerp(
                    carController.Rigid.linearVelocity,
                    networkVelocity,
                    Time.fixedDeltaTime * velocityDampening
                );

                carController.Rigid.angularVelocity = Vector3.Lerp(
                    carController.Rigid.angularVelocity,
                    networkAngularVelocity,
                    Time.fixedDeltaTime * velocityDampening
                );

            }

        }

    }

    /// <summary>
    /// Applies remote inputs and vehicle state.
    /// </summary>
    private void ApplyRemoteInputsAndState() {

        // Override inputs and modules
        inputsModule.OverrideInputs(new RCCP_Inputs {
            throttleInput = gasInput,
            brakeInput = brakeInput,
            steerInput = steerInput,
            handbrakeInput = handbrakeInput,
            nosInput = boostInput,
            clutchInput = clutchInput
        });

        engineModule.OverrideRPM(engineRPM);
        engineModule.engineStarting = engineStarting;
        engineModule.engineRunning = engineRunning;

        clutchModule.OverrideInput(clutchInput);

        var targetGear = MapIndexToGearState(gear);
        gearboxModule.OverrideGear(carController.currentGear, targetGear);
        gearboxModule.shiftingNow = changingGear;

        differentialModule.OverrideDifferential(differentialOutputLeft, differentialOutputRight);

        // Apply lights
        carController.Lights.lowBeamHeadlights = lowBeamHeadLightsOn;
        carController.Lights.highBeamHeadlights = highBeamHeadLightsOn;
        carController.Lights.indicatorsLeft = indicatorsLeft;
        carController.Lights.indicatorsRight = indicatorsRight;
        carController.Lights.indicatorsAll = indicatorsAll;

    }

    /// <summary>
    /// Serialize and deserialize network state with enhanced buffering.
    /// </summary>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {

        if (!carController)
            return;

        if (stream.IsWriting) {

            // Send local state to others
            stream.SendNext(carController.throttleInput_V);
            stream.SendNext(carController.brakeInput_V);
            stream.SendNext(carController.steerInput_V);
            stream.SendNext(carController.handbrakeInput_V);
            stream.SendNext(carController.nosInput_V);
            stream.SendNext(carController.clutchInput_V);

            stream.SendNext(carController.engineRPM);
            stream.SendNext(carController.currentGear);
            stream.SendNext(carController.shiftingNow);
            stream.SendNext(carController.engineStarting);
            stream.SendNext(carController.engineRunning);
            stream.SendNext(carController.direction);

            // Gear state mapping
            stream.SendNext(MapGearState(carController.Gearbox.currentGearState.gearState));

            // Differential outputs
            stream.SendNext(carController.Differential.outputLeft);
            stream.SendNext(carController.Differential.outputRight);

            // Lights
            stream.SendNext(carController.lowBeamLights);
            stream.SendNext(carController.highBeamLights);
            stream.SendNext(carController.indicatorsLeftLights);
            stream.SendNext(carController.indicatorsRightLights);
            stream.SendNext(carController.indicatorsAllLights);

            // Transform state with timestamp
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(carController.Rigid.linearVelocity);
            stream.SendNext(carController.Rigid.angularVelocity);
            stream.SendNext(Time.time);

        } else {

            // Receive remote state
            gasInput = (float)stream.ReceiveNext();
            brakeInput = (float)stream.ReceiveNext();
            steerInput = (float)stream.ReceiveNext();
            handbrakeInput = (float)stream.ReceiveNext();
            boostInput = (float)stream.ReceiveNext();
            clutchInput = (float)stream.ReceiveNext();

            engineRPM = (float)stream.ReceiveNext();
            gear = (int)stream.ReceiveNext();
            changingGear = (bool)stream.ReceiveNext();
            engineStarting = (bool)stream.ReceiveNext();
            engineRunning = (bool)stream.ReceiveNext();
            direction = (int)stream.ReceiveNext();

            gear = (int)stream.ReceiveNext();
            differentialOutputLeft = (float)stream.ReceiveNext();
            differentialOutputRight = (float)stream.ReceiveNext();

            lowBeamHeadLightsOn = (bool)stream.ReceiveNext();
            highBeamHeadLightsOn = (bool)stream.ReceiveNext();
            indicatorsLeft = (bool)stream.ReceiveNext();
            indicatorsRight = (bool)stream.ReceiveNext();
            indicatorsAll = (bool)stream.ReceiveNext();

            // Enhanced state buffering
            Vector3 receivedPosition = (Vector3)stream.ReceiveNext();
            Quaternion receivedRotation = (Quaternion)stream.ReceiveNext();
            Vector3 receivedVelocity = (Vector3)stream.ReceiveNext();
            Vector3 receivedAngularVelocity = (Vector3)stream.ReceiveNext();
            float receivedTimestamp = (float)stream.ReceiveNext();

            // Buffer the state
            BufferNetworkState(receivedPosition, receivedRotation, receivedVelocity, receivedAngularVelocity, receivedTimestamp);

            // Calculate network lag
            networkLag = (float)(PhotonNetwork.Time - info.SentServerTime);

            // Apply lag compensation
            ApplyLagCompensation(receivedPosition, receivedRotation, receivedVelocity, receivedAngularVelocity);

            lastUpdateTime = Time.time;

        }

    }

    /// <summary>
    /// Buffers network state for improved interpolation.
    /// </summary>
    private void BufferNetworkState(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity, float timestamp) {

        stateBuffer[bufferIndex] = new NetworkState {
            position = position,
            rotation = rotation,
            velocity = velocity,
            angularVelocity = angularVelocity,
            timestamp = timestamp
        };

        bufferIndex = (bufferIndex + 1) % BUFFER_SIZE;

    }

    /// <summary>
    /// Applies lag compensation to received network data.
    /// </summary>
    private void ApplyLagCompensation(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity) {

        // Store the raw network data
        networkVelocity = velocity;
        networkAngularVelocity = angularVelocity;

        // Apply lag compensation by predicting future position
        float compensationTime = networkLag + lagCompensationTime;
        targetPosition = position + velocity * compensationTime;
        targetRotation = rotation;

        // Apply angular compensation if significant rotation
        if (angularVelocity.magnitude > 0.1f) {

            Vector3 angularDisplacement = angularVelocity * compensationTime * Mathf.Rad2Deg;
            targetRotation *= Quaternion.Euler(angularDisplacement);

        }

    }

    /// <summary>
    /// Converts a GearState enum to an integer index for serialization.
    /// </summary>
    private int MapGearState(RCCP_Gearbox.CurrentGearState.GearState state) {

        switch (state) {
            case RCCP_Gearbox.CurrentGearState.GearState.InForwardGear:
                return 1;
            case RCCP_Gearbox.CurrentGearState.GearState.InReverseGear:
                return 2;
            case RCCP_Gearbox.CurrentGearState.GearState.Neutral:
                return 3;
            case RCCP_Gearbox.CurrentGearState.GearState.Park:
                return 4;
            default:
                return 0;
        }

    }

    /// <summary>
    /// Converts a serialized gear index back to the GearState enum.
    /// </summary>
    private RCCP_Gearbox.CurrentGearState.GearState MapIndexToGearState(int index) {

        switch (index) {
            case 1:
                return RCCP_Gearbox.CurrentGearState.GearState.InForwardGear;
            case 2:
                return RCCP_Gearbox.CurrentGearState.GearState.InReverseGear;
            case 3:
                return RCCP_Gearbox.CurrentGearState.GearState.Neutral;
            case 4:
                return RCCP_Gearbox.CurrentGearState.GearState.Park;
            default:
                return RCCP_Gearbox.CurrentGearState.GearState.Neutral;
        }

    }

    /// <summary>
    /// Ensures a PhotonView is attached and configured when this component is reset.
    /// </summary>
    private void Reset() {

        PhotonView pv = GetComponent<PhotonView>();

        if (!pv)
            pv = gameObject.AddComponent<PhotonView>();

        pv.OwnershipTransfer = OwnershipOption.Fixed;
        pv.Synchronization = ViewSynchronization.Unreliable;
        pv.observableSearch = PhotonView.ObservableSearch.AutoFindAll;

    }

    /// <summary>
    /// Visualizes network synchronization data in the scene view.
    /// </summary>
    private void OnDrawGizmos() {

        if (!IsMine && Application.isPlaying) {

            // Draw target position in red
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(targetPosition, 0.5f);

            // Draw velocity vector in blue
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, networkVelocity.normalized * 3f);

            // Draw interpolation line in yellow
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, targetPosition);

        }

    }

}
#endif