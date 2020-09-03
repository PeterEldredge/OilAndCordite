using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{

    public struct DestructibleWallHitEventArgs : IGameEvent
    {
        public float ShakeMagnitude { get; }
        public float ShakeDuration { get; }

        public DestructibleWallHitEventArgs(float magnitude, float duration)
        {
            ShakeMagnitude = magnitude;
            ShakeDuration = duration;
        }
    }
}

public class DestructibleWall : MonoBehaviour
{
    [SerializeField]
    private GameObject originalObject, fracturedObject;

    [SerializeField]
    private bool useTrigger;

    [SerializeField] private float _shakeMagnitude = 5f;
    [SerializeField] private float _shakeDuration = 1f;

    private void OnTriggerEnter(Collider collider)
    {
        if ((collider.CompareTag(Tags.PLAYER) || collider.CompareTag("PlayerAttack")) && useTrigger)
        {
            EventManager.Instance.TriggerEvent(new Events.DestructibleWallHitEventArgs(_shakeMagnitude, _shakeDuration));
            SwapObjects();
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
}
