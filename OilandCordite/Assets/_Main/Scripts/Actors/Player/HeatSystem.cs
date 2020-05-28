using System.Collections;
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
    [SerializeField] private float _timeToResetFromMax = 3f;
    [SerializeField] private float _overheatedTimePenalty = 2f;
    [SerializeField] private AnimationCurve _heatCurve;

    //Public
    public float Heat { get; private set; }
    public bool OverHeated { get; private set; }
    public bool IsIgniting = false;

    private void Update()
    {
        if (!IsIgniting && InputHelper.Player.GetAxis("Ignite") > 0 ) StartHeating();
    }

    private void StartHeating() => StartCoroutine(HeatRoutine());

    //A little too nested, should look for cleaner approach
    private IEnumerator HeatRoutine()
    {
        float timer = 0f;
        float coolingSpeed = _maxHeat / _timeToResetFromMax;

        IsIgniting = true;
        EventManager.Instance.TriggerEvent(new Events.BeginIgniteEventArgs());

        while(InputHelper.Player.GetAxis("Ignite") > 0)
        {
            if (!OverHeated)
            {
                timer += Time.deltaTime;

                if (timer > _timeToOverheat)
                {
                    OverHeated = true;
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

        while (Heat > 0)
        {
            Heat = Mathf.Max(0, Heat - coolingSpeed * Time.deltaTime);

            yield return null;
        }

        Heat = 0;

        IsIgniting = false;
    }

}
