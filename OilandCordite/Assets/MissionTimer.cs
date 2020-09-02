using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MissionTimer : MonoBehaviour
{
    [SerializeField] private float _warningTime;
    [SerializeField] private Color _warningColor;
    
    private TMP_Text _text;

    private float _missionTime;
    private bool _warning = false;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        if (MissionControllerData.Instance.MissionController.ExposedMissionTimer <= 0)
        {
            transform.parent.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        _missionTime = MissionControllerData.Instance.MissionController.ExposedMissionTimer;

        _text.text = _missionTime.ToString().Replace('.', ':');

        if (!_text.text.Contains(":")) _text.text += ":00";
        else if (!(_text.text.Substring(_text.text.IndexOf(":") + 1).Length == 2))
        {
            _text.text += "0";
        }

        if(_missionTime < _warningTime && !_warning)
        {
            _text.color = _warningColor;
        }
    }
}
