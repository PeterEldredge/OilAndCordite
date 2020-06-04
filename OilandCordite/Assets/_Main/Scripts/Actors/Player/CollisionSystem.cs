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
}

public class CollisionSystem : GameEventUserObject
{
    public bool InSmog { get; private set; }
    public bool InGas { get; private set; }

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
}
