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

    private void OnEnable()
    {
        _rigidbody.velocity = transform.forward * _speed;

        StartCoroutine(Disable(_aliveTime));
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
