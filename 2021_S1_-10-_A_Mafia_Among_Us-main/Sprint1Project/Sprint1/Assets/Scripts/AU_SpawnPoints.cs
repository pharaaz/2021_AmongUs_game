using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AU_SpawnPoints : MonoBehaviour
{
    public static AU_SpawnPoints instance;
    
    public Transform[] spawnPoints;
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }
}