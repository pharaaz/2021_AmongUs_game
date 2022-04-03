using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ReactorTaskGame : MonoBehaviour
{
    [SerializeField] int nextButton;
    [SerializeField] GameObject GamePanel;
    [SerializeField] GameObject[] myObjects;
    [SerializeField] GameObject RecactorTask;
    [SerializeField] GameObject ReactorSprite;
    PhotonView myPV;


    void Start()
    {
        myPV = GetComponent<PhotonView>();
        nextButton = 1;
    }

    private void OnEnable()
    {
        nextButton = 1;
        for(int i = 0; i < myObjects.Length; i++)
        {
            myObjects[i].transform.SetSiblingIndex(Random.Range(0, 9));
        }
    }

    public void ButtonOrder(int button)
    {
        Debug.Log("Pressed");
        if (button == 10 && button == nextButton)
        {
            Debug.Log("Pass");
            nextButton = 0;
            ButtonOrderPanelClose();
            foreach(AU_PlayerController p in AU_PlayerController.playersInGame){
                if (p.playerTextField.text == myPV.Owner.NickName){
                    p.tasksCompleted++;
                }
            }
            Destroy(RecactorTask);
            Destroy(ReactorSprite);
        }
        else if (button == nextButton)
        {
            nextButton++;
            Debug.Log("Next Button" + nextButton);
        }
        else
        {
            Debug.Log("Failed");
            Debug.Log("Next Button" + nextButton);
            nextButton = 1;
            OnEnable();
        }
        
    }

    public void ButtonOrderPanelClose()
    {
        GamePanel.SetActive(false);
    }
    public void ButtonOrderPanelOpen()
    {
        GamePanel.SetActive(true);
    }
}
