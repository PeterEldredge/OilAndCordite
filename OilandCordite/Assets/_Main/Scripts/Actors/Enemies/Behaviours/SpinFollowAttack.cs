using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AttackBehaviour/SpinFollowAttack")]
public class SpinFollowAttack : AttackBehaviour
{
    [SerializeField] protected float _spinSpeed = 150f;
    [SerializeField] protected float _moveSpeed = 100f;
    [SerializeField] protected float _lookAheadMultiplier = 2f;
    [SerializeField] protected float _sphereCastWidth = 75f;

    public override bool UsageCondition(PlayerData playerData, EnemyData enemyData)
    {
        return base.UsageCondition(playerData, enemyData);
    }

    public override void Attack(EnemyData enemyData)
    {
        if (enemyData.SpinnerTransforms.Count > 0)
        {
            foreach (Transform spinnerTransform in enemyData.SpinnerTransforms)
            {
                spinnerTransform.localEulerAngles += new Vector3(0, _spinSpeed * Time.deltaTime, 0);
            }
        }
    }

    public override void Track(EnemyData enemyData)
    {
        enemyData.Object.transform.rotation =
            Quaternion.Lerp(enemyData.WorldSpaceRotation,
                            Quaternion.LookRotation(PlayerData.Instance.WorldSpacePosition - enemyData.WorldSpacePosition),
                            _lookSpeed * _lookSpeedModifier.Evaluate((_distanceToPlayer - _minRange) / (_maxRange - _minRange)) * Time.deltaTime);

        Vector3 moveVector = enemyData.Object.transform.forward;      

        if (Physics.SphereCast(enemyData.Object.transform.position, _sphereCastWidth, moveVector, out RaycastHit hit, _moveSpeed * _lookAheadMultiplier * Time.deltaTime, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            if(!hit.transform.CompareTag(Tags.PLAYER))
            {
                moveVector = new Vector3(0, Mathf.Clamp(Mathf.Abs(moveVector.y), .5f, 1f) * Mathf.Sign(moveVector.y), 0);

                if(Physics.SphereCast(enemyData.Object.transform.position, _sphereCastWidth, moveVector, out _, _moveSpeed * _lookAheadMultiplier * Time.deltaTime, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                {
                    moveVector = Vector3.zero;
                }
            }
        }

        enemyData.Object.transform.position += moveVector * _moveSpeed * Time.deltaTime;
    }
}
