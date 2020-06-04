using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float _healthMod = 1;
    [SerializeField] private float _scoreMod = 1;

    public float HealthMod => _healthMod;
    public float ScoreMod => _scoreMod;

}
