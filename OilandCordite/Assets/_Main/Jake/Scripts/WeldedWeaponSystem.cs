using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeldedWeaponType { NONE, LASER }

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

    private Dictionary<WeldedWeaponType, WeldedWeapon> _weaponLibrary; 
    private List<WeldedWeapon> _currentWeapons = new List<WeldedWeapon>();

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

    private void Awake()
    {
        //foreach (WeldedWeapon weapon in _weldedWeapons)
        //{
        //    _weaponLibrary.Add(weapon.WeaponType, weapon);
        //}
    }

    private void Update()
    {
        if (InputHelper.Player.GetButtonDown("WeldedWeaponUse") && _currentWeapons != null)
        {
            //_currentWeapon.Use();
            foreach (WeldedWeapon weapon in _currentWeapons)
            {
                weapon.Use();
            }
        }
    }

    private void AttachWeapon( WeldedWeaponType weaponType )
    {
        if (_currentWeapons != null)
        {
            RemoveWeapon();
        }

        foreach (WeldedWeapon weapon in _weldedWeapons)
        {
            if(weapon.WeaponType == weaponType)
            {
                weapon.gameObject.SetActive(true);
                Debug.Log(weapon);
                _currentWeapons.Add(weapon);
            }
        }

        //_currentWeapons = _weaponLibrary[weaponType];
        //_currentWeapons.gameObject.SetActive(true);
    }

    private void RemoveWeapon()
    {
        if (_currentWeapons == null)
        {
            return;
        }

        foreach (WeldedWeapon weapon in _currentWeapons)
        {
            weapon.Remove();
        }
        _currentWeapons.Clear();

        //_currentWeapons.Remove();
        //_currentWeapons = null;
    }
}
