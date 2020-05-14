using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class InputHelper : MonoBehaviour
{
    public static Player Player { get; private set; }
    public int playerIndex;

    // Start is called before the first frame update
    void Awake()
    {
        Player = ReInput.players.GetPlayer(playerIndex);
    }

}
