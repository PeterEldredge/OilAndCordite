using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyColliders : MonoBehaviour
{
    private Enemy _enemy;
    private bool _canHitPlayer = true;

    private void Awake()
    {
        _enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        var attackData = collider.GetComponent<PlayerAttack>();
        if (attackData != null && !_enemy.Defeated)
        {

            if (collider.CompareTag(Tags.PLAYER) && collider.attachedRigidbody.velocity.magnitude < _enemy.PiercingSpeed)
            {
                if (_canHitPlayer)
                {
                    EventManager.Instance.TriggerEvent(new Events.ObstacleHitEventArgs(collider.transform.eulerAngles.normalized));
                    StartCoroutine(PauseEnemyCollisions());
                    return;
                }
            }

            EventManager.Instance.TriggerEvent(new Events.PlayerDefeatedEnemyEventArgs(_enemy.HealthGain * attackData.HealthMod, (int)(_enemy.BaseScore * attackData.ScoreMod)));
            
            //Replace 80 with variable
            if (PlayerData.Instance.Heat >= 80)
            {
                EventManager.Instance.TriggerEvent(new Events.PlayerGetWeaponEventArgs(_enemy.EnemyWeldedWeapon));
            }

            _enemy.OnDefeated();
            
        }
    }

    private IEnumerator PauseEnemyCollisions()
    {
        _canHitPlayer = false;

        float timer = 1f;

        while (timer > 0f)
        {
            yield return null;

            timer -= Time.deltaTime;
        }

        _canHitPlayer = true;
    }
}
