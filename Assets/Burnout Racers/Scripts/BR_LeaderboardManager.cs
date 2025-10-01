//----------------------------------------------
//			Burnout Racers
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using TMPro;
using System.Collections.Generic;

#if BCG_BURNOUTRACERS_LEADERBOARD

// NOTE: Make sure to include the following namespace wherever you want to access Leaderboard Creator methods.
using Dan.Main;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace LeaderboardCreatorDemo {

    /// <summary>
    /// Uploads, loads, and shows the leaderboard.
    /// </summary>
    public class BR_LeaderboardManager : MonoBehaviour {

        /// <summary>
        /// Prefab for single item.
        /// </summary>
        public BR_UI_LeaderboardItem UILeaderboardItem;

        /// <summary>
        /// All instantiated items.
        /// </summary>
        public List<BR_UI_LeaderboardItem> items = new List<BR_UI_LeaderboardItem>();

        /// <summary>
        /// Content.
        /// </summary>
        public GameObject content;

        /// <summary>
        /// Loading text.
        /// </summary>
        public TextMeshProUGUI loadingText;

        private IEnumerator Start() {

            //  Delay.
            yield return new WaitForSeconds(1f);

            //  Only upload the score if > 1.
            if (BR_API.GetPlayerName() != "" && BR_API.GetRacePoints() >= 1)
                UploadEntry();

        }

        /// <summary>
        /// Loads all entries.
        /// </summary>
        public void LoadEntries() {

            // Q: How do I reference my own leaderboard?
            // A: Leaderboards.<NameOfTheLeaderboard>
            loadingText.gameObject.SetActive(true);
            loadingText.text = "LOADING THE LEADERBOARD, PLEASE WAIT...";
            items = content.GetComponentsInChildren<BR_UI_LeaderboardItem>().ToList();

            foreach (BR_UI_LeaderboardItem item in items)
                Destroy(item.gameObject);

            items.Clear();

            List<string> existingNames = new List<string>();

            Leaderboards.LB.GetEntries(entries => {

                if (entries.Length <= 0)
                    loadingText.text = "LEADERBOARD IS EMPTY";

                for (int i = 0; i < entries.Length; i++) {

                    if (!existingNames.Contains(entries[i].Username)) {

                        BR_UI_LeaderboardItem newItem = Instantiate(UILeaderboardItem.gameObject, content.transform).GetComponent<BR_UI_LeaderboardItem>();
                        newItem.gameObject.SetActive(true);
                        newItem.Set(entries[i].Rank, entries[i].Username, entries[i].Score, entries[i].IsMine());
                        items.Add(newItem);
                        existingNames.Add(entries[i].Username);
                        loadingText.text = "";
                        loadingText.gameObject.SetActive(false);

                    }

                }

            }, errors => {

                loadingText.text = "LEADERBOARD COULDN'T BE LOADED, PLEASE TRY AGAIN LATER...";

                //for (int i = 0; i < errors.Length; i++)
                //    print(errors[i]);

            });

        }

        /// <summary>
        /// Uploads entry of the player.
        /// </summary>
        public void UploadEntry() {

            Leaderboards.LB.UploadNewEntry(BR_API.GetPlayerName(), BR_API.GetRacePoints(), isSuccessful => {

                if (!isSuccessful)
                    BR_UI_Informer.Instance.Info("FAILED TO UPLOAD THE SCORE DUE TO AN INTERRUPTION ON CONNECTION!");

            }, errors => {

                //for (int i = 0; i < errors.Length; i++)
                //    print(errors[i]);

            });

        }

    }

}

#else

public class BR_LeaderboardManager : MonoBehaviour {

    /// <summary>
    /// Prefab for single item.
    /// </summary>
    public BR_UI_LeaderboardItem UILeaderboardItem;

    /// <summary>
    /// All instantiated items.
    /// </summary>
    public List<BR_UI_LeaderboardItem> items = new List<BR_UI_LeaderboardItem>();

    /// <summary>
    /// Content.
    /// </summary>
    public GameObject content;

    /// <summary>
    /// Loading text.
    /// </summary>
    public TextMeshProUGUI loadingText;

    public void LoadEntries() {

        BR_UI_Informer.Instance.Info("LEADERBOARD FEATURES ARE DISABLED IN THE BR_SETTINGS!");

    }

    public void UploadEntry() {



    }

}

#endif