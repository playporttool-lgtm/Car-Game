//----------------------------------------------
//			Burnout Racers
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Options manager that handles quality, gameplay, and controller settings.
/// </summary>
public class BR_OptionsManager : MonoBehaviour {

    #region SINGLETON PATTERN
    private static BR_OptionsManager _instance;

    /// <summary>
    /// Instance.
    /// </summary>
    public static BR_OptionsManager Instance {
        get {
            if (_instance == null) {
                _instance = FindFirstObjectByType<BR_OptionsManager>();
            }

            return _instance;
        }
    }
    #endregion

    /// <summary>
    /// Toggle for touch input control.
    /// </summary>
    public Toggle touch;

    /// <summary>
    /// Toggle for tilt (accelerometer) control.
    /// </summary>
    public Toggle tilt;

    /// <summary>
    /// Toggle for joystick control.
    /// </summary>
    public Toggle joystick;

    [Space()]

    /// <summary>
    /// Toggle for low quality settings.
    /// </summary>
    public Toggle low;

    /// <summary>
    /// Toggle for medium quality settings.
    /// </summary>
    public Toggle med;

    /// <summary>
    /// Toggle for high quality settings.
    /// </summary>
    public Toggle high;

    /// <summary>
    /// Toggle for ultra quality settings.
    /// </summary>
    public Toggle ultra;

    [Space()]

    /// <summary>
    /// Toggle for enabling/disabling post-processing effects.
    /// </summary>
    public Toggle PP;

    /// <summary>
    /// Toggle for enabling/disabling shadows.
    /// </summary>
    public Toggle shadows;

    [Space()]

    /// <summary>
    /// Slider for adjusting the draw distance.
    /// </summary>
    public Slider drawDistance;

    [Space()]

    /// <summary>
    /// Slider for adjusting the master volume.
    /// </summary>
    public Slider masterVolume;

    /// <summary>
    /// Slider for adjusting the music volume.
    /// </summary>
    public Slider musicVolume;

    [Space()]

    /// <summary>
    /// Toggle for enabling/disabling camera offset.
    /// </summary>
    public Toggle cameraOffset;

    /// <summary>
    /// Toggle for enabling/disabling camera look back.
    /// </summary>
    public Toggle cameraLookBack;

    /// <summary>
    /// GameObject for the quit match verification panel.
    /// </summary>
    public GameObject quitMatchVerificationPanel;

    /// <summary>
    /// Delegate for the OptionsChanged event.
    /// </summary>
    public delegate void OptionsChanged();

    /// <summary>
    /// Event triggered when any option is changed.
    /// </summary>
    public static event OptionsChanged OnOptionsChanged;

    /// <summary>
    /// Initializes options when the script is enabled.
    /// </summary>
    private void OnEnable() {

        if (quitMatchVerificationPanel)
            quitMatchVerificationPanel.SetActive(false);

        if (RCCP_Settings.Instance.mobileControllerEnabled) {

            if (PlayerPrefs.GetInt("ControllerType", 0) == 0) {

                if (touch)
                    touch.SetIsOnWithoutNotify(true);

                if (tilt)
                    tilt.SetIsOnWithoutNotify(false);

                if (joystick)
                    joystick.SetIsOnWithoutNotify(false);

            }

            if (PlayerPrefs.GetInt("ControllerType", 0) == 1) {

                if (touch)
                    touch.SetIsOnWithoutNotify(false);

                if (tilt)
                    tilt.SetIsOnWithoutNotify(true);

                if (joystick)
                    joystick.SetIsOnWithoutNotify(false);

            }

            if (PlayerPrefs.GetInt("ControllerType", 0) == 3) {

                if (touch)
                    touch.SetIsOnWithoutNotify(false);

                if (tilt)
                    tilt.SetIsOnWithoutNotify(false);

                if (joystick)
                    joystick.SetIsOnWithoutNotify(true);

            }

        }

        if (QualitySettings.GetQualityLevel() == 0) {

            if (low)
                low.SetIsOnWithoutNotify(true);

            if (high)
                high.SetIsOnWithoutNotify(false);

            if (med)
                med.SetIsOnWithoutNotify(false);

            if (ultra)
                ultra.SetIsOnWithoutNotify(false);

        }

        if (QualitySettings.GetQualityLevel() == 1) {

            if (low)
                low.SetIsOnWithoutNotify(false);

            if (high)
                high.SetIsOnWithoutNotify(false);

            if (med)
                med.SetIsOnWithoutNotify(true);

            if (ultra)
                ultra.SetIsOnWithoutNotify(false);

        }

        if (QualitySettings.GetQualityLevel() == 2) {

            if (low)
                low.SetIsOnWithoutNotify(false);

            if (high)
                high.SetIsOnWithoutNotify(true);

            if (med)
                med.SetIsOnWithoutNotify(false);

            if (ultra)
                ultra.SetIsOnWithoutNotify(false);

        }

        if (QualitySettings.GetQualityLevel() == 3) {

            if (low)
                low.SetIsOnWithoutNotify(false);

            if (high)
                high.SetIsOnWithoutNotify(false);

            if (med)
                med.SetIsOnWithoutNotify(false);

            if (ultra)
                ultra.SetIsOnWithoutNotify(true);

        }

        if (drawDistance)
            drawDistance.SetValueWithoutNotify(PlayerPrefs.GetInt("DrawDistance", BR_Settings.Instance.defaultDrawDistance));

        if (masterVolume)
            masterVolume.SetValueWithoutNotify(PlayerPrefs.GetFloat("MasterVolume", BR_Settings.Instance.defaultAudioVolume));

        if (musicVolume)
            musicVolume.SetValueWithoutNotify(PlayerPrefs.GetFloat("MusicVolume", BR_Settings.Instance.defaultMusicVolume));

        if (cameraOffset)
            cameraOffset.SetIsOnWithoutNotify(RCCP_PlayerPrefsX.GetBool("CameraOffset", true));

        if (cameraLookBack)
            cameraLookBack.SetIsOnWithoutNotify(RCCP_PlayerPrefsX.GetBool("CameraLookBack", true));

        if (PP)
            PP.SetIsOnWithoutNotify(RCCP_PlayerPrefsX.GetBool("PP", BR_Settings.Instance.defaultPP));

        if (shadows)
            shadows.SetIsOnWithoutNotify(RCCP_PlayerPrefsX.GetBool("Shadows", BR_Settings.Instance.defaultShadows));

    }

