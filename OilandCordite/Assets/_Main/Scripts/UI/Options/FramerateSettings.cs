using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FramerateSettings : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text _value;

    private void Start()
    {
        _slider.value = Application.targetFrameRate;
        _value.text = Application.targetFrameRate.ToString();
    }

    private void Update()
    {
        if(QualitySettings.vSyncCount > 0)
        {
            _slider.interactable = false;
        }
        else
        {
            _slider.interactable = true;
        }
    }

    public void UpdateFramerateCap(float cap)
    {
        Application.targetFrameRate = (int)cap;

        Time.fixedDeltaTime = 1f / Application.targetFrameRate;

        _value.text = Application.targetFrameRate.ToString();
    }
}
