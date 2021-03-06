﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public struct OverheatedEventArgs : IGameEvent { }
    public struct BeginIgniteEventArgs : IGameEvent { }
    public struct EndIgniteEventArgs : IGameEvent { }
}

public class HeatSystem : MonoBehaviour
{
    [SerializeField] private float _maxHeat = 100f;
    [SerializeField] private float _timeToOverheat = 5f;
    [SerializeField] private float _timeToResetFromMax = 1f;
    [SerializeField] private float _overheatedTimePenalty = 2f;
    [SerializeField] private AnimationCurve _heatCurve;
    [SerializeField] private ParticleSystem _overheatingParticles;

    //Public
    public float Heat { get; private set; }
    public bool OverHeated { get; private set; }
    public bool IsIgniting = false;

    private void Update()
    {
        if (!IsIgniting && !PlayerData.Instance.SpinningOut && InputHelper.Player.GetAxis("Ignite") > 0 ) StartHeating();
    }

    private void StartHeating() => StartCoroutine(HeatRoutine());

    public void InstantCool() => Heat = 0;

    //A little too nested, should look for cleaner approach
    private IEnumerator HeatRoutine()
    {
        float timer = 0f;
        float coolingSpeed = _maxHeat / _timeToResetFromMax;

        IsIgniting = true;
        EventManager.Instance.TriggerEvent(new Events.BeginIgniteEventArgs());

        while(InputHelper.Player.GetAxis("Ignite") > 0 && !PlayerData.Instance.SpinningOut)
        {
            if (!OverHeated)
            {
                timer += Time.deltaTime;

                if (timer > _timeToOverheat)
                {
                    OverHeated = true;

                    _overheatingParticles.Play();

                    EventManager.Instance.TriggerEvent(new Events.OverheatedEventArgs());

                    Heat = _maxHeat;
                }
                else
                {
                    float heatingPercentage = timer / _timeToOverheat;

                    Heat = _heatCurve.Evaluate(heatingPercentage) * _maxHeat;
                }
            }

            yield return null;
        }

        EventManager.Instance.TriggerEvent(new Events.EndIgniteEventArgs());

        if(OverHeated)
        {
            yield return new WaitForSeconds(_overheatedTimePenalty);
        }

        OverHeated = false;

        _overheatingParticles.Stop();

        while (Heat > 0)
        {
            Heat = Mathf.Max(0, Heat - coolingSpeed * Time.deltaTime);

            yield return null;
        }

        Heat = 0;

        IsIgniting = false;
    }

}
