using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LaserAttack")]
public class LaserAttack : AttackBehaviour
{
    [SerializeField] protected GameObject _laser;

    public override bool UsageCondition(PlayerData playerData, EnemyData enemyData)
    {
        return base.UsageCondition(playerData, enemyData);
    }

    public override void Attack(EnemyData enemyData)
    {
        if(enemyData.AttackPoints.Count > 0)
        {
            foreach(Transform attackPoint in enemyData.AttackPoints)
            {
                Instantiate(_laser, attackPoint.position, Quaternion.LookRotation(PlayerData.Instance.WorldSpacePosition - attackPoint.position));
            }
        }
        else
        {
            Instantiate(_laser, enemyData.WorldSpacePosition, enemyData.WorldSpaceRotation);
        }
    }

    public override void Track(EnemyData enemyData)
    {
        Quaternion lookRotation = Quaternion.LookRotation(PlayerData.Instance.WorldSpacePosition - enemyData.WorldSpacePosition);
        enemyData.Object.transform.rotation = Quaternion.Lerp(enemyData.WorldSpaceRotation, lookRotation, _lookSpeed);
    }
}
