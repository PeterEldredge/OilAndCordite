using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AttackBehaviour/VerticleFollowAttack")]
public class VerticalFollowAttack : AttackBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _verticalSafetyBuffer;

    private Vector3 _moveDirection;

    public override bool UsageCondition(PlayerData playerData, EnemyData enemyData)
    {
        return base.UsageCondition(playerData, enemyData);
    }

    public override void Attack(EnemyData enemyData)
    {
        _moveDirection = Vector3.up * _moveSpeed * Time.deltaTime * Mathf.Sign(PlayerData.Instance.WorldSpacePosition.y - enemyData.transform.position.y);

        if (!Physics.Raycast(enemyData.transform.position + Vector3.up * 5f * Mathf.Sign(_moveDirection.y), _moveDirection, out RaycastHit hit, _verticalSafetyBuffer))
        {
            enemyData.transform.Translate(_moveDirection, Space.World);
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
}
