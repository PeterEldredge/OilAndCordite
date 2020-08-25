using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public struct ObstacleHitEventArgs : IGameEvent
    {
        public Vector3 CollisionNormal { get; }
        public float ShakeMagnitude { get; }
        public float ShakeDuration { get; }
        public bool Bounce;

        public ObstacleHitEventArgs(Vector3 collisionNormal, float magnitude, float duration, bool bounce)
        {
            CollisionNormal = collisionNormal;
            ShakeMagnitude = magnitude;
            ShakeDuration = duration;
            Bounce = bounce;
        }   
    }

    public struct GasExplosionEventArgs : IGameEvent
    {
        public float ExplosionMagnitude { get; }
        public float ShakeMagnitude { get; }
        public float ShakeDuration { get; }

        public GasExplosionEventArgs(float explosionMagnitude, float magnitude, float duration)
        {
            ExplosionMagnitude = explosionMagnitude;
            ShakeMagnitude = magnitude;
            ShakeDuration = duration;
        }
    }
}

public class CollisionSystem : GameEventUserObject
{
    public bool InSmog { get; private set; }
    public bool InGas { get; private set; }

    [SerializeField] private GameObject _gasExplosionParticles;
    [SerializeField] private float _obstacleBounceShakeMagnitude = 5f;
    [SerializeField] private float _obstacleBounceShakeDuration = 1f;
    [SerializeField] private float _obstacleBumpShakeMagnitude = 5f;
    [SerializeField] private float _obstacleBumpShakeDuration = .5f;
    [SerializeField] private float _gasShakeMagnitude = 3f;
    [SerializeField] private float _gasShakeDuration = .5f;
    [SerializeField] private float _bumpTolerance = -.7f;

    [SerializeField] private GameObject _sparksPrefab;

    private bool _canHitObstacles = true;

    private void OnCollisionEnter(Collision collision)
    { 

        if (collision.collider.CompareTag(Tags.OBSTACLE) && _canHitObstacles)
        {
            bool shouldBounce = Vector3.Dot(collision.contacts[0].normal, PlayerData.Instance.ForwardVector) < _bumpTolerance;
            Debug.Log(shouldBounce);
            Debug.Log(collision.contacts[0].normal);
            Debug.Log(Vector3.Dot(collision.contacts[0].normal, PlayerData.Instance.ForwardVector));
            EventManager.Instance.TriggerEvent(new Events.ObstacleHitEventArgs(collision.contacts[0].normal, shouldBounce ? _obstacleBounceShakeMagnitude : _obstacleBumpShakeMagnitude, shouldBounce ? _obstacleBounceShakeDuration : _obstacleBumpShakeDuration, shouldBounce));

            //Add to the pooling system
            Instantiate(_sparksPrefab, collision.contacts[0].point, Quaternion.Euler(PlayerData.Instance.ForwardVector));

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
            if ((PlayerData.Instance.IsIgniting || PlayerData.Instance.IsHeatShielded) && !PlayerData.Instance.SpinningOut && gasCloudData.active)
            {
                Instantiate(_gasExplosionParticles, transform.position, Quaternion.Euler(transform.rotation.eulerAngles));

                EventManager.Instance.TriggerEventImmediate(new Events.GasExplosionEventArgs(gasCloudData.ExplosionMagnitude, _gasShakeMagnitude, _gasShakeDuration));

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
