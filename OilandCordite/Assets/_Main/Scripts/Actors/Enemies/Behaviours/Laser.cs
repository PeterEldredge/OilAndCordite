using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Attack
{
    [SerializeField] private float _speed;
    [SerializeField] private float _aliveTime = 5f;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _rigidbody.velocity = (PlayerData.Instance.WorldSpacePosition - transform.position) * _speed;

        Destroy(gameObject, _aliveTime);
    }
}
