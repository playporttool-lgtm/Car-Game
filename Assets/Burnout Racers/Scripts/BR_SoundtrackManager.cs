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
using UnityEngine.SceneManagement;

/// <summary>
/// Creates and plays the main menu and gameplay soundtracks.
/// </summary>
public class BR_SoundtrackManager : MonoBehaviour {

    /// <summary>
    /// Instance.
    /// </summary>
    public static BR_SoundtrackManager Instance;

    /// <summary>
    /// Audiosource.
    /// </summary>
    public AudioSource audioSource;

    private void Awake() {

        // Getting the instance of the class.
        if (!Instance) {

            Instance = this;
            DontDestroyOnLoad(gameObject);

        } else {

            Destroy(gameObject);
            return;

        }

    }

    private void Start() {

        // Return if audiosource is not selected.
        if (!audioSource) {

            Debug.LogError("Audiosource is not selected on the " + transform.name + "!");
            enabled = false;
            return;

        }

        //  Make sure audiosource is not ignoring the pause and volume.
        audioSource.ignoreListenerPause = false;
        audioSource.ignoreListenerVolume = false;

        //  Listening the event when the scene changes.
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

        //  Setting the clip as main menu.
        audioSource.clip = BR_Settings.Instance.mainMenuSoundtrack;

        //  Playing the audiosource.
        audioSource.Play();

    }

    /// <summary>
    /// When the scene changed.
    /// </summary>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1) {

        //  Return if audiosource is null.
        if (!audioSource)
            return;

        //  Play the target clip with delay.
        StartCoroutine(PlayWithDelay(arg1.buildIndex));

    }

    /// <summary>
    /// Play the target clip with delay.
    /// </summary>
    /// <param name="arg1"></param>
    /// <returns></returns>
    private IEnumerator PlayWithDelay(int arg1) {

        // Delay.
        yield return new WaitForSeconds(1f);

        // Current clip.
        AudioClip currClip = audioSource.clip;

        //  If it's the main menu, set main menu soundtrack. Otherwise, set gameplay soundtrack.
        if (arg1 == 0)
            audioSource.clip = BR_Settings.Instance.mainMenuSoundtrack;
        else
            audioSource.clip = BR_Settings.Instance.gameplaySoundtrack;

        //  Stop and replay the audiosource if clip is changed.
        if (currClip != audioSource.clip) {

            audioSource.Stop();
            audioSource.Play();

        }

    }

}