    /// <summary>
    /// Called to trigger the OnOptionsChanged event when options are updated.
    /// </summary>
    public void OnUpdate() {

        if (OnOptionsChanged != null)
            OnOptionsChanged();

    }

    /// <summary>
    /// Sets the controller type based on the selected toggle.
    /// </summary>
    /// <param name="toggle">The selected toggle.</param>
    public void SetControllerType(Toggle toggle) {

        if (toggle.isOn)
            return;

        switch (toggle.name) {

            case "Touchscreen":
                PlayerPrefs.SetInt("ControllerType", 0);
                RCCP.SetMobileController(RCCP_Settings.MobileController.TouchScreen);

                if (touch)
                    touch.SetIsOnWithoutNotify(true);
                if (tilt)
                    tilt.SetIsOnWithoutNotify(false);
                if (joystick)
                    joystick.SetIsOnWithoutNotify(false);

                break;
            case "Accelerometer":
                PlayerPrefs.SetInt("ControllerType", 1);
                RCCP.SetMobileController(RCCP_Settings.MobileController.Gyro);

                if (touch)
                    touch.SetIsOnWithoutNotify(false);
                if (tilt)
                    tilt.SetIsOnWithoutNotify(true);
                if (joystick)
                    joystick.SetIsOnWithoutNotify(false);

                break;
            case "SteeringWheel":
                PlayerPrefs.SetInt("ControllerType", 2);
                RCCP.SetMobileController(RCCP_Settings.MobileController.SteeringWheel);
                break;
            case "Joystick":
                PlayerPrefs.SetInt("ControllerType", 3);
                RCCP.SetMobileController(RCCP_Settings.MobileController.Joystick);

                if (touch)
                    touch.SetIsOnWithoutNotify(false);
                if (tilt)
                    tilt.SetIsOnWithoutNotify(false);
                if (joystick)
                    joystick.SetIsOnWithoutNotify(true);

                break;

        }

        if (OnOptionsChanged != null)
            OnOptionsChanged();

    }

    /// <summary>
    /// Sets the master volume based on the slider value.
    /// </summary>
    /// <param name="slider">The slider used to set the volume.</param>
    public void SetMasterVolume(Slider slider) {

        PlayerPrefs.SetFloat("MasterVolume", slider.value);

        if (OnOptionsChanged != null)
            OnOptionsChanged();

    }

    /// <summary>
    /// Sets the music volume based on the slider value.
    /// </summary>
    /// <param name="slider">The slider used to set the music volume.</param>
    public void SetMusicVolume(Slider slider) {

        PlayerPrefs.SetFloat("MusicVolume", slider.value);

        if (OnOptionsChanged != null)
            OnOptionsChanged();

    }

    /// <summary>
    /// Sets the quality level based on the selected toggle.
    /// </summary>
    /// <param name="toggle">The selected quality level toggle.</param>
    public void SetQuality(Toggle toggle) {

        switch (toggle.name) {

            case "Low":

                PlayerPrefs.SetInt("QualityLevel", 0);
                QualitySettings.SetQualityLevel(0);
                break;

            case "Medium":

                PlayerPrefs.SetInt("QualityLevel", 1);
                QualitySettings.SetQualityLevel(1);
                break;

            case "High":

                PlayerPrefs.SetInt("QualityLevel", 2);
                QualitySettings.SetQualityLevel(2);
                break;

            case "Ultra":

                PlayerPrefs.SetInt("QualityLevel", 3);
                QualitySettings.SetQualityLevel(3);

                break;

        }

        if (OnOptionsChanged != null)
            OnOptionsChanged();

        //Invoke(nameof(CheckQualityToggles), .5f);
        CheckQualityToggles();

    }

