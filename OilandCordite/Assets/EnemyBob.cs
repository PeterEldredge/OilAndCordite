using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBob : MonoBehaviour
{
    [SerializeField] private float _movementDelta = 10f;
    [SerializeField] private float _movementSpeed = .2f;
    [SerializeField] private AnimationCurve _movementCurve;

    private Vector3 _startingPosition;

    private void Awake()
    {
        _startingPosition = transform.position;
    }

    private void OnEnable()
    {
        StartCoroutine(EnemyBobRoutine());
    }

    private IEnumerator EnemyBobRoutine()
    {
        float timer = 0;

        while(true)
        {
            transform.position = _startingPosition + Vector3.up * _movementCurve.Evaluate(timer) * _movementDelta;

            yield return null;

            timer = (timer + Time.deltaTime * _movementSpeed) % 1;
        }
    }
}
