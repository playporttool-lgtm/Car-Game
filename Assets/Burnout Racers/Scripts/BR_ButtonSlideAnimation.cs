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
/// UI button slide animator. Script based.
/// </summary>
public class BR_ButtonSlideAnimation : MonoBehaviour {

    public SlideFrom slideFrom = SlideFrom.Left;
    public enum SlideFrom { Left, Right, Top, Buttom }
    public float speed = 5000f;
    public bool actWhenEnabled = false;

    private RectTransform getRect;
    private Vector2 originalPosition;
    public bool actNow = false;

    private void Awake() {

        getRect = GetComponent<RectTransform>();
        originalPosition = GetComponent<RectTransform>().anchoredPosition;

    }

    private void Start() {

        SetOffset();

    }

    private void SetOffset() {

        switch (slideFrom) {

            case SlideFrom.Left:
                GetComponent<RectTransform>().anchoredPosition = new Vector2(-2000f, originalPosition.y);
                break;
            case SlideFrom.Right:
                GetComponent<RectTransform>().anchoredPosition = new Vector2(2000f, originalPosition.y);
                break;
            case SlideFrom.Top:
                GetComponent<RectTransform>().anchoredPosition = new Vector2(originalPosition.x, 500f);
                break;
            case SlideFrom.Buttom:
                GetComponent<RectTransform>().anchoredPosition = new Vector2(originalPosition.x, -500f);
                break;

        }

    }

    private void OnEnable() {

        if (actWhenEnabled) {

            SetOffset();
            Animate();

        }

    }

    public void Animate() {

        actNow = true;

    }

    private void Update() {

        if (!actNow)
            return;

        getRect.anchoredPosition = Vector2.MoveTowards(getRect.anchoredPosition, originalPosition, Time.unscaledDeltaTime * speed);

        if (Vector2.Distance(GetComponent<RectTransform>().anchoredPosition, originalPosition) < .05f)
            GetComponent<RectTransform>().anchoredPosition = originalPosition;

        if (!actWhenEnabled)
            enabled = false;

    }

}
