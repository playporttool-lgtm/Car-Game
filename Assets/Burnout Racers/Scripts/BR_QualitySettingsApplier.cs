//----------------------------------------------
//			Burnout Racers
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Camera quality settings applier.
/// </summary>
[DisallowMultipleComponent]
public class BR_QualitySettingsApplier : MonoBehaviour {

    #region SINGLETON PATTERN
    /// <summary>
    /// Singleton instance of the BR_QualitySettingsApplier.
    /// </summary>
    private static BR_QualitySettingsApplier _instance;

    /// <summary>
    /// Public accessor for the singleton instance.
    /// </summary>
    public static BR_QualitySettingsApplier Instance {
        get {
            if (_instance == null) {
                _instance = FindFirstObjectByType<BR_QualitySettingsApplier>();
            }

            return _instance;
        }
    }
    #endregion

    private void Start() {

        // Apply the quality settings when the script starts.
        Check();

    }

    /// <summary>
    /// Checking and applying settings based on player preferences.
    /// </summary>
    public void Check() {

        // Set the camera's far clip plane distance based on saved draw distance.
        int drawD = PlayerPrefs.GetInt("DrawDistance", BR_Settings.Instance.defaultDrawDistance);
        Camera.main.farClipPlane = drawD;

        // Set the master volume.
        AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume", BR_Settings.Instance.defaultAudioVolume);

        // Set the music volume if the soundtrack manager exists.
        if (BR_SoundtrackManager.Instance)
            BR_SoundtrackManager.Instance.audioSource.volume = PlayerPrefs.GetFloat("MusicVolume", BR_Settings.Instance.defaultMusicVolume);

        // Configure the RCCP camera settings if the camera is present.
        RCCP_Camera cam = GetComponent<RCCP_Camera>();

        if (cam) {

            cam.TPSDynamic = RCCP_PlayerPrefsX.GetBool("CameraOffset", true);
            cam.TPSAutoReverse = RCCP_PlayerPrefsX.GetBool("CameraLookBack", true);
            cam.actualCamera.farClipPlane = drawD;

        }

        // Enable or disable the post-processing volume based on player preferences.
        Volume volume = GetComponent<Volume>();

        if (volume)
            volume.enabled = RCCP_PlayerPrefsX.GetBool("PP", BR_Settings.Instance.defaultPP);

        // Enable or disable shadows rendering based on player preferences.
        UniversalAdditionalCameraData cameraData = GetComponent<UniversalAdditionalCameraData>();

        if (cameraData)
            cameraData.renderShadows = RCCP_PlayerPrefsX.GetBool("Shadows", BR_Settings.Instance.defaultShadows);

    }

    private void OnEnable() {

        // Subscribe to the OnOptionsChanged event.
        BR_OptionsManager.OnOptionsChanged += OptionsManager_OnOptionsChanged;

    }

    /// <summary>
    /// Callback for when the options are changed. Re-applies settings.
    /// </summary>
    public void OptionsManager_OnOptionsChanged() {

        Check();

    }

    private void OnDisable() {

        // Unsubscribe from the OnOptionsChanged event.
        BR_OptionsManager.OnOptionsChanged -= OptionsManager_OnOptionsChanged;

    }

}
