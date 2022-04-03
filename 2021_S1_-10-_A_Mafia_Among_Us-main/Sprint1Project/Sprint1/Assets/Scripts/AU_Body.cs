using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AU_Body : MonoBehaviour
{
    [SerializeField] SpriteRenderer bodySprite;

    //This method sets the colour of the body to newColor
    public void SetColor(Color newColor)
    {
        bodySprite.color = newColor;
    }

    //This methodd enables the deadbody by adding the transform to the player
    private void OnEnable()
    {
        if (AU_PlayerController.allBodies != null)
        {
            AU_PlayerController.allBodies.Add(transform);
        }
    }

    //This method removes the dead sprite of the dead character and notes to the debug log that the body has been reported
    public void Report()
    {
        Debug.Log("Reported");
        Destroy(gameObject);
    }
}