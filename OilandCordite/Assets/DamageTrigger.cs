using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public struct PlayerInDamageTriggerEventArgs : IGameEvent
    {
        public float Damage { get; private set; }

        public PlayerInDamageTriggerEventArgs(float damage)
        {
            Damage = damage;
        }
    }
}

public class DamageTrigger : MonoBehaviour
{
    [SerializeField] private float _damagePerSecond;

    bool _applyingDamage = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.PLAYER))
        {
            _applyingDamage = true;

            StartCoroutine(ApplyDamageRoutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.PLAYER)) _applyingDamage = false;
    }

    private IEnumerator ApplyDamageRoutine()
    {
        while (_applyingDamage)
        {
            EventManager.Instance.TriggerEventImmediate(new Events.PlayerInDamageTriggerEventArgs(_damagePerSecond * Time.deltaTime));

            yield return null;
        }
    }
}
