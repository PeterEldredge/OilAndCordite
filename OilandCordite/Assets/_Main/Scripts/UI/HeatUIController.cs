using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeatUIController : BaseUIController
{
    [SerializeField] private Text _heatText;
    [SerializeField] private Slider _heatBar;

    private void Update()
    {
        _heatText.text = "Heat:" + PlayerData.Instance.Heat.ToString("F1");
        _heatBar.value = PlayerData.Instance.Heat;
    }
}
