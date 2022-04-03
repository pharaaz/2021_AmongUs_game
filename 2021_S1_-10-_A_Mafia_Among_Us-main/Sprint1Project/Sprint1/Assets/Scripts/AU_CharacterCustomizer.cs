using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class AU_CharacterCustomizer : MonoBehaviour
{
    [SerializeField] Color[] allColors;

    //This method is used to set the player's colour which the user is controlling
    public void SetColor(int colorIndex)
    {
        AU_PlayerController.localPlayer.SetColor(allColors[colorIndex]);
    }

    //This method is used to move to the next scene in the build
    public void NextScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

}