using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public struct PlayerAttackedEventArgs : IGameEvent
    {
        public float Damage { get; }
        public float ShakeMagnitude { get; }
        public float ShakeDuration { get; }

        public PlayerAttackedEventArgs(float damage, float shakeMagnitude, float shakeDuration)
        {
            Damage = damage;
            ShakeMagnitude = shakeMagnitude;
            ShakeDuration = shakeDuration;
        }
    }
}

public class Attack : MonoBehaviour
{
    [SerializeField] private float _damage;
    [SerializeField] private float _shakeMagnitude = 2f;
    [SerializeField] private float _shakeDuration = .5f;

    public float Damage => _damage;

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag(Tags.PLAYER))
        {
            EventManager.Instance.TriggerEvent(new Events.PlayerAttackedEventArgs(_damage, _shakeMagnitude, _shakeDuration));
        }
    }
}
