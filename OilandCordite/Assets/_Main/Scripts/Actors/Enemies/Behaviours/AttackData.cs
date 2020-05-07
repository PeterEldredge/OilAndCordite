using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackData : ActorData
{
    //Public
    public float Damage => _attack.Damage;

    //Private
    private Attack _attack;

    private void Awake()
    {
        _attack = GetComponent<Attack>();
    }
}
