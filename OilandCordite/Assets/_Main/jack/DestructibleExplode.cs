using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{

    public struct DestructibleExplodeHitEventArgs : IGameEvent
    {
        public float ShakeMagnitude { get; }
        public float ShakeDuration { get; }

        public DestructibleExplodeHitEventArgs(float magnitude, float duration)
        {
            ShakeMagnitude = magnitude;
            ShakeDuration = duration;
        }
    }
}

public class DestructibleExplode : MonoBehaviour
{
    [SerializeField]
    private GameObject originalObject, fracturedObject;

    [SerializeField]
    private bool useTrigger;

    [SerializeField] private float _shakeMagnitude = 5f;
    [SerializeField] private float _shakeDuration = 1f;

    [SerializeField]
    private float despawnDelay, minForce, maxForce, radius;

    private Transform _shardParent;

    private void OnTriggerEnter(Collider collider)
    {
        if ((collider.CompareTag(Tags.PLAYER) || collider.CompareTag("PlayerAttack")) && useTrigger)
        {
            EventManager.Instance.TriggerEvent(new Events.DestructibleWallHitEventArgs(_shakeMagnitude, _shakeDuration));
            SwapObjects();
            Explode();
        }
    }

    public void SwapObjects()
    {
        if (fracturedObject != null)
        {
            fracturedObject.SetActive(true);
        }

        if (originalObject != null)
        {
            originalObject.SetActive(false);
        }
        
    }

    private void Start()
    {
        _shardParent = fracturedObject.transform;
    }

    public void Explode()
    {

        foreach (Transform t in _shardParent)
        {
            var rb = t.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(Random.Range(minForce, maxForce), _shardParent.position, radius);

            }

            //Destroy(t.gameObject, despawnDelay);
        }

    }
}
