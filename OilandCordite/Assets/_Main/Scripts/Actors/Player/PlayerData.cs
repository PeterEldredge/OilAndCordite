using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : ActorData
{
    public static PlayerData Instance { get; private set; }

    //Public
    public float Speed => _shipControl.Speed;

    public float Health => _healthSystem.Health;

    public float Heat => _heatSystem.Heat;
    public bool OverHeated => _heatSystem.OverHeated;

    //Private
    private ShipControlBasic _shipControl;
    private HealthSystem _healthSystem;
    private HeatSystem _heatSystem;


    private void Awake()
    {
        if (Instance == null) Instance = this;

        _shipControl = GetComponent<ShipControlBasic>();
        _healthSystem = GetComponent<HealthSystem>();
        _heatSystem = GetComponent<HeatSystem>();
    }

}
