using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public struct PlayerInHeatTriggerEventArgs : IGameEvent
    {
        public float Heat { get; private set; }
        
        public PlayerInHeatTriggerEventArgs(float heat)
        {
            Heat = heat;
        }
    }
}

public class HeatTrigger : MonoBehaviour
{
    [SerializeField] private float _heatPerSecond;

    bool _applyingHeat = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.PLAYER))
        {
            _applyingHeat = true;

            StartCoroutine(ApplyHeatRoutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.PLAYER)) _applyingHeat = false;
    }

    private IEnumerator ApplyHeatRoutine()
    {
        while(_applyingHeat)
        {
            EventManager.Instance.TriggerEventImmediate(new Events.PlayerInHeatTriggerEventArgs(_heatPerSecond * Time.deltaTime));

            yield return null;
        }
    }
}
