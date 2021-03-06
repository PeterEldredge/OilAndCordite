﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeldedWeaponType { LASER, BLADEWING };
public enum Position { LEFT, RIGHT, TOP};

public struct Slot{
    public Transform localPosition;
    public bool available;
    public WeldedWeapon weapon;

    public Slot(Transform pos)
    {
        localPosition = pos;
        available = true;
        weapon = null;
    }

    public void AddWeapon(WeldedWeapon wep)
    {
        weapon = wep;
    }

    public void SetAvailability(bool availability)
    {
        available = availability;
    }

    public void Clear()
    {
        available = true;
        weapon.Remove();
    }
}

namespace Events
{
    public struct WeldedWeaponPickupArgs : IGameEvent
    {
        public WeldedWeaponType WeaponPickup { get; }

        public WeldedWeaponPickupArgs(WeldedWeaponType type)
        {
            WeaponPickup = type;
        }
    }

    public struct UseWeldedWeaponArgs : IGameEvent { }
    public struct RemoveWeldedWeaponArgs : IGameEvent { }
}

public class WeldedWeaponSystem : GameEventUserObject
{
    [SerializeField] private List<WeldedWeapon> _weldedWeaponList;
    [SerializeField] private Transform _leftSlotTransform;
    [SerializeField] private Transform _rightSlotTransform;
    [SerializeField] private Transform _topSlotTransform;

    private Dictionary<WeldedWeaponType, WeldedWeapon> _weldedWeapons;
    private Dictionary<Position, Slot> _slots;

    private void AttachWeldedWeapon(Events.WeldedWeaponPickupArgs args) => AttachWeldedWeapon(args.WeaponPickup);

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.WeldedWeaponPickupArgs>(this, AttachWeldedWeapon);
        //EventManager.Instance.AddListener<Events.PlayerDefeatedEnemyEventArgs>(this, AttachWeldedWeapon);
    }

    public override void Unsubscribe()
    {
        EventManager.Instance.RemoveListener<Events.WeldedWeaponPickupArgs>(this, AttachWeldedWeapon);
        //EventManager.Instance.RemoveListener<Events.PlayerDefeatedEnemyEventArgs>(this, AttachWeldedWeapon);
    }

    private void Start()
    {
        foreach (WeldedWeapon weapon in _weldedWeaponList)
        {
            _weldedWeapons.Add(weapon.weaponType, weapon);
        }

        _slots.Add(Position.LEFT, new Slot(_leftSlotTransform));
        _slots.Add(Position.RIGHT, new Slot(_rightSlotTransform));
        _slots.Add(Position.TOP, new Slot(_topSlotTransform));
    }

    private void Update()
    {
        if (InputHelper.Player.GetButtonDown("WeldedWeaponUse"))
        {
            UseWeldedWeapon();
        }
    }

    private void AttachWeldedWeapon(WeldedWeaponType type)
    {
        //GameObject obj = Instantiate(_weldedWeapons[type], gameObject.transform, false);
        if (_slots[Position.LEFT].available)
        {
            Attach(_slots[Position.LEFT], type);
        }
        else if(_slots[Position.RIGHT].available)
        {
            Attach(_slots[Position.RIGHT], type);
        }
        else if (_slots[Position.TOP].available)
        {
            Attach(_slots[Position.TOP], type);
        }
        else
        {
            _slots[Position.LEFT].Clear();
            Attach(_slots[Position.LEFT], type);
        }
    }

    private void Attach(Slot slot, WeldedWeaponType type)
    {
        slot.AddWeapon(_weldedWeapons[type]);
        slot.weapon.Create(slot.localPosition);
        slot.SetAvailability(false);
    }

    private void UseWeldedWeapon()
    {
        if (!_slots[Position.LEFT].available)
        {
            _slots[Position.LEFT].weapon.Use();
        }
        else if (!_slots[Position.RIGHT].available)
        {
            _slots[Position.RIGHT].weapon.Use();
        }
        else if (!_slots[Position.TOP].available)
        {
            _slots[Position.TOP].weapon.Use();
        }
        else
        {
            return;
        }
    }
}
