using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    static public Main Instance;

    public int switchCount;
    public GameObject winText;
    private int onCount = 0;


    public void Awake()
    {
        Instance = this;
    }

    public void SwitchChange(int points)
    {
        onCount = onCount + points;
        if (onCount == switchCount)
        {
            winText.SetActive(true);
        }
    }
}
