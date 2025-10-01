//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2025 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// All demo scenes.
/// </summary>
public class RCCP_DemoScenes_Photon : ScriptableObject {

    public int instanceId = 0;

    #region singleton
    private static RCCP_DemoScenes_Photon instance;
    public static RCCP_DemoScenes_Photon Instance { get { if (instance == null) instance = Resources.Load("RCCP_DemoScenes_Photon") as RCCP_DemoScenes_Photon; return instance; } }
    #endregion

    public Object demo_PUN2City;
    public Object demo_PUN2Lobby;

    public string path_demo_PUN2City;
    public string path_demo_PUN2Lobby;

    public void Clean() {

        demo_PUN2City = null;
        demo_PUN2Lobby = null;

        path_demo_PUN2City = "";
        path_demo_PUN2Lobby = "";

    }

    public void GetPaths() {

        if (demo_PUN2City != null)
            path_demo_PUN2City = RCCP_GetAssetPath.GetAssetPath(demo_PUN2City);

        if (demo_PUN2Lobby != null)
            path_demo_PUN2Lobby = RCCP_GetAssetPath.GetAssetPath(demo_PUN2Lobby);

    }

}
