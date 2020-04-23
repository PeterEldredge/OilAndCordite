using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public PlayerData PlayerData { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;

        PlayerData = GetComponent<PlayerData>();
    }
}
