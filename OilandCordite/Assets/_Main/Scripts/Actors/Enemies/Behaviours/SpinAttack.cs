﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AttackBehaviour/SpinAttack")]
public class SpinAttack : AttackBehaviour
{
    [SerializeField] protected float _spinSpeed = 150f;

    public override bool UsageCondition(PlayerData playerData, EnemyData enemyData)
    {
        return base.UsageCondition(playerData, enemyData);
    }

    public override void Attack(EnemyData enemyData)
    {
        if (enemyData.SpinnerTransforms.Count > 0)
        {
            foreach(Transform spinnerTransform in enemyData.SpinnerTransforms)
            {
                spinnerTransform.localEulerAngles += new Vector3(0, _spinSpeed * Time.deltaTime, 0);
            }
        }
    }

    public override void Track(EnemyData enemyData)
    {
        Quaternion lookRotation = Quaternion.LookRotation(PlayerData.Instance.WorldSpacePosition - enemyData.WorldSpacePosition);
        enemyData.Object.transform.rotation = Quaternion.Lerp(enemyData.WorldSpaceRotation, lookRotation, _lookSpeed * Time.deltaTime);
    }
}
