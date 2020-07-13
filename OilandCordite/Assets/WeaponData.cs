using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData : MonoBehaviour
{
    [SerializeField] private List<Transform> _projectileOrigins;
    [SerializeField] private int _scoreBonus;

    [HideInInspector] public List<Transform> ProjectileOrigins => _projectileOrigins;
    [HideInInspector] public int ScoreBonus => _scoreBonus;

}
