using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : ActorData
{
    [HideInInspector] public List<Transform> AttackPoints;
    [HideInInspector] public List<Transform> SpinnerTransforms;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(Object.transform.position, Object.transform.forward * 1000);
    }
}
