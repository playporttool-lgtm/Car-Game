//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
// Buðra Özdoðanlar
//
//----------------------------------------------


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
#endif

#if PHOTON_UNITY_NETWORKING

/// <summary>
/// Customization applier for vehicles.
/// 6 Upgrade managers for paints, wheels, upgrades, spoilers, customization, and sirens.
/// </summary>
[DefaultExecutionOrder(10)]
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Addons/RCCP Customizer")]
public class RCCP_Customizer : RCCP_Component, IPunObservable {

    public string saveFileName = "";        //  Save file name of the vehicle.

    public bool autoInitialize = true;      //   Auto initializes all managers. Disable it for networked vehicles.
    public bool autoLoadLoadout = true;     //  Loads the latest loadout.
    public bool autoSave = true;     //  Auto save.

    //  Initialize all managers with this method.
    private enum InitializeMethod { Awake, OnEnable, Start, DelayedWithFixedUpdate, DelayedWithFixedUpdateOnEnable }
    [SerializeField] private InitializeMethod initializeMethod = InitializeMethod.DelayedWithFixedUpdateOnEnable;

    [SerializeField] private RCCP_CustomizationLoadout loadout = new RCCP_CustomizationLoadout();       //  Loadout class.

    #region All upgrade managers

    //  Paint manager.
    private RCCP_VehicleUpgrade_PaintManager _paintManager;
    public RCCP_VehicleUpgrade_PaintManager PaintManager {

        get {

            if (_paintManager == null)
                _paintManager = GetComponentInChildren<RCCP_VehicleUpgrade_PaintManager>(true);

            return _paintManager;

        }

    }

    //  Wheel Manager.
    private RCCP_VehicleUpgrade_WheelManager _wheelManager;
    public RCCP_VehicleUpgrade_WheelManager WheelManager {

        get {

            if (_wheelManager == null)
                _wheelManager = GetComponentInChildren<RCCP_VehicleUpgrade_WheelManager>(true);

            return _wheelManager;

        }

    }

    //  Upgrade Manager.
    private RCCP_VehicleUpgrade_UpgradeManager _upgradeManager;
    public RCCP_VehicleUpgrade_UpgradeManager UpgradeManager {

        get {

            if (_upgradeManager == null)
                _upgradeManager = GetComponentInChildren<RCCP_VehicleUpgrade_UpgradeManager>(true);

            return _upgradeManager;

        }

    }

    //  Spoiler Manager.
    private RCCP_VehicleUpgrade_SpoilerManager _spoilerManager;
    public RCCP_VehicleUpgrade_SpoilerManager SpoilerManager {

        get {

            if (_spoilerManager == null)
                _spoilerManager = GetComponentInChildren<RCCP_VehicleUpgrade_SpoilerManager>(true);

            return _spoilerManager;

        }

    }

    //  Siren Manager.
    private RCCP_VehicleUpgrade_SirenManager _sirenManager;
    public RCCP_VehicleUpgrade_SirenManager SirenManager {

        get {

            if (_sirenManager == null)
                _sirenManager = GetComponentInChildren<RCCP_VehicleUpgrade_SirenManager>(true);

            return _sirenManager;

        }

    }

    //  Customization Manager.
    private RCCP_VehicleUpgrade_CustomizationManager _customizationManager;
    public RCCP_VehicleUpgrade_CustomizationManager CustomizationManager {

        get {

            if (_customizationManager == null)
                _customizationManager = GetComponentInChildren<RCCP_VehicleUpgrade_CustomizationManager>(true);

            return _customizationManager;

        }

    }

    //  Decal Manager.
    private RCCP_VehicleUpgrade_DecalManager _decalManager;
    public RCCP_VehicleUpgrade_DecalManager DecalManager {

        get {

            if (_decalManager == null)
                _decalManager = GetComponentInChildren<RCCP_VehicleUpgrade_DecalManager>(true);

            return _decalManager;

        }

    }

