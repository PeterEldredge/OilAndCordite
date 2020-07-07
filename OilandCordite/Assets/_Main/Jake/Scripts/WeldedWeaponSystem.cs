using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeldedWeaponSystem : GameEventUserObject
{
    [SerializeField] private List<Transform> _creationPoints;
    
    private WeldedWeapon _currentWeapon;

    private void AttachWeldedWeapon(Events.PlayerGetWeaponEventArgs args) => AttachWeapon(args.Weapon);
    private void RemoveWeldedWeapon(Events.PlayerUseWeaponEventArgs args) => args.Weapon.Remove();

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.PlayerGetWeaponEventArgs>(this, AttachWeldedWeapon);
    }

    public override void Unsubscribe()
    {
        EventManager.Instance.RemoveListener<Events.PlayerGetWeaponEventArgs>(this, AttachWeldedWeapon);
    }

    private void Update()
    {
        if (_currentWeapon != null && InputHelper.Player.GetButtonDown("WeldedWeaponUse"))
        {
            _currentWeapon.Use();
        }
    }

    private void AttachWeapon( WeldedWeapon weapon )
    {
        if (_currentWeapon != null)
        {
            RemoveWeapon(weapon);
        }

        weapon.Create(_creationPoints);
        _currentWeapon = weapon;
    }

    private void RemoveWeapon( WeldedWeapon weapon )
    {
        _currentWeapon = null;
    }
}
