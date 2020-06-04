using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : ActorData
{
    [HideInInspector] public List<Transform> AttackPoints;
    [HideInInspector] public List<Transform> SpinnerTransforms;
}
