using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
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
        //Debug.Log(collision.collider.attachedRigidbody.velocity.magnitude);
        if (collision.collider.attachedRigidbody.velocity.magnitude >= 40)
        {
            this.gameObject.SetActive(false);
        }
    }

    private IEnumerator Attack()
    {
        while(true)
        {
            if (_coolDown > 0) _coolDown -= Time.deltaTime;
            else
            {
                foreach (AttackBehaviour attack in _attackBehaviours)
                {
                    if (attack.UsageCondition(Player.Instance.PlayerData, _enemyData))
                    {
                        attack.Attack(_enemyData);
                        _coolDown = attack.CoolDown;

                        break;
                    }
                }
            }

            yield return null;
        }
    }
}
