using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionCheckpoints : MissionController
{
    [SerializeField] private List<Checkpoint> _checkpoints;

    private int _currentPoint = 0;

    private void Awake()
    {
        for (int i = 1; i < _checkpoints.Count; i++)
        {
            _checkpoints[i].gameObject.SetActive(false);
        }
    }

    public void TickCheckpoint()
    {
        _currentPoint += 1;

        if(_currentPoint < _checkpoints.Count)
        {
            _checkpoints[_currentPoint].gameObject.SetActive(true);
        }
        else
        {
            MissionCompelete();
        }
    }
}
