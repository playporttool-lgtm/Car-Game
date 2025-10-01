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
using TMPro;

/// <summary>
/// UI informer panel.
/// </summary>
public class BR_UI_Informer : MonoBehaviour {

    /// <summary>
    /// Instance.
    /// </summary>
    public static BR_UI_Informer Instance;

    /// <summary>
    /// Info text.
    /// </summary>
    public TextMeshProUGUI infoText;

    /// <summary>
    /// Animator.
    /// </summary>
    public Animator animator;

    private void Awake() {

        //  Getting the info text if it's not selected.
        if (!infoText)
            infoText = GetComponent<TextMeshProUGUI>();

    }

    private void Start() {

        if (Instance == null) {

            Instance = this;
            DontDestroyOnLoad(gameObject);

        } else {

            Destroy(gameObject);
            return;

        }

    }

    public void Info(string info) {

        if (infoText)
            infoText.text = info;

        if (animator)
            animator.SetTrigger("Info");

        StartCoroutine(CloseInfo());

    }

    private IEnumerator CloseInfo() {

        yield return new WaitForSeconds(2);

        if (infoText)
            infoText.text = "";

    }

}
