using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VSyncSettings : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text _value;

    private void Start()
    {
        _slider.value = QualitySettings.vSyncCount;
        _value.text = QualitySettings.vSyncCount.ToString();
    }

    public void UpdateVSync(float count)
    {
        QualitySettings.vSyncCount = (int)count;

        _value.text = QualitySettings.vSyncCount.ToString(); 
    }
}
