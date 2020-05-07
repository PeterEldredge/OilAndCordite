using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerDefeatedEnemyEvent : IGameEvent 
{
    public float HealthGain { get; }
    
    public PlayerDefeatedEnemyEvent(float healthGain)
    {
        HealthGain = healthGain;
    }
}

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _piercingSpeed;
    [SerializeField] private float _healthGain = 30f;
    [SerializeField] private List<AttackBehaviour> _attackBehaviours;

    private EnemyData _enemyData;

    private float _coolDown = -1f;

    private void Awake()
    {
        _enemyData = GetComponent<EnemyData>();
    }

    private void Start()
    {
        StartCoroutine(Attack());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(Tags.PLAYER))
        {
            if(collision.collider.attachedRigidbody.velocity.magnitude > _piercingSpeed)
            {
                EventManager.Instance.TriggerEvent(new PlayerDefeatedEnemyEvent(_healthGain));
                Defeated();
            }
        }
    }

    private void Defeated()
    {
        Destroy(gameObject);
    }

    private IEnumerator Attack()
    {
        while(true)
        {
            if (_coolDown > 0) _coolDown -= Time.deltaTime;
            foreach (AttackBehaviour attack in _attackBehaviours)
            {
                if (attack.UsageCondition(PlayerData.Instance, _enemyData))
                {
                    if(_coolDown <= 0)
                    {
                        attack.Attack(_enemyData);
                        _coolDown = attack.CoolDown;
                    }

                    attack.Track(_enemyData);

                    break;
                }
            }

            yield return null;
        }
    }
}
