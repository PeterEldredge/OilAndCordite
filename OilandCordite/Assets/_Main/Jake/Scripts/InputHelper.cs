using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class InputHelper : MonoBehaviour
{
    public static Player Player { get; private set; }
    public int playerIndex;

    private void Awake()
    {
        Player = ReInput.players.GetPlayer(playerIndex);
    }
}