    //  Neon Manager.
    private RCCP_VehicleUpgrade_NeonManager _neonManager;
    public RCCP_VehicleUpgrade_NeonManager NeonManager {

        get {

            if (_neonManager == null)
                _neonManager = GetComponentInChildren<RCCP_VehicleUpgrade_NeonManager>(true);

            return _neonManager;

        }

    }

    #endregion

#if PHOTON_UNITY_NETWORKING
    private PhotonView photonView;
#endif

    private float timeout = -1f;

    public override void Awake() {

        base.Awake();

        //  Initializes all managers.
        if (initializeMethod == InitializeMethod.Awake)
            Initialize();

    }

    public override void OnEnable() {

        base.OnEnable();

        //  Initializes all managers.
        if (initializeMethod == InitializeMethod.OnEnable)
            Initialize();

        //  Initializes all managers.
        if (initializeMethod == InitializeMethod.DelayedWithFixedUpdateOnEnable)
            StartCoroutine(Delayed());

        timeout = -1f;

    }

    public override void Start() {

        base.Start();

        //  Initializes all managers.
        if (initializeMethod == InitializeMethod.Start)
            Initialize();

        //  Initializes all managers.
        if (initializeMethod == InitializeMethod.DelayedWithFixedUpdate)
            StartCoroutine(Delayed());

    }

    /// <summary>
    /// Initializing all managers delayed.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Delayed() {

