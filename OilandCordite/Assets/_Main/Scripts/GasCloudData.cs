using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasCloudData : MonoBehaviour
{
    [SerializeField] private float _explosionMagnitude = 1f;

    public float ExplosionMagnitude => _explosionMagnitude;

    public bool active = true;

    public void SetCloudActive(bool state)
    {
        active = state;
    }
}
