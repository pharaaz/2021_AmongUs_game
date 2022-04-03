using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public GameObject up, on;
    public bool isOn, isUp;

    void Start()
    {
        on.SetActive(isOn);
        up.SetActive(isUp);

        if (isOn)
        {
            Main.Instance.SwitchChange(1);
        }
        
    }

    private void OnMouseUp()
    {
        Debug.Log("Clicked!");
        isOn = !isOn;
        isUp = !isUp;

        on.SetActive(isOn);
        up.SetActive(isUp);

        if (isOn)
        {
            Main.Instance.SwitchChange(1);
        }
        else
        {
            Main.Instance.SwitchChange(-1);
        }
    }

}
