using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LaserAttack")]
public class LaserAttack : AttackBehaviour
{
    public override bool UsageCondition(PlayerData playerData, EnemyData enemyData)
    {
        return base.UsageCondition(playerData, enemyData);
    }

    public override void Attack()
    {
        Debug.LogError("In Range");
    }
}
