﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerDefeatedEnemyEventArgs : IGameEvent 
{
    public float HealthGain { get; }
    public int Score { get; }
    
    public PlayerDefeatedEnemyEventArgs(float healthGain, int score)
    {
        HealthGain = healthGain;
        Score = score;
    }
}

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _piercingSpeed;
    [SerializeField] private float _healthGain = 30f;
    [SerializeField] private int _baseScore = 250;
    [SerializeField] private GameObject _colliders;
    [SerializeField] private GameObject _particles;
    [SerializeField] private List<AttackBehaviour> _attackBehaviours;
    [SerializeField] private List<Transform> _attackPoints;

    private EnemyData _enemyData;

    private MeshRenderer _renderer;

    private float _coolDown = -1f;

    //Properties
    public float PiercingSpeed => _piercingSpeed;
    public float HealthGain => _healthGain;
    public int BaseScore => _baseScore;
    public bool Defeated { get; private set; } = false;

    private void Awake()
    {
        _enemyData = GetComponent<EnemyData>();
        _renderer = GetComponentInChildren<MeshRenderer>();

        _enemyData.AttackPoints = _attackPoints;
    }

    private void Start()
    {
        StartCoroutine(Attack());
    }

    public void OnDefeated()
    {
        Defeated = true;

        _renderer.enabled = false;
        _colliders.SetActive(false);

        Instantiate(_particles, transform.position, Quaternion.Euler(Vector3.zero));

        Destroy(gameObject, 5f);
    }

    private IEnumerator Attack()
    {
        while(!Defeated)
        {
            if (_coolDown > 0) _coolDown -= Time.deltaTime;
            foreach (AttackBehaviour attack in _attackBehaviours)
            {
                if (attack.UsageCondition(PlayerData.Instance, _enemyData))
                {
                    if(_coolDown <= 0)
                    {
                        attack.Attack(_enemyData);
                        _coolDown = attack.CoolDown;
                    }

                    attack.Track(_enemyData);

                    break;
                }
            }

            yield return null;
        }
    }
}
