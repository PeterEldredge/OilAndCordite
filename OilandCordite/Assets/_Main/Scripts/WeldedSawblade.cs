using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{

    public struct SawBladeBoostEventArgs : IGameEvent
    {
        public float BoostSpeed { get; }

        public SawBladeBoostEventArgs(float speed)
        {
            BoostSpeed = speed;
        }
    }
}

public class WeldedSawblade : WeldedWeapon
{
    [SerializeField] private GameObject _saw;
    [SerializeField] private float _delayTime = 1f;
    [SerializeField] private float _speedBoost = 200f;
    [SerializeField] private float _initialSpeed = 5f;
    [SerializeField] private float _maxSpeed = 10f;


    private AudioCuePlayer _acp;
    private Rotator _rotator;
    private bool _used = false;

    private void Awake()
    {
        _acp = GetComponent<AudioCuePlayer>();
        _rotator = _saw.GetComponent<Rotator>();
        _rotator.speed = _initialSpeed;
    }

    public override void Create()
    {
        //Play Animations and Sounds

        gameObject.SetActive(true);
        Clean();
    }

    public override void Use()
    {
        if (!_used)
        {
            StartCoroutine(SawSpin());
            _used = true;
        }
    }

    public override void Remove()
    {
        //Play animation, sound

        gameObject.SetActive(false);
    }

    public override void Clean()
    {
        _rotator.speed = _initialSpeed;
        _used = false;
    }

    private IEnumerator SawSpin()
    {
        float time = 0f;
        var rotatorSpeed = _initialSpeed;

        while (time < _delayTime)
        {
            _rotator.speed = Mathf.Lerp(_initialSpeed, _maxSpeed, time/_delayTime);
            time += Time.deltaTime;
            yield return null;
        }

        EventManager.Instance.TriggerEvent(new Events.SawBladeBoostEventArgs(_speedBoost));

        EventManager.Instance.TriggerEvent(new Events.PlayerRemoveWeaponEventArgs());
    }
}
