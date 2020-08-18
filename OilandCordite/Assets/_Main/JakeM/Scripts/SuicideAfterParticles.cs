using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicideAfterParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;

    private void Start()
    {
        Destroy(gameObject, _particleSystem.main.duration + _particleSystem.main.startLifetime.constantMax);
    }
}
