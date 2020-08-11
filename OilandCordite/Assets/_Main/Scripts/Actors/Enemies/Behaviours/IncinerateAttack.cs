using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AttackBehaviour/IncinerateAttack")]
public class IncinerateAttack : AttackBehaviour
{
    [SerializeField] protected GameObject _heatTrigger;

    private HeatTrigger _currentHeatTrigger;

    public override bool UsageCondition(PlayerData playerData, EnemyData enemyData)
    {
        return base.UsageCondition(playerData, enemyData);
    }

    public override void Attack(EnemyData enemyData)
    {
        if (_currentHeatTrigger != null) return;

        if (enemyData.AttackPoints.Count > 0)
        {
            foreach (Transform attackPoint in enemyData.AttackPoints)
            {
                _currentHeatTrigger = Instantiate(_heatTrigger, attackPoint).GetComponent<HeatTrigger>();
            }
        }
    }

    public override void Track(EnemyData enemyData)
    {
        enemyData.Object.transform.rotation =
            Quaternion.Lerp(enemyData.WorldSpaceRotation,
                            Quaternion.LookRotation(PlayerData.Instance.WorldSpacePosition - enemyData.WorldSpacePosition),
                            _lookSpeed * _lookSpeedModifier.Evaluate((_distanceToPlayer - _minRange) / (_maxRange - _minRange)) * Time.deltaTime);

        //Terrible
        enemyData.Object.transform.eulerAngles = new Vector3(0, enemyData.Object.transform.eulerAngles.y, 0);
    }

    public override void CleanUp(PlayerData playerData, EnemyData enemyData)
    {
        Destroy(_currentHeatTrigger.gameObject);
    }
}
