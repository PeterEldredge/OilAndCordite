using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITextUpdate : MonoBehaviour
{
    [SerializeField] private Text HealthText;
    [SerializeField] private Text HeatText;
    [SerializeField] private Slider HealthBar;
    [SerializeField] private Slider HeatBar;
    [SerializeField] private Text SpeedText;

    // Update is called once per frame
    void Update()
    {
        generateHeat();
        updateHealth();
        updateHeat();
        UpdateSpeed();
    }

    void generateHeat()
    {
        if (Input.GetMouseButton(0)&&PlayerStats.heat<99.9)
        {
            PlayerStats.changeHeat(1.5f);
        }
        else if (PlayerStats.heat>.1)
        {
            PlayerStats.changeHeat(-4.5f);
        }
    }

    void updateHealth()
    {
        HealthText.text = "Health:" + PlayerStats.health.ToString("F0");
        HealthBar.value = PlayerStats.health;
    }

    void updateHeat()
    {
        HeatText.text = "Heat:" + PlayerStats.heat.ToString("F1");
        HeatBar.value = PlayerStats.heat;
    }

    void UpdateSpeed()
    {
        SpeedText.text = "Speed:" + PlayerStats.Speed.ToString();
    }
}
