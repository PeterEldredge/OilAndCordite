using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public struct ObstacleHitEventArgs : IGameEvent
    {
        public ContactPoint ContactPoint { get; }

        public ObstacleHitEventArgs(ContactPoint contactPoint)
        {
            ContactPoint = contactPoint;
        }
    }
}

public class CollisionSystem : MonoBehaviour
{
    public bool InSmog { get; private set; }
    public bool InGas { get; private set; }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag(Tags.OBSTACLE))
        {
            EventManager.Instance.TriggerEvent(new Events.ObstacleHitEventArgs(collision.contacts[0]));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.GAS_CLOUD))
        {
            InGas = true;
        }
        else if(other.CompareTag(Tags.SMOG))
        {
            InSmog = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.GAS_CLOUD))
        {
            InGas = false;
        }
        else if (other.CompareTag(Tags.SMOG))
        {
            InSmog = false;
        }
    }
}
