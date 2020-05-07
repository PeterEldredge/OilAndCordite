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
        _rigidbody.velocity = transform.forward * _speed;

        Destroy(gameObject, _aliveTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(Tags.OBSTACLE))
        {
            Destroy(gameObject);
        }
    }
}
