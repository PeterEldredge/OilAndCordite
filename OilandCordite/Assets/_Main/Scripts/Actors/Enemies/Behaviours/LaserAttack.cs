using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AttackBehaviour/LaserAttack")]
public class LaserAttack : AttackBehaviour
{
    [SerializeField] int _shotsPerBurst;

    [SerializeField] float _burstFireSpeed;
    [SerializeField] float _betweenBurstFireSpeed;

    [SerializeField] protected GameObject _laser;
    [SerializeField] protected GameObject _muzzleFlash;

    private int currentBurstShots = 0;

    private Vector3 currentLookRotation;

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
                currentLookRotation = PlayerData.Instance.WorldSpacePosition - attackPoint.position;

                Instantiate(_laser, attackPoint.position, Quaternion.LookRotation(currentLookRotation));
                Instantiate(_muzzleFlash, attackPoint.position, Quaternion.LookRotation(currentLookRotation));
            }
        }

        if(_shotsPerBurst > 0)
        {
            currentBurstShots += 1;

            if(currentBurstShots >= _shotsPerBurst)
            {
                currentBurstShots = 0;

                CoolDown = _betweenBurstFireSpeed;
            }
            else
            {
                CoolDown = _burstFireSpeed;
            }
        }
    }

    public override void Track(EnemyData enemyData)
    {      
        enemyData.Object.transform.rotation = 
            Quaternion.Lerp(enemyData.WorldSpaceRotation,
                            Quaternion.LookRotation(PlayerData.Instance.WorldSpacePosition - enemyData.WorldSpacePosition), 
                            _lookSpeed * _lookSpeedModifier.Evaluate((_distanceToPlayer - _minRange) / (_maxRange - _minRange)) * Time.deltaTime);
    }
}
