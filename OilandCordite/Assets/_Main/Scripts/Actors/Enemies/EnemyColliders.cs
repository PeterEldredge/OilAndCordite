using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyColliders : MonoBehaviour
{
    private Enemy _enemy;

    private void Awake()
    {
        _enemy = GetComponentInParent<Enemy>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(Tags.PLAYER) && !_enemy.Defeated)
        {
            if (collision.collider.attachedRigidbody.velocity.magnitude > _enemy.PiercingSpeed)
            {
                EventManager.Instance.TriggerEvent(new PlayerDefeatedEnemyEventArgs(_enemy.HealthGain, _enemy.BaseScore));

                _enemy.OnDefeated();
            }
        }
    }
}
