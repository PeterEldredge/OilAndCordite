using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceToTheFinishController : MissionController
{
    [SerializeField] private List<Checkpoint> _checkpoints;

    private int _currentPoint = 0;

    private AudioCuePlayer _audioCuePlayer;

    private void Awake()
    {
        for (int i = 1; i < _checkpoints.Count; i++)
        {
            //_checkpoints[i].gameObject.SetActive(false);
        }

        _audioCuePlayer = GetComponent<AudioCuePlayer>();
    }

    public void TickCheckpoint()
    {
        _audioCuePlayer?.PlayRandomSound("Goal");

        _currentPoint += 1;

        if (_currentPoint < _checkpoints.Count)
        {
            _checkpoints[_currentPoint].gameObject.SetActive(true);
        }
        else
        {
            MissionComplete();
        }
    }
}
