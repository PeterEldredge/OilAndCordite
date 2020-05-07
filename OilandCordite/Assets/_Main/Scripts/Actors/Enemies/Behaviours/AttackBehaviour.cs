using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackBehaviour : ScriptableObject
{
    public float CoolDown;

    [SerializeField] protected float _minRange;
    [SerializeField] protected float _maxRange;

    [SerializeField] protected float _lookSpeed = 10f;

    public virtual bool UsageCondition(PlayerData playerData, EnemyData enemyData)
    {
        float distance = Vector3.Distance(playerData.WorldSpacePosition, enemyData.WorldSpacePosition);

        return  distance >= _minRange && distance <= _maxRange;
    }

    public abstract void Attack(EnemyData enemyData);
    public abstract void Track(EnemyData enemyData);
}
