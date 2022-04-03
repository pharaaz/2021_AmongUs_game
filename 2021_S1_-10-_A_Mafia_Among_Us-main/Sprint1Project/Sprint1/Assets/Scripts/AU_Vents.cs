using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AU_Vents : MonoBehaviour
{
    [SerializeField] GameObject currentPanel;
    [SerializeField] GameObject nextPanel;
    [SerializeField] Transform NewPosition;
    public void ChangeVent()
    {
        Debug.Log("Changing vent");
        currentPanel.SetActive(false);
        nextPanel.SetActive(true);
        AU_PlayerController.localPlayer.transform.position = NewPosition.position;
    }
    public void ExitVent()
    {
        Debug.Log("Exiting Vent");
        currentPanel.SetActive(false);
    }
}