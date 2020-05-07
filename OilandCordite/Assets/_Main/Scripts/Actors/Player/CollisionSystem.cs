using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ObstacleHitEvent : IGameEvent 
{
    public Vector3 ContactNormal { get; }

    public ObstacleHitEvent(Vector3 contactNormal)
    {
        ContactNormal = contactNormal;
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
            EventManager.Instance.TriggerEvent(new ObstacleHitEvent(collision.contacts[0].normal));
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
