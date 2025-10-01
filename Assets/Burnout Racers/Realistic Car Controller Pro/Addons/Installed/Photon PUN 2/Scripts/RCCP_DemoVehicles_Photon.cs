//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2025 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RCCP_DemoVehicles_Photon : ScriptableObject {

    /// <summary>
    /// All spawnable photon demo vehicles.
    /// </summary>
    public RCCP_CarController[] vehicles;

    #region singleton
    private static RCCP_DemoVehicles_Photon instance;
    public static RCCP_DemoVehicles_Photon Instance { get { if (instance == null) instance = Resources.Load("RCCP_DemoVehicles_Photon") as RCCP_DemoVehicles_Photon; return instance; } }
    #endregion

}
