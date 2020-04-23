using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private Rigidbody _rigidbody;

    [SerializeField] float _speed;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _rigidbody.velocity = (Player.Instance.PlayerData.WorldSpacePosition - transform.position) * _speed;

        Destroy(gameObject, 5);
    }
}
