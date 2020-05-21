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
    [SerializeField] private GameObject _colliders;
    [SerializeField] private GameObject _particles;
    [SerializeField] private List<AttackBehaviour> _attackBehaviours;
    [SerializeField] private List<Transform> _attackPoints;

    private EnemyData _enemyData;

    private MeshRenderer _renderer;

    private float _coolDown = -1f;
    
    private bool _defeated = false;

    //Properties
    public float PiercingSpeed => _piercingSpeed;
    public float HealthGain => _healthGain;
    public bool Defeated => _defeated;

    private void Awake()
    {
        _enemyData = GetComponent<EnemyData>();
        _renderer = GetComponentInChildren<MeshRenderer>();

        _enemyData.AttackPoints = _attackPoints;
    }

    private void Start()
    {
        StartCoroutine(Attack());
    }

    public void OnDefeated()
    {
        _defeated = true;

        _renderer.enabled = false;
        _colliders.SetActive(false);

        Instantiate(_particles, transform.position, Quaternion.Euler(Vector3.zero));

        Destroy(gameObject, 5f);
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
