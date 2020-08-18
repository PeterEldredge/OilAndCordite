using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnParticleSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject particles;

    public void SpawnParticles()
    {
        Instantiate(particles, transform.position, Quaternion.identity);
    }
}