        yield return new WaitForSeconds(.02f);
        Initialize();

    }

    /// <summary>
    /// Initialize all managers.
    /// </summary>
    public void Initialize() {

        photonView = CarController.GetComponent<PhotonView>();

        if (photonView && !photonView.IsMine) {

            HideAll();
            return;

        }

        //  Loads the latest loadout.
        if (autoLoadLoadout)
            Load();

        if (loadout == null)
            loadout = new RCCP_CustomizationLoadout();

        //  Initializes paint manager.
        if (PaintManager)
            PaintManager.Initialize();

        //  Initializes wheel manager.
        if (WheelManager)
            WheelManager.Initialize();

        //  Initializes upgrade manager.
        if (UpgradeManager)
            UpgradeManager.Initialize();

        //  Initializes spoiler manager.
        if (SpoilerManager)
            SpoilerManager.Initialize();

        //  Initializes siren manager.
        if (SirenManager)
            SirenManager.Initialize();

        //  Initializes customization manager.
        if (CustomizationManager)
            CustomizationManager.Initialize();

        //  Initializes decal manager.
        if (DecalManager)
            DecalManager.Initialize();

        //  Initializes neon manager.
        if (NeonManager)
            NeonManager.Initialize();

    }

    /// <summary>
    /// Get loadout.
    /// </summary>
    /// <returns></returns>
    public RCCP_CustomizationLoadout GetLoadout() {

        if (loadout != null) {

            return loadout;

        } else {

            loadout = new RCCP_CustomizationLoadout();
            return loadout;

        }

    }

    private void Update() {

        if (photonView && !photonView.IsMine) {

            if (timeout != -1f && timeout > 0) {

                timeout -= Time.deltaTime;

                if (timeout < 0)
                    timeout = 0f;

            }

            if (timeout <= 0)
                return;

            if (SpoilerManager && loadout.spoiler != -1)
                SpoilerManager.UpgradeWithoutSave(loadout.spoiler);

            if (SirenManager && loadout.siren != -1)
                SirenManager.UpgradeWithoutSave(loadout.siren);

            if (DecalManager) {

                if (loadout.decalIndexFront != -1)
                    DecalManager.UpgradeWithoutSave(0, DecalManager.decals[loadout.decalIndexFront]);

                if (loadout.decalIndexBack != -1)
                    DecalManager.UpgradeWithoutSave(1, DecalManager.decals[loadout.decalIndexBack]);

                if (loadout.decalIndexLeft != -1)
                    DecalManager.UpgradeWithoutSave(2, DecalManager.decals[loadout.decalIndexLeft]);

                if (loadout.decalIndexRight != -1)
                    DecalManager.UpgradeWithoutSave(3, DecalManager.decals[loadout.decalIndexRight]);

            }

            if (PaintManager) {

                if (loadout.paint != new Color(1f, 1f, 1f, 0f))
                    PaintManager.PaintWithoutSave(loadout.paint);

            }

            if (NeonManager && loadout.neonIndex != -1)
                NeonManager.UpgradeWithoutSave(NeonManager.neons[loadout.neonIndex]);

            if (WheelManager && loadout.wheel != -1)
                WheelManager.UpdateWheelWithoutSave(loadout.wheel);

            if (PaintManager && loadout.paint != new Color(1f, 1f, 1f, 0f))
                PaintManager.PaintWithoutSave(loadout.paint);

            if (UpgradeManager) {

                if (UpgradeManager.Engine && loadout.engineLevel != -1)
                    UpgradeManager.UpgradeEngineWithoutSave(loadout.engineLevel);

                if (UpgradeManager.Handling && loadout.handlingLevel != -1)
                    UpgradeManager.UpgradeHandlingWithoutSave(loadout.handlingLevel);

                if (UpgradeManager.Brake && loadout.brakeLevel != -1)
                    UpgradeManager.UpgradeBrakeWithoutSave(loadout.brakeLevel);

                if (UpgradeManager.Speed && loadout.speedLevel != -1)
                    UpgradeManager.UpgradeSpeedWithoutSave(loadout.speedLevel);

            }

            if (CustomizationManager) {

                if (CustomizationManager.customizationData != null) {

                    CustomizationManager.SetHeadlightsColor(CustomizationManager.customizationData.headlightColor);
                    CustomizationManager.SetSmokeColor(CustomizationManager.customizationData.wheelSmokeColor);

                    CustomizationManager.SetFrontCambers(CustomizationManager.customizationData.cambersFront);
                    CustomizationManager.SetRearCambers(CustomizationManager.customizationData.cambersRear);

                    CustomizationManager.SetFrontSuspensionsDistances(CustomizationManager.customizationData.suspensionDistanceFront);
                    CustomizationManager.SetRearSuspensionsDistances(CustomizationManager.customizationData.suspensionDistanceRear);

                }

            }

        }

    }

    /// <summary>
    /// Saves the current loadout with Json.
    /// </summary>
    public void Save() {

        if (photonView && !photonView.IsMine)
            return;

        if (loadout == null)
            loadout = new RCCP_CustomizationLoadout();

        PlayerPrefs.SetString(saveFileName, JsonUtility.ToJson(loadout));

    }

    /// <summary>
    /// Loads the latest saved loadout with Json.
    /// </summary>
    public void Load() {

        if (photonView && !photonView.IsMine)
            return;

        if (PlayerPrefs.HasKey(saveFileName))
            loadout = (RCCP_CustomizationLoadout)JsonUtility.FromJson(PlayerPrefs.GetString(saveFileName), typeof(RCCP_CustomizationLoadout));

    }

    /// <summary>
    /// Deletes the latest saved loadout.
    /// </summary>
    public void Delete() {

        if (photonView && !photonView.IsMine)
            return;

        if (PlayerPrefs.HasKey(saveFileName))
            PlayerPrefs.DeleteKey(PlayerPrefs.GetString(saveFileName));

        loadout = new RCCP_CustomizationLoadout();

        //  Restores paint manager.
        if (PaintManager)
            PaintManager.Restore();

        //  Restores wheel manager.
        if (WheelManager)
            WheelManager.Restore();

        //  Restores upgrade manager.
        if (UpgradeManager)
            UpgradeManager.Restore();

        //  Restores spoiler manager.
        if (SpoilerManager)
            SpoilerManager.Restore();

        //  Restores siren manager.
        if (SirenManager)
            SirenManager.Restore();

        //  Restores customization manager.
        if (CustomizationManager)
            CustomizationManager.Restore();

        //  Restores decal manager.
        if (DecalManager)
            DecalManager.Restore();

        //  Restores neon manager.
        if (NeonManager)
            NeonManager.Restore();

    }

    public void HideAll() {

        //  Restores spoiler manager.
        if (SpoilerManager)
            SpoilerManager.DisableAll();

        //  Restores siren manager.
        if (SirenManager)
            SirenManager.DisableAll();

        //  Restores decal manager.
        if (DecalManager)
            DecalManager.DisableAll();

        //  Restores neon manager.
        if (NeonManager)
            NeonManager.DisableAll();

    }

    public void ShowAll() {

        //  Restores spoiler manager.
        if (SpoilerManager)
            SpoilerManager.EnableAll();

        //  Restores siren manager.
        if (SirenManager)
            SirenManager.EnableAll();

        //  Restores decal manager.
        if (DecalManager)
            DecalManager.EnableAll();

        //  Restores neon manager.
        if (NeonManager)
            NeonManager.EnableAll();

    }

    public void Reload() {

        //

    }

    private void Reset() {

        saveFileName = CarController.transform.name;

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {

        if (stream.IsWriting) {

            if (SpoilerManager)
                stream.SendNext(SpoilerManager.spoilerIndex);
            else
                stream.SendNext(-1);

            if (SirenManager)
                stream.SendNext(SirenManager.sirenIndex);
            else
                stream.SendNext(-1);

            if (DecalManager) {

                stream.SendNext(DecalManager.index_decalFront);
                stream.SendNext(DecalManager.index_decalBack);
                stream.SendNext(DecalManager.index_decalLeft);
                stream.SendNext(DecalManager.index_decalRight);

            } else {

                stream.SendNext(-1);
                stream.SendNext(-1);
                stream.SendNext(-1);
                stream.SendNext(-1);

            }

            if (PaintManager) {

                stream.SendNext(PaintManager.color.r);
                stream.SendNext(PaintManager.color.g);
                stream.SendNext(PaintManager.color.b);
                stream.SendNext(PaintManager.color.a);

            } else {

                stream.SendNext(1);
                stream.SendNext(1);
                stream.SendNext(1);
                stream.SendNext(0);

            }

            if (NeonManager)
                stream.SendNext(NeonManager.index);
            else
                stream.SendNext(-1);

            if (WheelManager)
                stream.SendNext(WheelManager.wheelIndex);
            else
                stream.SendNext(-1);

            if (UpgradeManager) {

                if (UpgradeManager.Engine)
                    stream.SendNext(UpgradeManager.EngineLevel);
                else
                    stream.SendNext(-1);

                if (UpgradeManager.Handling)
                    stream.SendNext(UpgradeManager.HandlingLevel);
                else
                    stream.SendNext(-1);

                if (UpgradeManager.Brake)
                    stream.SendNext(UpgradeManager.BrakeLevel);
                else
                    stream.SendNext(-1);

                if (UpgradeManager.Speed)
                    stream.SendNext(UpgradeManager.SpeedLevel);
                else
                    stream.SendNext(-1);

            } else {

                stream.SendNext(-1);
                stream.SendNext(-1);
                stream.SendNext(-1);

            }

            if (CustomizationManager) {

                if (CustomizationManager.customizationData != null) {

                    stream.SendNext(CustomizationManager.customizationData.headlightColor.r);
                    stream.SendNext(CustomizationManager.customizationData.headlightColor.g);
                    stream.SendNext(CustomizationManager.customizationData.headlightColor.b);

                    stream.SendNext(CustomizationManager.customizationData.wheelSmokeColor.r);
                    stream.SendNext(CustomizationManager.customizationData.wheelSmokeColor.g);
                    stream.SendNext(CustomizationManager.customizationData.wheelSmokeColor.b);

                    stream.SendNext(CustomizationManager.customizationData.cambersFront);
                    stream.SendNext(CustomizationManager.customizationData.cambersRear);

                    stream.SendNext(CustomizationManager.customizationData.suspensionDistanceFront);
                    stream.SendNext(CustomizationManager.customizationData.suspensionDistanceRear);

                }

            } else {

                stream.SendNext(-1);
                stream.SendNext(-1);
                stream.SendNext(-1);

                stream.SendNext(-1);
                stream.SendNext(-1);
                stream.SendNext(-1);

                stream.SendNext(-1);
                stream.SendNext(-1);

                stream.SendNext(-1);
                stream.SendNext(-1);

            }

        } else if (stream.IsReading) {

            loadout.spoiler = (int)stream.ReceiveNext();
            loadout.siren = (int)stream.ReceiveNext();
            loadout.decalIndexFront = (int)stream.ReceiveNext();
            loadout.decalIndexBack = (int)stream.ReceiveNext();
            loadout.decalIndexLeft = (int)stream.ReceiveNext();
            loadout.decalIndexRight = (int)stream.ReceiveNext();
            loadout.paint.r = (float)stream.ReceiveNext();
            loadout.paint.g = (float)stream.ReceiveNext();
            loadout.paint.b = (float)stream.ReceiveNext();
            loadout.paint.a = (float)stream.ReceiveNext();
            loadout.neonIndex = (int)stream.ReceiveNext();
            loadout.wheel = (int)stream.ReceiveNext();
            loadout.engineLevel = (int)stream.ReceiveNext();
            loadout.handlingLevel = (int)stream.ReceiveNext();
            loadout.brakeLevel = (int)stream.ReceiveNext();
            loadout.speedLevel = (int)stream.ReceiveNext();

            loadout.customizationData.headlightColor.r = (float)stream.ReceiveNext();
            loadout.customizationData.headlightColor.g = (float)stream.ReceiveNext();
            loadout.customizationData.headlightColor.b = (float)stream.ReceiveNext();

            loadout.customizationData.wheelSmokeColor.r = (float)stream.ReceiveNext();
            loadout.customizationData.wheelSmokeColor.g = (float)stream.ReceiveNext();
            loadout.customizationData.wheelSmokeColor.b = (float)stream.ReceiveNext();

            loadout.customizationData.cambersFront = (float)stream.ReceiveNext();
            loadout.customizationData.cambersRear = (float)stream.ReceiveNext();

            loadout.customizationData.suspensionDistanceFront = (float)stream.ReceiveNext();
            loadout.customizationData.suspensionDistanceRear = (float)stream.ReceiveNext();

        }

        if (stream.IsReading || stream.IsWriting) {

            if (timeout == -1f)
                timeout = 2f;

        }

    }

}

