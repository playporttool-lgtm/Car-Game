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
using UnityEngine.Rendering;

/// <summary>
/// SRP Lensflare adjuster.
/// </summary>
public class BR_LensFlareAdjuster : MonoBehaviour {

    /// <summary>
    /// Actual lightsource.
    /// </summary>
    private Light _lightSource;

    /// <summary>
    /// Light source.
    /// </summary>
    private Light LightSource {

        get {

            if (_lightSource == null)
                _lightSource = GetComponent<Light>();

            return _lightSource;

        }

    }

    /// <summary>
    /// SRP Lens flare for URP / HDRP.
    /// </summary>
    private LensFlareComponentSRP lensFlare_SRP;

    /// <summary>
    /// Max flare brigthness of the light.
    /// </summary>
    [Range(0f, 10f)] public float flareBrightness = 1.5f;

    /// <summary>
    /// Calculated final flare brightness of the light.
    /// </summary>
    private float finalFlareBrightness = 0f;

    /// <summary>
    /// Use angle to simulate intensity of the flare.
    /// </summary>
    public bool useAngle = true;

    private void Update() {

        //  Operating SRP lensflares related to camera angle.
        LensFlare_SRP();

    }

    /// <summary>
    /// Operating SRP lensflares related to camera angle.
    /// </summary>
    private void LensFlare_SRP() {

        if (!lensFlare_SRP)
            lensFlare_SRP = GetComponent<LensFlareComponentSRP>();

        //  Searching for the main camera.
        Camera maincamera = Camera.main;

        //  If no main camera found, return.
        if (!maincamera)
            return;

        //  Lensflares are not affected by collider of the vehicle. They will ignore it. Below code will calculate the angle of the light-camera, and sets intensity of the lensflare.
        float distanceTocam = Vector3.Distance(transform.position, maincamera.transform.position);
        float angle = Vector3.Angle(transform.forward, maincamera.transform.position - transform.position);

        if (!useAngle)
            angle = 0f;

        if (!Mathf.Approximately(angle, 0f))
            finalFlareBrightness = flareBrightness * (8f / distanceTocam) * ((300f - (3f * angle)) / 300f) / 3f;
        else
            finalFlareBrightness = flareBrightness;

        if (finalFlareBrightness < 0)
            finalFlareBrightness = 0f;

        lensFlare_SRP.attenuationByLightShape = false;
        lensFlare_SRP.intensity = finalFlareBrightness * LightSource.intensity;

    }

}
