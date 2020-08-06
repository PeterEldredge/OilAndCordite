using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoyEnemiesController : MissionController
{
    [SerializeField] private int _enemiesToBeKilled = 0;
    [SerializeField] private int _enemyRemainingUIThreshold;
    [SerializeField] private GameObject _uiElement;

    private bool _enemyRemainingUIEnabled = false;
    private int _childrenToWin;

    private void Awake()
    {
        _childrenToWin = _enemiesToBeKilled == 0 ? 0 : transform.childCount - _enemiesToBeKilled;

        StartCoroutine(CheckMissionComplete());
    }
    private void InstantiateUI(Transform WorkingTransform)
    {
        for (int i = 0; i < WorkingTransform.childCount; i++)
        {
            GameObject Temp = Instantiate(_uiElement);
            Temp.transform.SetParent(WorkingTransform.GetChild(i));
            Temp.transform.localPosition = new Vector3(0, 0, 0);
        }
    }
    private IEnumerator CheckMissionComplete()
    {
        while (true)
        {
            if (transform.childCount <= _childrenToWin)
            {
                MissionComplete();

                break;
            }
            if (transform.GetChild(0).gameObject.CompareTag("Enemy"))
            {
                if (transform.childCount <= _enemyRemainingUIThreshold && !_enemyRemainingUIEnabled)
                {
                    _enemyRemainingUIEnabled = true;
                    InstantiateUI(transform);
                }
            }
            else
            {
                int temp = 0;

                for (int i = 0; i < transform.childCount; i++)
                {
                    temp += transform.GetChild(i).childCount;
                }

                if (temp <= _enemyRemainingUIThreshold && !_enemyRemainingUIEnabled)
                {
                    _enemyRemainingUIEnabled = true;
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        InstantiateUI(transform.GetChild(i));
                    }
                }
            }

            yield return 10f;
        }
    }
}