#else

/// <summary>
/// Customization applier for vehicles.
/// 6 Upgrade managers for paints, wheels, upgrades, spoilers, customization, and sirens.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Addons/RCCP Customizer")]
public class RCCP_Customizer : RCCP_Component {

    public string saveFileName = "";        //  Save file name of the vehicle.

    public bool autoInitialize = true;      //   Auto initializes all managers. Disable it for networked vehicles.
    public bool autoLoadLoadout = true;     //  Loads the latest loadout.
    public bool autoSave = true;     //  Auto save.

    //  Initialize all managers with this method.
    private enum InitializeMethod { Awake, OnEnable, Start, DelayedWithFixedUpdate, DelayedWithFixedUpdateOnEnable }
    [SerializeField] private InitializeMethod initializeMethod = InitializeMethod.DelayedWithFixedUpdateOnEnable;

    [SerializeField] private RCCP_CustomizationLoadout loadout = new RCCP_CustomizationLoadout();       //  Loadout class.

#region All upgrade managers

    //  Paint manager.
    private RCCP_VehicleUpgrade_PaintManager _paintManager;
    public RCCP_VehicleUpgrade_PaintManager PaintManager {

        get {

            if (_paintManager == null)
                _paintManager = GetComponentInChildren<RCCP_VehicleUpgrade_PaintManager>(true);

            return _paintManager;

        }

    }

