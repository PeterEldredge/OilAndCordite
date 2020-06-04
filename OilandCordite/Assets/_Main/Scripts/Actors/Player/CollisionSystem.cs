using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public struct ObstacleHitEventArgs : IGameEvent
    {
        public Vector3 CollisionNormal { get; }

        public ObstacleHitEventArgs(Vector3 collisionNormal)
        {
            CollisionNormal = collisionNormal;
        }   
    }

    public struct GasExplosionEventArgs : IGameEvent
    {
        public float ExplosionMagnitude { get; }

        public GasExplosionEventArgs(float explosionMagnitude)
        {
            ExplosionMagnitude = explosionMagnitude;
        }
    }
}

public class CollisionSystem : GameEventUserObject
{
    public bool InSmog { get; private set; }
    public bool InGas { get; private set; }

    [SerializeField] private GameObject _gasExplosionParticles;

    private bool _canHitObstacles = true;

    private void OnCollisionEnter(Collision collision)
    { 

        if (collision.collider.CompareTag(Tags.OBSTACLE) && _canHitObstacles)
        {
            EventManager.Instance.TriggerEvent(new Events.ObstacleHitEventArgs(collision.contacts[0].normal));

            StartCoroutine(PauseObstacleCollisions());
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.GAS_CLOUD))
        {
            InGas = true;
        }

        if (other.CompareTag(Tags.SMOG))
        {
            InSmog = true;
        } 
    }

    private void OnTriggerExit(Collider other)
    {
        var gasCloudData = other.GetComponent<GasCloudData>();
        if (gasCloudData != null && other.CompareTag(Tags.GAS_CLOUD))
        {
            StartCoroutine(WaitReactivateGas(gasCloudData));
            InGas = false;
        }

        if (other.CompareTag(Tags.SMOG))
        {
            InSmog = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        var gasCloudData = other.GetComponent<GasCloudData>();
        if (gasCloudData != null && other.CompareTag(Tags.GAS_CLOUD))
        {
            if (PlayerData.Instance.IsIgniting && gasCloudData.active)
            {
                Instantiate(_gasExplosionParticles, transform.position, Quaternion.Euler(transform.rotation.eulerAngles));

                EventManager.Instance.TriggerEventImmediate(new Events.GasExplosionEventArgs(gasCloudData.ExplosionMagnitude));

                gasCloudData.SetCloudActive(false);
            }
        }
    }

    private IEnumerator PauseObstacleCollisions()
    {
        _canHitObstacles = false;

        float timer = 1f;

        while(timer > 0f)
        {
            yield return null;

            timer -= Time.deltaTime;
        }

        _canHitObstacles = true;
    }

    private IEnumerator WaitReactivateGas(GasCloudData data)
    {

        float timer = 1.5f;

        while (timer > 0f)
        {
            yield return null;

            timer -= Time.deltaTime;
        }

        data.SetCloudActive(true);
    }
}
