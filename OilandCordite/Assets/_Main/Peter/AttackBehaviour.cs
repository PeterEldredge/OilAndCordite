using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackBehaviour : ScriptableObject
{
    public float CoolDown;

    [SerializeField] protected float _range;
    [SerializeField] protected GameObject _projectile;

    public virtual bool UsageCondition(PlayerData playerData, EnemyData enemyData)
    {
        return Vector3.Distance(playerData.WorldSpacePosition, enemyData.WorldSpacePosition) <= _range;
    }

    public abstract void Attack();
}
