using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Attack
{
    [SerializeField] private float _speed;
    [SerializeField] private float _aliveTime = 5f;
    [SerializeField] private LayerMask myLayerMask;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        float tempAliveTime = _aliveTime;

        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 10000f, myLayerMask))
        {
            _aliveTime = hit.distance / _speed;
        }

        _rigidbody.velocity = transform.forward * _speed;

        StartCoroutine(Disable(tempAliveTime));
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(Tags.OBSTACLE))
        {
            StartCoroutine(Disable(0f));
        }
    }

    private IEnumerator Disable(float time)
    {
        yield return new WaitForSeconds(time);

        gameObject.SetActive(false);
    }
}
