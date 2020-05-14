using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoyEnemiesController : MonoBehaviour
{
    [SerializeField] private int _enemiesToBeKilled = 0;

    private int _childrenToWin;

    public void Start()
    {
        _childrenToWin = _enemiesToBeKilled == 0 ? 0 : transform.childCount - _enemiesToBeKilled;

        StartCoroutine(CheckMissionComplete());
    }

    public IEnumerator CheckMissionComplete()
    {
        while(true)
        {
            if (transform.childCount <= _childrenToWin)
            {
                EventManager.Instance.TriggerEvent(new MissionCompleteEventArgs());

                break;
            }

            yield return 10f;
        }
    }
}
