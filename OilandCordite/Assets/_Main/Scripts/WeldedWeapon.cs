using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum weldedWeaponTypes { EXAUST, LASER, BLADEWING };

public struct WeldedWeaponPickupArgs : IGameEvent {
    public int weaponPickup { get; }

    public WeldedWeaponPickupArgs(int value)
    {
        this.weaponPickup = value;
    }
}
public struct useWeldedWeaponArgs : IGameEvent{}
public struct removeWeldedWeaponArgs : IGameEvent { }

public class WeldedWeapon : GameEventUserObject
{
    [SerializeField]private GameObject[] _weldedWeapons;
    private void AttachWeldedWeapon(WeldedWeaponPickupArgs args) => AttachWeldedWeapon(args.weaponPickup);
    private void AttachWeldedWeapon(PlayerDefeatedEnemyEventArgs args) => AttachWeldedWeapon(0);
    public override void Subscribe()
    {
        EventManager.Instance.AddListener<PlayerDefeatedEnemyEventArgs>(this, AttachWeldedWeapon);
    }
    private void AttachWeldedWeapon(int value)
    {
        GameObject obj =Instantiate(_weldedWeapons[value],gameObject.transform,false);
    }
}