    /// <summary>
    /// Updates the quality toggles based on the current quality level.
    /// </summary>
    private void CheckQualityToggles() {

        if (QualitySettings.GetQualityLevel() == 0) {

            if (low)
                low.SetIsOnWithoutNotify(true);

            if (high)
                high.SetIsOnWithoutNotify(false);

            if (med)
                med.SetIsOnWithoutNotify(false);

            if (ultra)
                ultra.SetIsOnWithoutNotify(false);

        }

        if (QualitySettings.GetQualityLevel() == 1) {

            if (low)
                low.SetIsOnWithoutNotify(false);

            if (high)
                high.SetIsOnWithoutNotify(false);

            if (med)
                med.SetIsOnWithoutNotify(true);

            if (ultra)
                ultra.SetIsOnWithoutNotify(false);

        }

        if (QualitySettings.GetQualityLevel() == 2) {

            if (low)
                low.SetIsOnWithoutNotify(false);

            if (high)
                high.SetIsOnWithoutNotify(true);

            if (med)
                med.SetIsOnWithoutNotify(false);

            if (ultra)
                ultra.SetIsOnWithoutNotify(false);

        }

        if (QualitySettings.GetQualityLevel() == 3) {

            if (low)
                low.SetIsOnWithoutNotify(false);

            if (high)
                high.SetIsOnWithoutNotify(false);

            if (med)
                med.SetIsOnWithoutNotify(false);

            if (ultra)
                ultra.SetIsOnWithoutNotify(true);

        }

    }

    /// <summary>
    /// Sets the draw distance based on the slider value.
    /// </summary>
    /// <param name="slider">The slider used to set the draw distance.</param>
    public void SetDrawDistance(Slider slider) {

        PlayerPrefs.SetInt("DrawDistance", (int)slider.value);

        if (OnOptionsChanged != null)
            OnOptionsChanged();

    }

    /// <summary>
    /// Sets the camera offset option based on the toggle state.
    /// </summary>
    /// <param name="toggle">The toggle used to set the camera offset.</param>
    public void SetCameraOffset(Toggle toggle) {

        RCCP_PlayerPrefsX.SetBool("CameraOffset", toggle.isOn);

        if (OnOptionsChanged != null)
            OnOptionsChanged();

    }

    /// <summary>
    /// Sets the camera look-back option based on the toggle state.
    /// </summary>
    /// <param name="toggle">The toggle used to set the camera look-back option.</param>
    public void SetCameraLookBack(Toggle toggle) {

        RCCP_PlayerPrefsX.SetBool("CameraLookBack", toggle.isOn);

        if (OnOptionsChanged != null)
            OnOptionsChanged();

    }

    /// <summary>
    /// Sets the post-processing option based on the toggle state.
    /// </summary>
    /// <param name="toggle">The toggle used to set the post-processing option.</param>
    public void SetPP(Toggle toggle) {

        RCCP_PlayerPrefsX.SetBool("PP", toggle.isOn);

        if (OnOptionsChanged != null)
            OnOptionsChanged();

    }

    /// <summary>
    /// Sets the shadows option based on the toggle state.
    /// </summary>
    /// <param name="toggle">The toggle used to set the shadows option.</param>
    public void SetShadows(Toggle toggle) {

        RCCP_PlayerPrefsX.SetBool("Shadows", toggle.isOn);

        if (OnOptionsChanged != null)
            OnOptionsChanged();

    }

    /// <summary>
    /// Enables or disables the quit match verification panel based on the game state.
    /// </summary>
    /// <param name="state">True to enable the panel, false to disable.</param>
    public void EnableQuitVerificationPanel(bool state) {

        BR_GameplayManager gameplayManager = BR_GameplayManager.Instance;

        if (gameplayManager.gameType == BR_GameplayManager.GameType.Race && gameplayManager.raceStarted) {

            if (quitMatchVerificationPanel)
                quitMatchVerificationPanel.SetActive(state);

            return;

        }

        gameplayManager.Quit();

    }

    /// <summary>
    /// Quits the game or race, depending on the game state.
    /// </summary>
    public void QuitGame() {

        if (quitMatchVerificationPanel)
            quitMatchVerificationPanel.SetActive(false);

        BR_GameplayManager gameplayManager = BR_GameplayManager.Instance;

        if (gameplayManager)
            gameplayManager.Quit();
        else
            Application.Quit();

    }

    /// <summary>
    /// Cleans up resources when the script is disabled.
    /// </summary>
    private void OnDisable() {

        if (quitMatchVerificationPanel)
            quitMatchVerificationPanel.SetActive(false);

    }

}
