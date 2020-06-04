using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
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

            gameObject.SetActive(false);
        }
    }
}
