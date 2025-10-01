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
using UnityEditor;

public class BR_SceneManager {

    public static void MainMenu() {

        if (!EditorUtility.DisplayDialog("Creating Managers", "Main menu managers will be created and existing managers will be destroyed along with main camera and audiolistener. Managers include main camera and audiolistener as well.", "Proceed", "Cancel"))
            return;

        BR_MainMenuManager mainMenuManager = null;
        BR_MainMenuCanvas mainMenuCanvas = null;
        BR_ModHandler modHandler = null;
        BR_ShowroomCamera showroomCamera = null;
        BR_SoundtrackManager soundtrackManager = null;
        BR_UI_Informer uIInformer = null;
        RCCP_SceneManager sceneManager = null;
        Camera cam = null;
        AudioListener aListener = null;

        mainMenuManager = GameObject.FindFirstObjectByType<BR_MainMenuManager>(FindObjectsInactive.Include);
        mainMenuCanvas = GameObject.FindFirstObjectByType<BR_MainMenuCanvas>(FindObjectsInactive.Include);
        modHandler = GameObject.FindFirstObjectByType<BR_ModHandler>(FindObjectsInactive.Include);
        showroomCamera = GameObject.FindFirstObjectByType<BR_ShowroomCamera>(FindObjectsInactive.Include);
        soundtrackManager = GameObject.FindFirstObjectByType<BR_SoundtrackManager>(FindObjectsInactive.Include);
        uIInformer = GameObject.FindFirstObjectByType<BR_UI_Informer>(FindObjectsInactive.Include);
        sceneManager = GameObject.FindFirstObjectByType<RCCP_SceneManager>(FindObjectsInactive.Include);
        cam = GameObject.FindFirstObjectByType<Camera>(FindObjectsInactive.Include);
        aListener = GameObject.FindFirstObjectByType<AudioListener>(FindObjectsInactive.Include);

        if (mainMenuManager) {

            if (EditorUtility.DisplayDialog("Existing Managers Found", "Existing managers have been found in this scene. Please remove them before adding the new managers. Otherwise you'll have duplicated managers.", "Ok"))
                return;

            if (mainMenuManager.spawnPoint != null)
                GameObject.DestroyImmediate(mainMenuManager.spawnPoint.gameObject);

            if (mainMenuManager.vehicleRenderCamera)
                GameObject.DestroyImmediate(mainMenuManager.vehicleRenderCamera);

            GameObject.DestroyImmediate(mainMenuManager.gameObject);

        }

        if (mainMenuCanvas)
            GameObject.DestroyImmediate(mainMenuCanvas.gameObject);

        if (modHandler)
            GameObject.DestroyImmediate(modHandler.gameObject);

        if (showroomCamera)
            GameObject.DestroyImmediate(showroomCamera.gameObject);

        if (soundtrackManager)
            GameObject.DestroyImmediate(soundtrackManager.gameObject);

        if (uIInformer)
            GameObject.DestroyImmediate(uIInformer.gameObject);

        if (sceneManager)
            GameObject.DestroyImmediate(sceneManager.gameObject);

        if (cam)
            GameObject.DestroyImmediate(cam.gameObject);

        if (aListener)
            GameObject.DestroyImmediate(aListener.gameObject);

        GameObject instantiated = PrefabUtility.InstantiatePrefab(BR_SceneResources.Instance.mainMenu) as GameObject;
        PrefabUtility.UnpackPrefabInstance(instantiated, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);

        foreach (Transform item in instantiated.GetComponentsInChildren<Transform>(true)) {

            if (item.parent == instantiated.transform)
                item.SetParent(null);

        }

        GameObject.DestroyImmediate(instantiated);

        SceneView.lastActiveSceneView.ShowNotification(new GUIContent("All main menu managers have been created, you can change location of the spawn point now."), 3);

    }

    public static void Gameplay() {

        if (!EditorUtility.DisplayDialog("Creating Managers", "Gameplay managers will be created and existing managers will be destroyed along with main camera and audiolistener. Managers include main camera and audiolistener as well.", "Proceed", "Cancel"))
            return;

        BR_GameplayManager gameplayManager = null;
        BR_UI_CanvasManager gameplayCanvas = null;
        RCCP_Camera RCCPCamera = null;
        RCCP_CinematicCamera RCCPCinematicCamera = null;
        RCCP_FixedCamera RCCPFixedCamera = null;
        RCCP_SceneManager sceneManager = null;
        Camera cam = null;
        AudioListener aListener = null;

        gameplayManager = GameObject.FindFirstObjectByType<BR_GameplayManager>(FindObjectsInactive.Include);
        gameplayCanvas = GameObject.FindFirstObjectByType<BR_UI_CanvasManager>(FindObjectsInactive.Include);
        RCCPCamera = GameObject.FindFirstObjectByType<RCCP_Camera>(FindObjectsInactive.Include);
        RCCPCinematicCamera = GameObject.FindFirstObjectByType<RCCP_CinematicCamera>(FindObjectsInactive.Include);
        RCCPFixedCamera = GameObject.FindFirstObjectByType<RCCP_FixedCamera>(FindObjectsInactive.Include);
        sceneManager = GameObject.FindFirstObjectByType<RCCP_SceneManager>(FindObjectsInactive.Include);
        cam = GameObject.FindFirstObjectByType<Camera>(FindObjectsInactive.Include);
        aListener = GameObject.FindFirstObjectByType<AudioListener>(FindObjectsInactive.Include);

        if (gameplayManager) {

            if (EditorUtility.DisplayDialog("Existing Managers Found", "Existing managers have been found in this scene. Please remove them before adding the new managers. Otherwise you'll have duplicated managers.", "Ok"))
                return;

            if (gameplayManager.spawnPoints != null && gameplayManager.spawnPoints.Length > 0)
                GameObject.DestroyImmediate(gameplayManager.spawnPoints[0].parent.gameObject);

            GameObject.DestroyImmediate(gameplayManager.gameObject);

        }

        if (gameplayCanvas)
            GameObject.DestroyImmediate(gameplayCanvas.gameObject);

        if (RCCPCamera)
            GameObject.DestroyImmediate(RCCPCamera.gameObject);

        if (RCCPCinematicCamera)
            GameObject.DestroyImmediate(RCCPCinematicCamera.gameObject);

        if (RCCPFixedCamera)
            GameObject.DestroyImmediate(RCCPFixedCamera.gameObject);

        if (sceneManager)
            GameObject.DestroyImmediate(sceneManager.gameObject);

        if (cam)
            GameObject.DestroyImmediate(cam.gameObject);

        if (aListener)
            GameObject.DestroyImmediate(aListener.gameObject);

        GameObject instantiated = PrefabUtility.InstantiatePrefab(BR_SceneResources.Instance.gameplay) as GameObject;
        PrefabUtility.UnpackPrefabInstance(instantiated, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);

        foreach (Transform item in instantiated.GetComponentsInChildren<Transform>(true)) {

            if (item.parent == instantiated.transform)
                item.SetParent(null);

        }

        GameObject.DestroyImmediate(instantiated);

        SceneView.lastActiveSceneView.ShowNotification(new GUIContent("All gameplay managers have been created, you can change location of the spawn points now.\nAlso you can change the race path (BR_AIWaypointsContainer). Be sure your scene has a proper navigation mesh (NavMesh).\nMore info can be found in the documentation."), 15);

    }

}
