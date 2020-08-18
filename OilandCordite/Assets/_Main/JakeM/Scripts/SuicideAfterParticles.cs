using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicideAfterParticles : MonoBehaviour
{
    [SerializeField] private bool _disableInstead;
    [SerializeField] private ParticleSystem _particleSystem;

    private void OnEnable()
    {
        if(_disableInstead)
        {
            StartCoroutine(Disable(_particleSystem.main.duration + _particleSystem.main.startLifetime.constantMax));
        }
        else
        {
            Destroy(gameObject, _particleSystem.main.duration + _particleSystem.main.startLifetime.constantMax);
        }
    }

    private IEnumerator Disable(float time)
    {
        yield return new WaitForSeconds(time);

        gameObject.SetActive(false);
    }
}
