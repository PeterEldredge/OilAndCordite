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

            EventManager.Instance.TriggerEvent(new Events.PlayerDefeatedEnemyEventArgs(_enemy.HealthGain * attackData.HealthMod + attackData.HealthBonus, (int)(_enemy.BaseScore * attackData.ScoreMod) + attackData.ScoreBonus));
            
            //Replace 80 with variable
            if (PlayerData.Instance.Heat >= 80 && _enemy.WeaponType != WeldedWeaponType.NONE)
            {
                EventManager.Instance.TriggerEvent(new Events.PlayerGetWeaponEventArgs(_enemy.WeaponType));
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