    //  Wheel Manager.
    private RCCP_VehicleUpgrade_WheelManager _wheelManager;
    public RCCP_VehicleUpgrade_WheelManager WheelManager {

        get {

            if (_wheelManager == null)
                _wheelManager = GetComponentInChildren<RCCP_VehicleUpgrade_WheelManager>(true);

            return _wheelManager;

        }

    }

    //  Upgrade Manager.
    private RCCP_VehicleUpgrade_UpgradeManager _upgradeManager;
    public RCCP_VehicleUpgrade_UpgradeManager UpgradeManager {

        get {

            if (_upgradeManager == null)
                _upgradeManager = GetComponentInChildren<RCCP_VehicleUpgrade_UpgradeManager>(true);

            return _upgradeManager;

        }

    }

    //  Spoiler Manager.
    private RCCP_VehicleUpgrade_SpoilerManager _spoilerManager;
    public RCCP_VehicleUpgrade_SpoilerManager SpoilerManager {

        get {

            if (_spoilerManager == null)
                _spoilerManager = GetComponentInChildren<RCCP_VehicleUpgrade_SpoilerManager>(true);

            return _spoilerManager;

        }

    }

    //  Siren Manager.
    private RCCP_VehicleUpgrade_SirenManager _sirenManager;
    public RCCP_VehicleUpgrade_SirenManager SirenManager {

        get {

            if (_sirenManager == null)
                _sirenManager = GetComponentInChildren<RCCP_VehicleUpgrade_SirenManager>(true);

            return _sirenManager;

        }

    }

