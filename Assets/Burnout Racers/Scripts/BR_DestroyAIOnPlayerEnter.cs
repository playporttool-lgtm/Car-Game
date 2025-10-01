//----------------------------------------------
//			Burnout Racers
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
//
//----------------------------------------------

#if RCCP_PHOTON && PHOTON_UNITY_NETWORKING

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BR_DestroyAIOnPlayerEnter : MonoBehaviourPunCallbacks {

    public override void OnEnable() {

        //  Adding PUN Callbacks.
        PhotonNetwork.AddCallbackTarget(this);

    }

    public override void OnDisable() {

        //  Adding PUN Callbacks.
        PhotonNetwork.RemoveCallbackTarget(this);

    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {

        if (!PhotonNetwork.IsMasterClient)
            return;

        int countOfAIVehicles = 0;

        for (int i = 0; i < RCCP_SceneManager.Instance.allVehicles.Count; i++) {

            if (RCCP_SceneManager.Instance.allVehicles[i].OtherAddonsManager.AI != null)
                countOfAIVehicles++;

        }

        if (countOfAIVehicles > PhotonNetwork.CurrentRoom.PlayerCount) {

            for (int i = 0; i < RCCP_SceneManager.Instance.allVehicles.Count; i++) {

                if (RCCP_SceneManager.Instance.allVehicles[i].OtherAddonsManager.AI != null) {

                    PhotonNetwork.Destroy(RCCP_SceneManager.Instance.allVehicles[i].gameObject);
                    break;

                }

            }

        }

    }

}

#else

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BR_DestroyAIOnPlayerEnter : MonoBehaviour {}

#endif