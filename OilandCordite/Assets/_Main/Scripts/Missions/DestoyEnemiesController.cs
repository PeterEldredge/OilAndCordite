using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoyEnemiesController : MissionController
{
    [SerializeField] private int _enemiesToBeKilled = 0;

    private int _childrenToWin;

    private void Awake()
    {
        _childrenToWin = _enemiesToBeKilled == 0 ? 0 : transform.childCount - _enemiesToBeKilled;

        StartCoroutine(CheckMissionComplete());
    }

    private IEnumerator CheckMissionComplete()
    {
        while(true)
        {
            if (transform.childCount <= _childrenToWin)
            {
                MissionCompelete();

                break;
            }

            yield return 10f;
        }
    }
}
