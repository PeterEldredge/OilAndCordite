﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
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
    [SerializeField] private List<Transform> _spinnerTransforms;
    [SerializeField] private string _defeatedCueName;
    [SerializeField] private WeldedWeaponType _enemyWeldedWeapon;

    public WeldedWeaponType EnemyWeldedWeapon => _enemyWeldedWeapon;

    private EnemyData _enemyData;

    private MeshRenderer _renderer;

    private AudioCuePlayer _acp;

    private AttackBehaviour _previousAttackBehavior;
    private AttackBehaviour _currentAttackBehaviour;

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
        _acp = GetComponent<AudioCuePlayer>();

        _enemyData.AttackPoints = _attackPoints;
        _enemyData.SpinnerTransforms = _spinnerTransforms;
    }

    private void Start()
    {
        StartCoroutine(Attack());
    }

    public void OnDefeated()
    {
        Defeated = true;

        _previousAttackBehavior = null;
        _currentAttackBehaviour = null;

        foreach(Transform tr in _spinnerTransforms)
        {
            Destroy(tr.gameObject); 
        }

        _renderer.enabled = false;
        _colliders.SetActive(false);

        _acp.PlaySound(_defeatedCueName);

        Instantiate(_particles, transform.position, Quaternion.Euler(Vector3.zero));

        Destroy(gameObject, 5f);
    }

    private IEnumerator Attack()
    {
        while(!Defeated && !PlayerData.Instance.IsDead)
        {
            _previousAttackBehavior = _currentAttackBehaviour;

            if (_coolDown > 0) _coolDown -= Time.deltaTime;

            foreach (AttackBehaviour attack in _attackBehaviours)
            {
                if (attack.UsageCondition(PlayerData.Instance, _enemyData))
                {
                    _currentAttackBehaviour = attack;

                    if(_coolDown <= 0)
                    {
                        attack.Attack(_enemyData);

                        if(!attack.LoopAttackAudio)
                        {
                            _acp.PlaySound(attack.AttackAudio);
                        }
                        else
                        {
                            if(_currentAttackBehaviour != _previousAttackBehavior)
                            {
                                StartCoroutine(AttackSoundLoop(_currentAttackBehaviour));
                            }
                        }
                        
                        _coolDown = attack.CoolDown;
                    }

                    attack.Track(_enemyData);

                    break;
                }
            }

            yield return null;
        }
    }

    private IEnumerator AttackSoundLoop(AttackBehaviour attack)
    {
        _acp.PlaySound(attack.AttackAudio);

        while(_currentAttackBehaviour == attack)
        {
            yield return null;
        }

        _acp.StopSound(attack.AttackAudio);
    }
}
