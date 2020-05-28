using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeldedLaser : GameEventUserObject
{
    [SerializeField] private GameObject _Laser;
    [SerializeField] private Transform _shotOrigin;
    
    private void UseLaser(Events.UseWeldedWeaponArgs args) => UseLaser();
    private void Destroy(Events.RemoveWeldedWeaponArgs args) => Destroy();

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.UseWeldedWeaponArgs>(this, UseLaser);
        EventManager.Instance.AddListener<Events.RemoveWeldedWeaponArgs>(this, Destroy);
    }

    public void Update()
    {
        if (InputHelper.Player.GetButtonDown("WeldedWeaponUse"))
        {
            UseLaser();
        }
    }

    private void UseLaser()
    {
        Instantiate(_Laser, _shotOrigin.position, _shotOrigin.rotation);
    }

    private void Destroy()
    {
        EventManager.Instance.RemoveListener<Events.UseWeldedWeaponArgs>(this, UseLaser);
        EventManager.Instance.RemoveListener<Events.RemoveWeldedWeaponArgs>(this, Destroy);

        Destroy(gameObject);
    }
}
