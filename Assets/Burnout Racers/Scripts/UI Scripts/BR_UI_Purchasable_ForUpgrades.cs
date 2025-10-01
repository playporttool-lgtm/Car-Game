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
/// UI purchasable item.
/// </summary>
[RequireComponent(typeof(RCCP_UI_Upgrade))]
public class BR_UI_Purchasable_ForUpgrades : MonoBehaviour, IPointerClickHandler {

    private Button upgraderButton;

    private bool canUpgrade = true;
    private bool canPurchase = true;

    public int price = 1000;
    public int maxLevel = 5;

    public TextMeshProUGUI priceText;
    public TextMeshProUGUI levelText;

    public AudioClip unlockAudioclip;

    public void OnPointerClick(PointerEventData eventData) {

        if (!upgraderButton)
            return;

        if (!levelText)
            return;

        int.TryParse(levelText.text, out int level);

        if (level >= maxLevel)
            canUpgrade = false;

        if (canUpgrade) {

            int currentMoney = BR_API.GetMoney();

            if (currentMoney >= price) {

                BR_API.ConsumeMoney(price);

                if (unlockAudioclip)
                    RCCP_AudioSource.NewAudioSource(gameObject, unlockAudioclip.name, 0f, 0f, 1f, unlockAudioclip, false, true, true);

            } else {

                if (BR_UI_Informer.Instance)
                    BR_UI_Informer.Instance.Info((price - BR_API.GetMoney()).ToString("F0") + " More Money Needed To Purchase This Item!");

            }

        }

    }

    private void Awake() {

        upgraderButton = GetComponent<Button>();

    }

    private void OnEnable() {

        int.TryParse(levelText.text, out int level);

        if (level >= maxLevel)
            canUpgrade = false;
        else
            canUpgrade = true;

    }

    private void Update() {

        if (BR_API.GetMoney() >= price)
            canPurchase = true;
        else
            canPurchase = false;

        if (upgraderButton) {

            if (canPurchase && canUpgrade)
                upgraderButton.interactable = true;
            else
                upgraderButton.interactable = false;

        }

        if (priceText) {

            if (canUpgrade)
                priceText.text = price.ToString("F0");
            else
                priceText.text = "Owned";

        }

        int.TryParse(levelText.text, out int level);

        if (level >= maxLevel)
            canUpgrade = false;

    }

    //private void OnValidate() {

    //    foreach (Transform item in GetComponentsInChildren<Transform>()) {

    //        if (item.name == "Price Label")
    //            priceText = item.GetComponent<Text>();

    //        if (item.name == "Locked")
    //            lockImage = item.gameObject;

    //    }

    //}

}
