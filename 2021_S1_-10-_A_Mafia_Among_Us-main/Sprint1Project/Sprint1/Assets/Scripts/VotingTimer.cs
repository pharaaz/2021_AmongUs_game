using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;

public class VotingTimer : MonoBehaviour
{
    [SerializeField] float TimeLimit;

    static bool votingTime;

    //The time limit
    private float countDown;

    //Displaying the count down for voting
    [SerializeField]Text countdownDisplay;

    [SerializeField]GameObject VotingCanvas;

    List<AU_PlayerController> playerList;
    Rigidbody[] buttonArray;

    PhotonView myPV;

    void Awake()
    {
        myPV = GetComponent<PhotonView>();
        TimeLimit = 10;
        this.countDown = TimeLimit;

        playerList = AU_PlayerController.playersInGame;
        buttonArray = GetComponents<Rigidbody>();
        Debug.Log("Players in game = "+playerList.Count);
        Debug.Log("Amount of buttons = "+buttonArray.Length);

        if(!myPV.IsMine){
            return;
        }
        myPV.RPC("RPC_CallVotingCanvas", RpcTarget.All, playerList.Count);
        votingTime = true;
    }

    [PunRPC]
    void RPC_CallVotingCanvas(int playerCount){
        CallVotingCanvas(playerCount);
    }

    void CallVotingCanvas(int playerCount){
        if (!myPV.IsMine)
        {
            return;
        }
        this.VotingCanvas.SetActive(true);
        for(int i = 0; i < playerCount; i++){
            VotingCanvas.transform.GetChild(i+3).gameObject.SetActive(true);
        }

        votingTime = false;
    }

    void Update()
    {
        if(votingTime){
            CallVotingCanvas(playerList.Count);
        }
        //Increasing the time counter
        countDown -= Time.deltaTime;

        //Displaying the countdown timer on the GUI
        countdownDisplay.text = ((int)countDown).ToString();

        //If the time counter reaches the time limit then the scene changes
        if(countDown <= 0)
        {
            VotingCanvas.SetActive(false);
            countDown = TimeLimit;
        }
    }
}