    //  Customization Manager.
    private RCCP_VehicleUpgrade_CustomizationManager _customizationManager;
    public RCCP_VehicleUpgrade_CustomizationManager CustomizationManager {

        get {

            if (_customizationManager == null)
                _customizationManager = GetComponentInChildren<RCCP_VehicleUpgrade_CustomizationManager>(true);

            return _customizationManager;

        }

    }

    //  Decal Manager.
    private RCCP_VehicleUpgrade_DecalManager _decalManager;
    public RCCP_VehicleUpgrade_DecalManager DecalManager {

        get {

            if (_decalManager == null)
                _decalManager = GetComponentInChildren<RCCP_VehicleUpgrade_DecalManager>(true);

            return _decalManager;

        }

    }

    //  Neon Manager.
    private RCCP_VehicleUpgrade_NeonManager _neonManager;
    public RCCP_VehicleUpgrade_NeonManager NeonManager {

        get {

            if (_neonManager == null)
                _neonManager = GetComponentInChildren<RCCP_VehicleUpgrade_NeonManager>(true);

            return _neonManager;

        }

    }

#endregion

    public override void Awake() {

        base.Awake();

        //  Initializes all managers.
        if (initializeMethod == InitializeMethod.Awake)
            Initialize();

    }

    public override void OnEnable() {

        base.OnEnable();

        //  Initializes all managers.
        if (initializeMethod == InitializeMethod.OnEnable)
            Initialize();

        //  Initializes all managers.
        if (initializeMethod == InitializeMethod.DelayedWithFixedUpdateOnEnable)
            StartCoroutine(Delayed());

    }

    public override void Start() {

        base.Start();

        //  Initializes all managers.
        if (initializeMethod == InitializeMethod.Start)
            Initialize();

        //  Initializes all managers.
        if (initializeMethod == InitializeMethod.DelayedWithFixedUpdate)
            StartCoroutine(Delayed());

    }

    /// <summary>
    /// Initializing all managers delayed.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Delayed() {

