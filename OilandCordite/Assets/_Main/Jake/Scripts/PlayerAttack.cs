using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float _healthMod = 1;
    [SerializeField] private int _healthBonus = 0;
    [SerializeField] private float _scoreMod = 1;
    [SerializeField] private int _scoreBonus = 0;

    public float HealthMod => _healthMod;
    public int HealthBonus => _healthBonus;
    public float ScoreMod => _scoreMod;
    public int ScoreBonus => _scoreBonus;

}
