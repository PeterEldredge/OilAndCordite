using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackBehaviour : ScriptableObject
{
    public float CoolDown;
    public string AttackAudio;
    public bool LoopAttackAudio;

    [SerializeField] protected float _minRange;
    [SerializeField] protected float _maxRange;

    [SerializeField] protected float _lookSpeed = 1f;
    [SerializeField] protected AnimationCurve _lookSpeedModifier;

    protected float _distanceToPlayer = 0f;

    public virtual void InitializeDataForFrame(PlayerData playerData, EnemyData enemyData)
    {
        _distanceToPlayer = Vector3.Distance(playerData.WorldSpacePosition, enemyData.WorldSpacePosition);
    }

    public virtual bool UsageCondition(PlayerData playerData, EnemyData enemyData)
    {
        return _distanceToPlayer >= _minRange && _distanceToPlayer <= _maxRange;
    }

    public abstract void Attack(EnemyData enemyData);
    public abstract void Track(EnemyData enemyData);
}
