using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AU_GameController : MonoBehaviour
{

    PhotonView myPV;
    int whichPlayerIsImposter;

    // Start is called before the first frame update
    void Start()
    {

        myPV = GetComponent<PhotonView>();


        if(PhotonNetwork.IsMasterClient)
        {
            PickImposter();
        }
    }

    void PickImposter()
    {
        whichPlayerIsImposter = Random.Range(0, PhotonNetwork.CurrentRoom.PlayerCount);
        myPV.RPC("RPC_SyncImposter", RpcTarget.All, whichPlayerIsImposter);
        Debug.Log("Imposter " + whichPlayerIsImposter);
    }
    
    [PunRPC]
    void RPC_SyncImposter(int playerNumber)
    {
        whichPlayerIsImposter = playerNumber;
        AU_PlayerController.localPlayer.setImposterNumber(whichPlayerIsImposter);
    }
}