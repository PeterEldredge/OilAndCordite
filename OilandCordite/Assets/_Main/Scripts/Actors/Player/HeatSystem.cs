using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatSystem : MonoBehaviour
{
    [SerializeField] private float _maxHeat = 100f;
    [SerializeField] private float _timeToOverheat = 5f;
    [SerializeField] private float _timeToResetFromOverHeat = 3f;
    [SerializeField] private AnimationCurve _heatCurve;

    //Public
    public float Heat { get; private set; }
    public bool OverHeated { get; private set; }

    //Private
    private bool _heating = false;

    private void Update()
    {
        if (!_heating && Input.GetMouseButtonDown(0)) StartHeating();
    }

    private void StartHeating() => StartCoroutine(HeatRoutine());

    //A little too nested, should look for cleaner approach
    private IEnumerator HeatRoutine()
    {
        float timer = 0f;
        float heatingPercentage = 0f;
        float coolingSpeed = _maxHeat / _timeToResetFromOverHeat;

        _heating = true;

        while(Input.GetMouseButton(0))
        {
            if (!OverHeated)
            {
                timer += Time.deltaTime;

                if (timer > _timeToOverheat)
                {
                    OverHeated = true;

                    Heat = _maxHeat;
                }
                else
                {
                    heatingPercentage = timer / _timeToOverheat;

                    Heat = _heatCurve.Evaluate(heatingPercentage) * _maxHeat;
                }
            }

            yield return null;
        }

        while(Heat > 0)
        {
            Heat = Mathf.Max(0, Heat - coolingSpeed * Time.deltaTime);

            yield return null;
        }

        Heat = 0;
        OverHeated = false;

        _heating = false;
    }

}
