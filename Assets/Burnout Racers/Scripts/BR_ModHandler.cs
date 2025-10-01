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
/// Modification manager used with UI in the main menu.
/// </summary>
public class BR_ModHandler : MonoBehaviour {

    private static BR_ModHandler instance;

    /// <summary>
    /// Instance.
    /// </summary>
    public static BR_ModHandler Instance {

        get {

            if (instance == null)
                instance = FindFirstObjectByType<BR_ModHandler>();

            return instance;

        }

    }

    /// <summary>
    /// Current player vehicle.
    /// </summary>
    private BR_PlayerManager currentApplier;

    /// <summary>
    /// UI Panels.
    /// </summary>
    [Header("Modify Panels")]
    public GameObject colorClass;
    public GameObject wheelClass;
    public GameObject modificationClass;
    public GameObject upgradesClass;
    public GameObject decalsClass;
    public GameObject neonsClass;
    public GameObject spoilerClass;
    public GameObject sirenClass;

    /// <summary>
    /// UI Buttons.
    /// </summary>
    [Header("Modify Buttons")]
    public Button bodyPaintButton;
    public Button rimButton;
    public Button customizationButton;
    public Button upgradeButton;
    public Button decalsButton;
    public Button neonsButton;
    public Button spoilersButton;
    public Button sirensButton;

    /// <summary>
    /// Default button color for modder buttons.
    /// </summary>
    private Color orgButtonColor;

    private void Awake() {

        // Getting original color of the button.
        orgButtonColor = bodyPaintButton.image.color;

    }

    private void Update() {

        // Getting player vehicle from the MainMenuManager.
        if (BR_MainMenuManager.Instance.currentPlayerCar)
            currentApplier = BR_MainMenuManager.Instance.currentPlayerCar;
        else
            currentApplier = null;

        // Setting interactable states of the buttons depending on upgrade managers. 
        //	Ex. If spoiler manager not found, spoiler button will be disabled.
        bool paintsEnabled = false;
        bool wheelsEnabled = false;
        bool customizationEnabled = false;
        bool upgradesEnabled = false;
        bool decalsEnabled = false;
        bool neonsEnabled = false;
        bool spoilersEnabled = false;
        bool sirensEnabled = false;

        // If no any player vehicle, return.
        if (!currentApplier || (currentApplier && !currentApplier.CarController.Customizer)) {

            if (currentApplier)
                Debug.LogWarning(currentApplier.name + " has missing customizer component or player vehicle not found. Add customizer component to the vehicle to use customization features.");
            else
                Debug.LogWarning("Player vehicle not found.");

            if (bodyPaintButton)
                bodyPaintButton.interactable = paintsEnabled;

            if (rimButton)
                rimButton.interactable = wheelsEnabled;

            if (customizationButton)
                customizationButton.interactable = customizationEnabled;

            if (upgradeButton)
                upgradeButton.interactable = upgradesEnabled;

            if (decalsButton)
                decalsButton.interactable = decalsEnabled;

            if (neonsButton)
                neonsButton.interactable = neonsEnabled;

            if (spoilersButton)
                spoilersButton.interactable = spoilersEnabled;

            if (sirensButton)
                sirensButton.interactable = sirensEnabled;

            return;

        }

        // Setting interactable states of the buttons depending on upgrade managers. 
        //	Ex. If spoiler manager not found, spoiler button will be disabled.
        if (currentApplier.CarController.Customizer.UpgradeManager)
            upgradesEnabled = true;

        if (currentApplier.CarController.Customizer.DecalManager)
            decalsEnabled = true;

        if (currentApplier.CarController.Customizer.SpoilerManager)
            spoilersEnabled = true;

        if (currentApplier.CarController.Customizer.SirenManager)
            sirensEnabled = true;

        if (currentApplier.CarController.Customizer.NeonManager)
            neonsEnabled = true;

        if (currentApplier.CarController.Customizer.WheelManager)
            wheelsEnabled = true;

        if (currentApplier.CarController.Customizer.PaintManager)
            paintsEnabled = true;

        if (currentApplier.CarController.Customizer.CustomizationManager)
            customizationEnabled = true;

        if (bodyPaintButton)
            bodyPaintButton.interactable = paintsEnabled;

        if (rimButton)
            rimButton.interactable = wheelsEnabled;

        if (customizationButton)
            customizationButton.interactable = customizationEnabled;

        if (upgradeButton)
            upgradeButton.interactable = upgradesEnabled;

        if (decalsButton)
            decalsButton.interactable = decalsEnabled;

        if (neonsButton)
            neonsButton.interactable = neonsEnabled;

        if (spoilersButton)
            spoilersButton.interactable = spoilersEnabled;

        if (sirensButton)
            sirensButton.interactable = sirensEnabled;

    }

