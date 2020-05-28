using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeldedWeaponTypes { EXAUST, LASER, BLADEWING };

namespace Events
{
    public struct WeldedWeaponPickupArgs : IGameEvent
    {
        public WeldedWeaponTypes WeaponPickup { get; }

        public WeldedWeaponPickupArgs(WeldedWeaponTypes type)
        {
            WeaponPickup = type;
        }
    }

    public struct UseWeldedWeaponArgs : IGameEvent { }
    public struct RemoveWeldedWeaponArgs : IGameEvent { }
}

public class WeldedWeaponSystem : GameEventUserObject
{
    [SerializeField] private Dictionary<WeldedWeaponTypes, GameObject> _weldedWeapons;

    private void AttachWeldedWeapon(Events.WeldedWeaponPickupArgs args) => AttachWeldedWeapon(args.WeaponPickup);

    private void AttachWeldedWeapon(Events.PlayerDefeatedEnemyEventArgs args) => AttachWeldedWeapon(0);

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.WeldedWeaponPickupArgs>(this, AttachWeldedWeapon);
        EventManager.Instance.AddListener<Events.PlayerDefeatedEnemyEventArgs>(this, AttachWeldedWeapon);
    }

    public override void Unsubscribe()
    {
        EventManager.Instance.RemoveListener<Events.WeldedWeaponPickupArgs>(this, AttachWeldedWeapon);
        EventManager.Instance.RemoveListener<Events.PlayerDefeatedEnemyEventArgs>(this, AttachWeldedWeapon);
    }

    private void AttachWeldedWeapon(WeldedWeaponTypes type)
    {
        GameObject obj = Instantiate(_weldedWeapons[type], gameObject.transform, false);
    }
}
