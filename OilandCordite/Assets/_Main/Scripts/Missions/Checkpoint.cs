using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private UnityEvent _onHitEvent;

    private RaceToTheFinishController _raceToTheFinishController;

    private void Awake()
    {
        _raceToTheFinishController = GetComponentInParent<RaceToTheFinishController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(Tags.PLAYER))
        {
            _raceToTheFinishController.TickCheckpoint();

            _onHitEvent.Invoke();

            gameObject.SetActive(false);
        }
    }
}
