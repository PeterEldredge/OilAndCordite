using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : ActorData
{
    public static PlayerData Instance { get; private set; }

    //Public
    public float Speed => _shipControl.Speed;

    public float Health => _healthSystem.Health;
    public bool IsDead => _healthSystem.IsDead;

    public float Heat => _heatSystem.Heat;
    public bool IsOverHeated => _heatSystem.OverHeated;
    public bool IsIgniting => _heatSystem.IsIgniting;

    public bool InSmog => _collisionSystem.InSmog;
    public bool InGas => _collisionSystem.InGas;

    //Private
    private ShipControlBasic _shipControl;
    private HealthSystem _healthSystem;
    private HeatSystem _heatSystem;
    private CollisionSystem _collisionSystem;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        _shipControl = GetComponent<ShipControlBasic>();
        _healthSystem = GetComponent<HealthSystem>();
        _heatSystem = GetComponent<HeatSystem>();
        _collisionSystem = GetComponentInChildren<CollisionSystem>();
    }

}
