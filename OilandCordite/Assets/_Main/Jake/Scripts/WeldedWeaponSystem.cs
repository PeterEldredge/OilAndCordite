using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum WeldedWeaponType { NONE, LASER, SAW, HEATSHIELD};

namespace Events
{

    public struct PlayerGetWeaponEventArgs : IGameEvent
    {
        public WeldedWeaponType WeaponType { get; }

        public PlayerGetWeaponEventArgs(WeldedWeaponType weaponType)
        {
            WeaponType = weaponType;
        }
    }

    public struct PlayerUseWeaponEventArgs : IGameEvent
    {

        public WeldedWeaponType WeaponType { get; }

        public PlayerUseWeaponEventArgs(WeldedWeaponType weaponType)
        {
            WeaponType = weaponType;
        }
    }

    public struct PlayerRemoveWeaponEventArgs : IGameEvent { }
}

public class WeldedWeaponSystem : GameEventUserObject
{
    [SerializeField] private List<WeldedWeapon> _weldedWeapons;

    private Dictionary<WeldedWeaponType, WeldedWeapon> _weaponDictionary = new Dictionary<WeldedWeaponType, WeldedWeapon>(); 
    private WeldedWeapon _currentWeapon;

    private void AttachWeldedWeapon(Events.PlayerGetWeaponEventArgs args) => AttachWeapon(args.WeaponType);
    private void RemoveWeldedWeapon(Events.PlayerRemoveWeaponEventArgs args) => RemoveWeapon();

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.PlayerGetWeaponEventArgs>(this, AttachWeldedWeapon);
        EventManager.Instance.AddListener<Events.PlayerRemoveWeaponEventArgs>(this, RemoveWeldedWeapon);
    }

    public override void Unsubscribe()
    {
        EventManager.Instance.RemoveListener<Events.PlayerGetWeaponEventArgs>(this, AttachWeldedWeapon);
        EventManager.Instance.RemoveListener<Events.PlayerRemoveWeaponEventArgs>(this, RemoveWeldedWeapon);
    }

    private void Start()
    {
        foreach (WeldedWeapon weapon in _weldedWeapons)
        {
            _weaponDictionary.Add( weapon.WeaponType, weapon);
        }
    }

    private void Update()
    {
        if (InputHelper.Player.GetButtonDown("WeldedWeaponUse"))
        {
            _currentWeapon?.Use();
        }
    }
    
    private void AttachWeapon( WeldedWeaponType type )
    {
        if (_currentWeapon != null)
        {
            RemoveWeapon();
        }
        Debug.Log(type);
        _currentWeapon = _weaponDictionary[type];
        _currentWeapon.Create();
    }

    private void RemoveWeapon()
    {

        _currentWeapon?.Remove();
        _currentWeapon = null;
    }
}
