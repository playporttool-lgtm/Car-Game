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

public class RCCP_Prototype_Photon : ScriptableObject {

    /// <summary>
    /// All spawnable photon prototype vehicles.
    /// </summary>
    public RCCP_CarController[] vehicles;

    #region singleton
    private static RCCP_Prototype_Photon instance;
    public static RCCP_Prototype_Photon Instance { get { if (instance == null) instance = Resources.Load("RCCP_Prototype_Photon") as RCCP_Prototype_Photon; return instance; } }
    #endregion

}
