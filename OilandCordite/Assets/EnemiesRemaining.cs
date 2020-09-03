using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemiesRemaining : MonoBehaviour
{
    private TMP_Text _text;

    private DestoyEnemiesController _missionController;
    
    private void Awake()
    {
        object tempMissionController = FindObjectOfType(typeof(DestoyEnemiesController));

        if(tempMissionController == null)
        {
            transform.parent.gameObject.SetActive(false);
        }

        _text = GetComponent<TMP_Text>();
        _missionController = (DestoyEnemiesController)tempMissionController;
    }

    private void Update()
    {
        _text.text = _missionController.EnemiesRemaining.ToString();
    }
}
