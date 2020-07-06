using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUIController : BaseUIController
{
    [SerializeField] private Text _healthText;
    [SerializeField] private Slider _healthBar;

    private void Update()
    {
        _healthText.text = "Health:" + PlayerData.Instance.Health.ToString("F0");
        _healthBar.value = PlayerData.Instance.Health > 0f ? PlayerData.Instance.Health : 0f;
    }
}
