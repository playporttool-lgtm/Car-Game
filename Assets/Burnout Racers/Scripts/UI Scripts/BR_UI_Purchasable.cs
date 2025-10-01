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
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// UI element representing a purchasable item.
/// </summary>
public class BR_UI_Purchasable : MonoBehaviour, IPointerClickHandler {

    RCCP_UI_Color color;
    RCCP_UI_Decal decal;
    RCCP_UI_Neon neon;
    RCCP_UI_Siren siren;
    RCCP_UI_Spoiler spoiler;
    RCCP_UI_Wheel wheel;

    private bool canPurchase = true;

    private Button button;
    public bool unlocked = false;

    /// <summary>
    /// Price of the item.
    /// </summary>
    public int price = 1000;

    /// <summary>
    /// UI element displaying the price.
    /// </summary>
    public TextMeshProUGUI priceText;

    /// <summary>
    /// Image indicating that the item is locked.
    /// </summary>
    public GameObject lockImage;

    /// <summary>
    /// Audio clip to play when the item is unlocked.
    /// </summary>
    public AudioClip unlockAudioclip;

    /// <summary>
    /// Handles the pointer click event, attempting to purchase the item if it's not unlocked.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData) {

        if (!button)
            button = GetComponent<Button>();

        if (!button) {

            Debug.LogError("Button is not found on " + transform.name + ". Disabling it.");
            enabled = false;
            return;

        }

        if (!unlocked) {

            int currentMoney = BR_API.GetMoney();

            if (currentMoney >= price) {

                // Unlock the item and deduct the price from the player's money.
                unlocked = true;
                BR_API.ConsumeMoney(price);
                PlayerPrefs.SetInt("Unlocked_" + transform.name, 1);

                // Play the unlock sound if available.
                if (unlockAudioclip)
                    RCCP_AudioSource.NewAudioSource(gameObject, unlockAudioclip.name, 0f, 0f, 1f, unlockAudioclip, false, true, true);

            } else {

                // Notify the player if they don't have enough money.
                if (BR_UI_Informer.Instance)
                    BR_UI_Informer.Instance.Info((price - BR_API.GetMoney()).ToString() + " More Money To Purchase This Item!");

            }

        }

    }

    /// <summary>
    /// Initializes references to necessary components.
    /// </summary>
    private void Awake() {

        if (!button)
            button = GetComponent<Button>();

        color = GetComponent<RCCP_UI_Color>();
        decal = GetComponent<RCCP_UI_Decal>();
        neon = GetComponent<RCCP_UI_Neon>();
        siren = GetComponent<RCCP_UI_Siren>();
        spoiler = GetComponent<RCCP_UI_Spoiler>();
        wheel = GetComponent<RCCP_UI_Wheel>();

    }

    /// <summary>
    /// Checks if the item is unlocked when the object is enabled.
    /// </summary>
    private void OnEnable() {

        if (PlayerPrefs.HasKey("Unlocked_" + transform.name))
            unlocked = true;

    }

    /// <summary>
    /// Updates the state of the purchasable item.
    /// </summary>
    private void Update() {

        // Check if the player has enough money to purchase the item.
        if (BR_API.GetMoney() >= price)
            canPurchase = true;
        else
            canPurchase = false;

        // Toggle the components based on whether the item is unlocked or can be purchased.
        if (unlocked) {

            ToggleComponent(true);

        } else {

            if (canPurchase)
                ToggleComponent(true);
            else
                ToggleComponent(false);

        }

        // Update the button interactability and lock image visibility.
        if (button)
            button.interactable = unlocked;

        if (lockImage)
            lockImage.SetActive(!unlocked);

        // Update the price text to show either the price or "Owned" if the item is unlocked.
        if (priceText) {

            if (!unlocked)
                priceText.text = price.ToString("F0");
            else
                priceText.text = "Owned";

        }

    }

    /// <summary>
    /// Toggles the state of the associated components based on the item's state.
    /// </summary>
    /// <param name="state">True to enable the components, false to disable them.</param>
    private void ToggleComponent(bool state) {

        if (color)
            color.enabled = state;

        if (decal)
            decal.enabled = state;

        if (neon)
            neon.enabled = state;

        if (siren)
            siren.enabled = state;

        if (spoiler)
            spoiler.enabled = state;

        if (wheel)
            wheel.enabled = state;

    }

    // Commented out code for validating the price label and lock image during development.
    //private void OnValidate() {

    //    foreach (Transform item in GetComponentsInChildren<Transform>()) {

    //        if (item.name == "Price Label")
    //            priceText = item.GetComponent<Text>();

    //        if (item.name == "Locked")
    //            lockImage = item.gameObject;

    //    }

    //}

}