    /// <summary>
    /// Opens up the target class panel.
    /// </summary>
    /// <param name="activeClass"></param>
    public void ChooseClass(GameObject activeClass) {

        if (colorClass)
            colorClass.SetActive(false);

        if (wheelClass)
            wheelClass.SetActive(false);

        if (modificationClass)
            modificationClass.SetActive(false);

        if (upgradesClass)
            upgradesClass.SetActive(false);

        if (decalsClass)
            decalsClass.SetActive(false);

        if (neonsClass)
            neonsClass.SetActive(false);

        if (spoilerClass)
            spoilerClass.SetActive(false);

        if (sirenClass)
            sirenClass.SetActive(false);

        if (activeClass)
            activeClass.SetActive(true);

    }

    /// <summary>
    /// Checks colors of the UI buttons. Ex. If paint class is enabled, color of the button will be green. 
    /// </summary>
    /// <param name="activeButton"></param>
    public void CheckButtonColors(Button activeButton) {

        if (bodyPaintButton)
            bodyPaintButton.image.color = orgButtonColor;

        if (rimButton)
            rimButton.image.color = orgButtonColor;

        if (customizationButton)
            customizationButton.image.color = orgButtonColor;

        if (upgradeButton)
            upgradeButton.image.color = orgButtonColor;

        if (decalsButton)
            decalsButton.image.color = orgButtonColor;

        if (neonsButton)
            neonsButton.image.color = orgButtonColor;

        if (spoilersButton)
            spoilersButton.image.color = orgButtonColor;

        if (sirensButton)
            sirensButton.image.color = orgButtonColor;

        if (activeButton)
            activeButton.image.color = new Color(0f, 1f, 0f);

    }

    /// <summary>
    /// Sets auto rotation of the showrooom camera.
    /// </summary>
    /// <param name="state"></param>
    public void ToggleAutoRotation(bool state) {

        if (!Camera.main) {

            Debug.LogError("Main camera couldn't be found in the scene. Be sure your scene has a camera with main camera tag.");
            return;

        }

        if (!Camera.main.gameObject.GetComponent<BR_ShowroomCamera>()) {

            Debug.LogError("Main camera doesn't have BR_ShowroomCamera component. Therefore, auto rotation can't be used.");
            return;

        }

        Camera.main.gameObject.GetComponent<BR_ShowroomCamera>().ToggleAutoRotation(state);

    }

    /// <summary>
    /// Sets horizontal angle of the showroom camera.
    /// </summary>
    /// <param name="hor"></param>
    public void SetHorizontal(float hor) {

        if (!Camera.main) {

            Debug.LogError("Main camera couldn't be found in the scene. Be sure your scene has a camera with main camera tag.");
            return;

        }

        if (!Camera.main.gameObject.GetComponent<BR_ShowroomCamera>()) {

            Debug.LogError("Main camera doesn't have BR_ShowroomCamera component. Therefore, auto rotation can't be used.");
            return;

        }

        Camera.main.gameObject.GetComponent<BR_ShowroomCamera>().orbitX = hor;

    }
    /// <summary>
    /// Sets vertical angle of the showroom camera.
    /// </summary>
    /// <param name="ver"></param>
    public void SetVertical(float ver) {

        if (!Camera.main) {

            Debug.LogError("Main camera couldn't be found in the scene. Be sure your scene has a camera with main camera tag.");
            return;

        }

        if (!Camera.main.gameObject.GetComponent<BR_ShowroomCamera>()) {

            Debug.LogError("Main camera doesn't have BR_ShowroomCamera component. Therefore, auto rotation can't be used.");
            return;

        }

        Camera.main.gameObject.GetComponent<BR_ShowroomCamera>().orbitY = ver;

    }

    /// <summary>
    /// Resets the current vehicle in the scene.
    /// </summary>
    public void ResetVehicle() {

        if (!BR_MainMenuManager.Instance)
            return;

        if (!BR_MainMenuManager.Instance.currentPlayerCar)
            return;

        if (!BR_MainMenuManager.Instance.currentPlayerCar.CarController.Customizer)
            return;

        BR_MainMenuManager.Instance.currentPlayerCar.CarController.Customizer.Delete();
        StartCoroutine(ToggleUpgradeButtons());

    }

    /// <summary>
    /// Toggling the upgrade buttons.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ToggleUpgradeButtons() {

        RCCP_UI_Upgrade[] upgrades = FindObjectsByType<RCCP_UI_Upgrade>(FindObjectsSortMode.None);

        foreach (RCCP_UI_Upgrade item in upgrades)
            item.enabled = false;

        yield return new WaitForEndOfFrame();

        foreach (RCCP_UI_Upgrade item in upgrades)
            item.enabled = true;

    }

}
