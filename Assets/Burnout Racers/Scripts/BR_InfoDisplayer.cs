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
/// UI Info Displayer. Used to inform the player when rewarded, out of money, and about important infos.
/// </summary>
public class BR_InfoDisplayer : MonoBehaviour {

    #region SINGLETON PATTERN
    /// <summary>
    /// Singleton instance of BR_InfoDisplayer.
    /// </summary>
    private static BR_InfoDisplayer _instance;

    /// <summary>
    /// Provides access to the singleton instance of BR_InfoDisplayer.
    /// </summary>
    public static BR_InfoDisplayer Instance {
        get {
            if (_instance == null) {
                _instance = FindFirstObjectByType<BR_InfoDisplayer>();
            }

            return _instance;
        }
    }
    #endregion

    /// <summary>
    /// The type of information being displayed.
    /// </summary>
    public InfoType infoType;

    /// <summary>
    /// Enum representing the different types of information that can be displayed.
    /// </summary>
    public enum InfoType { NotEnoughMoney, Rewarded, Info }

    /// <summary>
    /// GameObject displayed when there is not enough money.
    /// </summary>
    public GameObject notEnoughMoney;

    /// <summary>
    /// GameObject displayed when the player is rewarded.
    /// </summary>
    public GameObject reward;

    /// <summary>
    /// GameObject displayed for general information.
    /// </summary>
    public GameObject info;

    /// <summary>
    /// Text component displaying the description when there is not enough money.
    /// </summary>
    public Text notEnoughMoneyDescText;

    /// <summary>
    /// Text component displaying the description when the player is rewarded.
    /// </summary>
    public Text rewardDescText;

    /// <summary>
    /// Text component displaying the general information description.
    /// </summary>
    public Text infoDescText;

    /// <summary>
    /// Button to close the information display.
    /// </summary>
    public Button close;

    /// <summary>
    /// Displays the information based on the provided type.
    /// </summary>
    /// <param name="title">The title of the information (unused in the current implementation).</param>
    /// <param name="description">The description of the information.</param>
    /// <param name="type">The type of information to display.</param>
    public void ShowInfo(string title, string description, InfoType type) {

        switch (type) {

            case InfoType.NotEnoughMoney:
                notEnoughMoney.SetActive(true);
                notEnoughMoneyDescText.text = description;
                StartCoroutine("CloseInfoDelayed");
                break;

            case InfoType.Rewarded:
                reward.SetActive(true);
                rewardDescText.text = description;
                StartCoroutine("CloseInfoDelayed");
                break;

            case InfoType.Info:
                info.SetActive(true);
                infoDescText.text = description;
                StartCoroutine("CloseInfoDelayed");
                break;

        }

    }

    /// <summary>
    /// Closes the currently displayed information.
    /// </summary>
    public void CloseInfo() {

        notEnoughMoney.SetActive(false);
        reward.SetActive(false);
        info.SetActive(false);

    }

    /// <summary>
    /// Closes the information display after a delay of 3 seconds.
    /// </summary>
    /// <returns>An IEnumerator for the coroutine.</returns>
    private IEnumerator CloseInfoDelayed() {

        yield return new WaitForSeconds(3);

        notEnoughMoney.SetActive(false);
        reward.SetActive(false);
        info.SetActive(false);

    }

}
