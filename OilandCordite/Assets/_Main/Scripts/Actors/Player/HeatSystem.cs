using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public struct OverheatedEventArgs : IGameEvent { }
    public struct BeginIgniteEventArgs : IGameEvent { }
    public struct EndIgniteEventArgs : IGameEvent { }
}

public class HeatSystem : GameEventUserObject
{
    [SerializeField] private float _maxHeat = 100f;
    [SerializeField] private float _timeToOverheat = 5f;
    [SerializeField] private float _timeToResetFromMax = 1f;
    [SerializeField] private float _overheatedTimePenalty = 2f;
    [SerializeField] private ParticleSystem _overheatingParticles;

    //Public
    public float Heat { get; private set; }
    public bool OverHeated { get; private set; }
    public bool IsIgniting = false;

    //Private
    public float _heatToBeApplied;

    public void Start()
    {
        StartCoroutine(HeatRoutine());
    }

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.PlayerInHeatTriggerEventArgs>(this, OnPlayerInHeatTrigger);
    }

    public override void Unsubscribe()
    {
        EventManager.Instance.RemoveListener<Events.PlayerInHeatTriggerEventArgs>(this, OnPlayerInHeatTrigger);
    }

    private void OnPlayerInHeatTrigger(Events.PlayerInHeatTriggerEventArgs args) => _heatToBeApplied += args.Heat;

    public void InstantCool() => Heat = 0;

    //A little too nested, should look for cleaner approach
    private IEnumerator HeatRoutine()
    {
        float timer = 0f;
        float coolingSpeed = _maxHeat / _timeToResetFromMax;

        _heatToBeApplied = 0f;

        while(true)
        {
            while (!InputHelper.Player.GetButton("Ignite") && _heatToBeApplied <= 0) yield return null;
            IsIgniting = true;

            EventManager.Instance.TriggerEvent(new Events.BeginIgniteEventArgs());

            while ((InputHelper.Player.GetButton("Ignite") && !PlayerData.Instance.SpinningOut) ||
                 _heatToBeApplied > 0f)
            {
                if (!OverHeated)
                {
                    if (Heat >= _maxHeat)
                    {
                        OverHeated = true;

                        _overheatingParticles.Play();

                        EventManager.Instance.TriggerEvent(new Events.OverheatedEventArgs());

                        Heat = _maxHeat;
                    }
                    else
                    {
                        Heat += (_maxHeat / _timeToOverheat) * Time.deltaTime + _heatToBeApplied;
                    }
                }

                _heatToBeApplied = 0f;

                yield return null;

                timer += Time.deltaTime;
            }

            EventManager.Instance.TriggerEvent(new Events.EndIgniteEventArgs());

            if (OverHeated)
            {
                yield return new WaitForSeconds(_overheatedTimePenalty);
            }

            OverHeated = false;

            _overheatingParticles.Stop();

            while (Heat > 0)
            {
                Heat = Mathf.Max(0, Heat - coolingSpeed * Time.deltaTime) + _heatToBeApplied;

                _heatToBeApplied = 0f;

                yield return null;
            }

            Heat = 0;

            IsIgniting = false;

            yield return null;
        }
    }
}
