using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerAttackedEventArgs : IGameEvent
{
    public float Damage { get; }

    public PlayerAttackedEventArgs(float damage)
    {
        Damage = damage;
    }
}

public class Attack : MonoBehaviour
{
    [SerializeField] private float _damage;

    public float Damage => _damage;

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag(Tags.PLAYER))
        {
            EventManager.Instance.TriggerEvent(new PlayerAttackedEventArgs(_damage));
        }
    }
}
