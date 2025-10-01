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
using TMPro;

/// <summary>
/// Plays animation on texts, shakes them, and changes their colors on value change.
/// </summary>
public class BR_AnimateTextOnChange : MonoBehaviour {

    /// <summary>
    /// The TextMeshProUGUI component of the text that will be animated and modified.
    /// </summary>
    private TextMeshProUGUI text;

    /// <summary>
    /// The Animation component attached to the GameObject for playing animations.
    /// </summary>
    private Animation anim;

    /// <summary>
    /// If true, the text color will change when the text value changes.
    /// </summary>
    public bool changeColorOnChange = false;

    /// <summary>
    /// If true, the text color will change to red if the new value is lower than the old value.
    /// </summary>
    public bool changeColorToRedOnLowerValue = false;

    /// <summary>
    /// If true, the color change will be permanent until manually changed.
    /// </summary>
    public bool permanent = false;

    /// <summary>
    /// If true, the text will shake when the value changes.
    /// </summary>
    public bool shake = false;

    /// <summary>
    /// If true, the text will shake when it is enabled.
    /// </summary>
    public bool shakeOnEnable = false;

    /// <summary>
    /// If true, the text will shake when the color changes.
    /// </summary>
    public bool shakeOnColorChange = false;

    /// <summary>
    /// The duration of the shake effect.
    /// </summary>
    public float damageTime = 0.1f;

    /// <summary>
    /// The range of the shake effect.
    /// </summary>
    public float shakeRange = 20f;

    /// <summary>
    /// The color to change to when the text value changes.
    /// </summary>
    public Color color;

    /// <summary>
    /// The default color of the text.
    /// </summary>
    private Color defColor;

    /// <summary>
    /// The previous text value used to detect changes.
    /// </summary>
    private string oldText;

    /// <summary>
    /// The previous color of the text used to detect changes.
    /// </summary>
    private Color oldColor;

    /// <summary>
    /// The original position of the text before any shake effects.
    /// </summary>
    private Vector3 orgPosition;

    /// <summary>
    /// The original rotation of the text before any shake effects.
    /// </summary>
    private Quaternion orgRotation;

    /// <summary>
    /// Initializes variables and sets default values.
    /// </summary>
    private void Awake() {

        text = GetComponent<TextMeshProUGUI>();
        anim = GetComponent<Animation>();

        defColor = text.color;
        oldText = text.text;
        oldColor = text.color;

    }

    /// <summary>
    /// Stores the original position and rotation of the text and triggers shaking if enabled on start.
    /// </summary>
    private void Start() {

        orgPosition = transform.localPosition;
        orgRotation = transform.localRotation;

        if (shakeOnEnable)
            Shake();

    }

    /// <summary>
    /// Updates the text color and triggers animations or shaking based on changes in the text.
    /// </summary>
    private void Update() {

        if (Time.time < 1f) {

            defColor = text.color;
            oldText = text.text;
            oldColor = text.color;

            return;

        }

        if (oldText != text.text) {

            if (anim && !anim.isPlaying)
                anim.Play();

            if (!changeColorToRedOnLowerValue && changeColorOnChange)
                text.color = color;

            else if (changeColorToRedOnLowerValue && StringToFloat(text.text) < StringToFloat(oldText))
                text.color = Color.red;

            if (shake)
                Shake();

        } else {

            if (anim && !anim.isPlaying && !permanent)
                text.color = Color.Lerp(text.color, defColor, Time.deltaTime * 10f);

        }

        if (shakeOnColorChange && oldColor != text.color) {

            Shake();

        }

        oldText = text.text;
        oldColor = text.color;

    }

    /// <summary>
    /// Converts a string to a float.
    /// </summary>
    /// <param name="stringValue">The string value to convert.</param>
    /// <returns>The float representation of the string, or 0 if conversion fails.</returns>
    private float StringToFloat(string stringValue) {

        float result;
        float.TryParse(stringValue, out result);
        return result;

    }

    /// <summary>
    /// Triggers the shake effect.
    /// </summary>
    public void Shake() {

        StartCoroutine(ShakeNum());

    }

    /// <summary>
    /// Coroutine that applies the shake effect to the text.
    /// </summary>
    /// <returns>An IEnumerator for the shake effect.</returns>
    private IEnumerator ShakeNum() {

        float elapsed = 0.0f;

        while (elapsed < damageTime) {

            elapsed += Time.deltaTime;

            float x = Random.value * shakeRange - (shakeRange / 2);
            float y = Random.value * shakeRange - (shakeRange / 2);
            float z = Random.value * shakeRange - (shakeRange / 2);

            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(orgPosition.x + x, orgPosition.y + y, orgPosition.z + z), Time.deltaTime * 50f);
            yield return null;

        }

        transform.localPosition = orgPosition;
        transform.localRotation = orgRotation;

    }

}
