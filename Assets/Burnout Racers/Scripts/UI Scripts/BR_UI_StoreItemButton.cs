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

/// <summary>
/// Represents a store item button in the UI. Handles the purchase of items.
/// </summary>
public class BR_UI_StoreItemButton : MonoBehaviour {

    public float price = 4.99f;  // Price of the store item.
    public int amount = 25000;   // Amount of in-game currency or items the player receives upon purchase.

    /// <summary>
    /// Triggered when the store item button is clicked.
    /// Attempts to purchase the store item through the main menu manager.
    /// </summary>
    public void OnClick() {

        // Reference to the main menu manager.
        BR_MainMenuManager mainMenuManager = BR_MainMenuManager.Instance;

        // If the main menu manager is not found, exit.
        if (!mainMenuManager)
            return;

        // Attempt to purchase the store item.
        mainMenuManager.AttemptPurchaseStoreItem(this);

    }

}
