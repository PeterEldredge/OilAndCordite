using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITextUpdate : MonoBehaviour
{
    [SerializeField] private Text _healthText;
    [SerializeField] private Text _heatText;
    [SerializeField] private Slider _healthBar;
    [SerializeField] private Slider _heatBar;
    [SerializeField] private Text _speedText;

    // Update is called once per frame
    void Update()
    {
        UpdateHealth();
        UpdateHeat();
        UpdateSpeed();
    }

    void UpdateHealth()
    {
        _healthText.text = "Health:" + PlayerData.Instance.Health.ToString("F0");
        _healthBar.value = PlayerData.Instance.Health;
    }

    void UpdateHeat()
    {
        _heatText.text = "Heat:" + PlayerData.Instance.Heat.ToString("F1");
        _heatBar.value = PlayerData.Instance.Heat;
    }

    void UpdateSpeed()
    {
        _speedText.text = "Speed: " + PlayerData.Instance.Speed.ToString();
    }
}
