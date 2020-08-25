using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{

    public struct SmokeUpdraftEventArgs : IGameEvent
    {
        public float VelocityUp { get; }
        public Vector3 UpdraftDirection { get;}
        public float ShakeMagnitude { get; }
        public float ShakeDuration { get; }

        public SmokeUpdraftEventArgs(float velocityUp, Vector3 updraftDirection, float magnitude, float duration)
        {
            VelocityUp = velocityUp;
            UpdraftDirection = updraftDirection;
            ShakeMagnitude = magnitude;
            ShakeDuration = duration;
        }
    }
}

public class SmokePlume : MonoBehaviour
{
    [SerializeField] float _velocityUp = 100f;
    [SerializeField] float _shakeMagnitude = 3f;
    [SerializeField] float _shakeDuration = 1f;

    [SerializeField] float _inactiveTimer = .5f;

    [SerializeField] GameObject _colliders;

    private ParticleSystem _ps;
    bool _active = true;

    private void Start()
    {
        _ps = GetComponent<ParticleSystem>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && _active)
        {
            EventManager.Instance.TriggerEvent(new Events.SmokeUpdraftEventArgs(_velocityUp, transform.forward, _shakeMagnitude, _shakeDuration));
            SmokeToggle(false);
            StartCoroutine(ReactivateTimer());
        }
    }

    private IEnumerator ReactivateTimer()
    {
        float time = 0f;

        while (time < _inactiveTimer)
        {
            time += Time.deltaTime;
            yield return null;
        }

        SmokeToggle(true);
    }

    private void SmokeToggle(bool active)
    {
        _active = active;
        _colliders.SetActive(_active);
        if (active) { _ps.Play(false); }
        else { _ps.Stop(); } 
    }
}
