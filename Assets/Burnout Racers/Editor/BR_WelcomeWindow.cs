//----------------------------------------------
//			Burnout Racers
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
//
//----------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class BR_WelcomeWindow : EditorWindow {

    public class ToolBar {

        public string title;
        public UnityEngine.Events.UnityAction Draw;

        /// <summary>
        /// Create New Toolbar
        /// </summary>
        public ToolBar(string title, UnityEngine.Events.UnityAction onDraw) {

            this.title = title;
            this.Draw = onDraw;

        }

        public static implicit operator string(ToolBar tool) {

            return tool.title;

        }

    }

    /// <summary>
    /// Index of selected toolbar.
    /// </summary>
    public int toolBarIndex = 0;

    /// <summary>
    /// List of Toolbars
    /// </summary>
    public ToolBar[] toolBars = new ToolBar[]{

        new ToolBar("Welcome", WelcomePageContent),
        new ToolBar("Setup", InstallationPageContent),
        new ToolBar("Scenes", DemosPageContent),
        new ToolBar("Updates", UpdatePageContent),
        new ToolBar("DOCS", Documentations)

    };

    public static Texture2D bannerTexture = null;

    private GUISkin skin;

    private const int windowWidth = 600;
    private const int windowHeight = 750;

    [MenuItem("Tools/BoneCracker Games/Burnout Racers/Welcome Window", false, 0)]
    public static void OpenWindow() {

        GetWindow<BR_WelcomeWindow>(true);

    }

    private void OnEnable() {

        titleContent = new GUIContent("Burnout Racers Manager");
        maxSize = new Vector2(windowWidth, windowHeight);
        minSize = maxSize;

        InitStyle();

    }

    private void InitStyle() {

        if (!skin)
            skin = Resources.Load("BR_WindowSkin") as GUISkin;

        bannerTexture = (Texture2D)Resources.Load("Editor/BR_Banner", typeof(Texture2D));

    }

    private void OnGUI() {

        GUI.skin = skin;

        DrawHeader();
        DrawMenuButtons();
        DrawToolBar();
        DrawFooter();
        Repaint();

    }

    private void DrawHeader() {

        GUILayout.Label(bannerTexture, GUILayout.Height(120));

    }

    private void DrawMenuButtons() {

        GUILayout.Space(-10);
        toolBarIndex = GUILayout.Toolbar(toolBarIndex, ToolbarNames());

    }

    #region ToolBars

    public static void WelcomePageContent() {

        EditorGUILayout.BeginVertical("window");
        GUILayout.Label("<b>Welcome!</b>");
        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("Thank you for purchasing and using <b>Burnout Racers</b>. Please read the documentation before use. Also check out the online documentation for updated info. You can reopen this window from the <b>Tools --> BCG --> Burnout Racers --> Welcome Window</b>. Have fun :)");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();

        GUILayout.Label("<b>Realistic Car Controller Pro</b>");
        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("This project includes latest version of <b>Realistic Car Controller Pro</b> for vehicle physics. This version of the package has been modified, and can't be used on another project separately.");
        EditorGUILayout.EndHorizontal();

        GUILayout.Label("<b>Photon PUN 2</b>");
        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("Project requires latest version of Photon PUN2 to work. Please import the latest version of Photon PUN2 to your project from the asset store, or from the Package Manager (Window --> Package Manager).");
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Open Photon PUN2 Asset Store Page"))
            Application.OpenURL(BR_AssetPaths.assetStorePUN2Path);

        EditorGUILayout.Separator();

        GUILayout.Label("<b>Photon PUN Configuration</b>");
        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("Photon will ask your AppID for the configuration. Please pass your AppID into your Photon Settings (Window --> Photon Unity Networking --> Highlight Server Settings). You can create a new AppID from Photon's website below.");
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Open Photon PUN2 Dashboard"))
            Application.OpenURL(BR_AssetPaths.dashboardPUN2Path);

        GUILayout.Label("<b>General Settings</b>");
        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("You can access Burnout Racers settings and Realistic Car Controller Pro settings from the 'Tools' bar.");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal("box");

        if (GUILayout.Button("Open Burnout Racers Settings"))
            Selection.activeObject = BR_Settings.Instance;

        if (GUILayout.Button("Open RCC Pro Settings"))
            Selection.activeObject = RCCP_Settings.Instance;

        EditorGUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        EditorGUILayout.EndVertical();

    }

    public static void InstallationPageContent() {

        EditorGUILayout.BeginVertical("window");

        GUILayout.Label("<b>Installation</b>");
        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("Editor will restart itself after importing the package. If you are seeing this welcome window without any red errors on your console, that means import is succeeded. \n \n Project needs Realistic Car Controller Pro and latest version of Photon PUN2 imported and installed. Burnout Racers already includes the latest version of Realistic Car Controller Pro, but you'll need to install and configure latest version of Photon PUN2 by yourself.");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();

        GUILayout.Label("<b>Step 1</b>");
        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("Import the latest version of Photon PUN2 to your project.");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();

        GUILayout.Label("<b>Step 2</b>");
        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("Log in to your account on Photon's dashboard (website). Get your AppID, and paste it to the Photon's server settings. You can access the server settings from Window --> Photon Unity Networking --> Highlight Server Settings.");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();

        GUILayout.Label("<b>Step 3</b>");
        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("After installing Photon PUN2, all photon based prefabs in the project will be updated. But it may fail for some reason, you might wanna update the prefabs manually from Tools --> BCG --> Burnout Racers --> Update Prefabs.");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();

        GUILayout.Label("<b>Step 4</b>");
        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("I recommend you to restart Unity after importing Photon PUN2, because prefabs with missing scripts (such as PhotonView) will be added again. All prefabs will be updated.");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();

        GUILayout.FlexibleSpace();

        EditorGUILayout.EndVertical();

    }

    public static void UpdatePageContent() {

        EditorGUILayout.BeginVertical("window");
        GUILayout.Label("<b>Updates</b>");

        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("<b>Installed Version: </b>" + BR_Version.version.ToString());
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(6);

        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("<b>1</b>- Always backup your project before updating Burnout Racers or any asset in your project!");
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(6);

        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("<b>2</b>- If you have own assets such as prefabs, audioclips, models, scripts in Burnout Racers folder, keep your own asset outside from Burnout Racers folder.");
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(6);

        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("<b>3</b>- Delete Burnout Racers folder, and import latest version to your project.");
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(6);

        if (GUILayout.Button("Check Updates"))
            Application.OpenURL(BR_AssetPaths.assetStorePath);

        GUILayout.Space(6);

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();

    }

    public static void DemosPageContent() {

        EditorGUILayout.BeginVertical("window");

        GUILayout.Label("<b>Demo Scenes</b>");

        EditorGUILayout.Separator();
        EditorGUILayout.HelpBox("All demo scenes must be added to your Build Settings.", MessageType.Warning, true);
        EditorGUILayout.Separator();

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("<b>These scenes are demo scenes of the Burnout Racers Multiplayer.</b>");
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Main Menu"))
            EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(BR_Resources.Instance.mainMenuScene), OpenSceneMode.Single);

        if (GUILayout.Button("Gameplay 1"))
            EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(BR_Resources.Instance.gameplay1Scene), OpenSceneMode.Single);

        if (GUILayout.Button("Gameplay 2"))
            EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(BR_Resources.Instance.gameplay2Scene), OpenSceneMode.Single);

        EditorGUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();

    }

    public static void Documentations() {

        EditorGUILayout.BeginVertical("window");

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.HelpBox("Latest online documentations for scripts, settings, setup, how to do, and API.", MessageType.Info);

        if (GUILayout.Button("Documentation"))
            Application.OpenURL(BR_AssetPaths.documentations);

        if (GUILayout.Button("Youtube Tutorial Videos"))
            Application.OpenURL(BR_AssetPaths.YTVideos);

        if (GUILayout.Button("Other Assets"))
            Application.OpenURL(BR_AssetPaths.otherAssets);

        EditorGUILayout.EndVertical();

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();

    }

    #endregion

    private string[] ToolbarNames() {

        string[] names = new string[toolBars.Length];

        for (int i = 0; i < toolBars.Length; i++)
            names[i] = toolBars[i];

        return names;

    }

    private void DrawToolBar() {

        GUILayout.BeginArea(new Rect(4, 140, 592, 540));

        toolBars[toolBarIndex].Draw();

        GUILayout.EndArea();

        GUILayout.FlexibleSpace();

    }

    private void DrawFooter() {

        EditorGUILayout.BeginHorizontal("box");

        EditorGUILayout.LabelField("BoneCracker Games", EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.LabelField("Burnout Racers " + BR_Version.version, EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.LabelField("Ekrem Bugra Ozdoganlar", EditorStyles.centeredGreyMiniLabel);

        EditorGUILayout.EndHorizontal();

    }

}
