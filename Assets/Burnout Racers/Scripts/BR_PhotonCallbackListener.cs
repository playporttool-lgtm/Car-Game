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

#if PHOTON_UNITY_NETWORKING
using Photon.Realtime;
using Photon.Pun;
#endif

#if PHOTON_UNITY_NETWORKING

public class BR_PhotonCallbackListener : MonoBehaviourPunCallbacks {

    public override void OnEnable() {

        base.OnEnable();

    }

    public override void OnDisable() {

        base.OnDisable();

    }

    public override void OnDisconnected(DisconnectCause cause) {

        if (BR_UI_Informer.Instance)
            BR_UI_Informer.Instance.Info("Disconnected from server, loading the main menu...");

        PhotonNetwork.LoadLevel(0);

    }

    public override void OnLeftRoom() {

        PhotonNetwork.LoadLevel(0);

    }

    public override void OnMasterClientSwitched(Player newMasterClient) {

        if (BR_UI_Informer.Instance)
            BR_UI_Informer.Instance.Info("Master client has been switched to: " + newMasterClient.NickName);

        List<RCCP_CarController> allCars = new List<RCCP_CarController>();

        for (int i = 0; i < RCCP_SceneManager.Instance.allVehicles.Count; i++) {

            if (RCCP_SceneManager.Instance.allVehicles[i] != null && RCCP_SceneManager.Instance.allVehicles[i].OtherAddonsManager.AI && RCCP_SceneManager.Instance.allVehicles[i].OtherAddonsManager.AI.enabled)
                allCars.Add(RCCP_SceneManager.Instance.allVehicles[i]);

        }

        for (int i = 0; i < allCars.Count; i++) {

            if (allCars[i] != null && allCars[i].GetComponent<PhotonView>()) {

                // Check if the photonView's current owner is the previous master client
                if (allCars[i].GetComponent<PhotonView>().Owner != null && allCars[i].GetComponent<PhotonView>().Owner.IsMasterClient) {

                    allCars[i].Rigid.isKinematic = true;

                    // Transfer ownership to the new master client
                    allCars[i].GetComponent<PhotonView>().TransferOwnership(newMasterClient);
                    Debug.Log("Transferred ownership of object: " + allCars[i].GetComponent<PhotonView>().gameObject.name + " to " + newMasterClient.NickName);

                }

            }

        }

        StartCoroutine(UnfreezeAIVehicles(allCars));

    }

    // This method is called when any player leaves the room
    public override void OnPlayerLeftRoom(Player otherPlayer) {

        Debug.Log(otherPlayer.NickName + " has left the room.");

        List<RCCP_CarController> allCars = new List<RCCP_CarController>();

        for (int i = 0; i < RCCP_SceneManager.Instance.allVehicles.Count; i++) {

            if (RCCP_SceneManager.Instance.allVehicles[i] != null && !RCCP_SceneManager.Instance.allVehicles[i].OtherAddonsManager.AI)
                allCars.Add(RCCP_SceneManager.Instance.allVehicles[i]);

        }

        for (int i = 0; i < allCars.Count; i++) {

            if (allCars[i] != null && allCars[i].GetComponent<PhotonView>()) {

                // Check if the photonView's current owner is the previous master client
                if (allCars[i].GetComponent<PhotonView>().CreatorActorNr == otherPlayer.ActorNumber) {

                    PhotonNetwork.Destroy(allCars[i].gameObject);
                    Debug.Log("Destroyed object owned by: " + otherPlayer.NickName);

                }

            }

        }

    }

    private IEnumerator UnfreezeAIVehicles(List<RCCP_CarController> allCars) {

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < allCars.Count; i++) {

            if (allCars[i] != null)
                allCars[i].Rigid.isKinematic = false;

        }

    }

}

#else

public class BR_PhotonCallbackListener : MonoBehaviour
{
}

#endif