        yield return new WaitForSeconds(.02f);
        Initialize();

    }

    /// <summary>
    /// Initialize all managers.
    /// </summary>
    public void Initialize() {

        //  Loads the latest loadout.
        if (autoLoadLoadout)
            Load();

        if (loadout == null)
            loadout = new RCCP_CustomizationLoadout();

        //  Initializes paint manager.
        if (PaintManager)
            PaintManager.Initialize();

        //  Initializes wheel manager.
        if (WheelManager)
            WheelManager.Initialize();

        //  Initializes upgrade manager.
        if (UpgradeManager)
            UpgradeManager.Initialize();

        //  Initializes spoiler manager.
        if (SpoilerManager)
            SpoilerManager.Initialize();

        //  Initializes siren manager.
        if (SirenManager)
            SirenManager.Initialize();

        //  Initializes customization manager.
        if (CustomizationManager)
            CustomizationManager.Initialize();

        //  Initializes decal manager.
        if (DecalManager)
            DecalManager.Initialize();

        //  Initializes neon manager.
        if (NeonManager)
            NeonManager.Initialize();

    }

    /// <summary>
    /// Get loadout.
    /// </summary>
    /// <returns></returns>
    public RCCP_CustomizationLoadout GetLoadout() {

        if (loadout != null) {

            return loadout;

        } else {

            loadout = new RCCP_CustomizationLoadout();
            return loadout;

        }

    }

    /// <summary>
    /// Saves the current loadout with Json.
    /// </summary>
    public void Save() {

        if (loadout == null)
            loadout = new RCCP_CustomizationLoadout();

        PlayerPrefs.SetString(saveFileName, JsonUtility.ToJson(loadout));

    }

    /// <summary>
    /// Loads the latest saved loadout with Json.
    /// </summary>
    public void Load() {

        if (PlayerPrefs.HasKey(saveFileName))
            loadout = (RCCP_CustomizationLoadout)JsonUtility.FromJson(PlayerPrefs.GetString(saveFileName), typeof(RCCP_CustomizationLoadout));

    }

    /// <summary>
    /// Deletes the latest saved loadout.
    /// </summary>
    public void Delete() {

        if (PlayerPrefs.HasKey(saveFileName))
            PlayerPrefs.DeleteKey(PlayerPrefs.GetString(saveFileName));

        loadout = new RCCP_CustomizationLoadout();

        //  Restores paint manager.
        if (PaintManager)
            PaintManager.Restore();

        //  Restores wheel manager.
        if (WheelManager)
            WheelManager.Restore();

        //  Restores upgrade manager.
        if (UpgradeManager)
            UpgradeManager.Restore();

        //  Restores spoiler manager.
        if (SpoilerManager)
            SpoilerManager.Restore();

        //  Restores siren manager.
        if (SirenManager)
            SirenManager.Restore();

        //  Restores customization manager.
        if (CustomizationManager)
            CustomizationManager.Restore();

        //  Restores decal manager.
        if (DecalManager)
            DecalManager.Restore();

        //  Restores neon manager.
        if (NeonManager)
            NeonManager.Restore();

    }

    public void HideAll() {

        //  Restores spoiler manager.
        if (SpoilerManager)
            SpoilerManager.DisableAll();

        //  Restores siren manager.
        if (SirenManager)
            SirenManager.DisableAll();

        //  Restores decal manager.
        if (DecalManager)
            DecalManager.DisableAll();

        //  Restores neon manager.
        if (NeonManager)
            NeonManager.DisableAll();

    }

    public void ShowAll() {

        //  Restores spoiler manager.
        if (SpoilerManager)
            SpoilerManager.EnableAll();

        //  Restores siren manager.
        if (SirenManager)
            SirenManager.EnableAll();

        //  Restores decal manager.
        if (DecalManager)
            DecalManager.EnableAll();

        //  Restores neon manager.
        if (NeonManager)
            NeonManager.EnableAll();

    }

    public void Reload() {

        //

    }

    private void Reset() {

        saveFileName = CarController.transform.name;

    }

}

#endif