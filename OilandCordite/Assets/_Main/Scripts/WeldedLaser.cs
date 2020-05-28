using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeldedLaser : GameEventUserObject
{
    [SerializeField] private GameObject _Laser;
    [SerializeField] private Transform _shotOrigin;
    
    private void useLaser(useWeldedWeaponArgs args) => useLaser();
    private void destroy(removeWeldedWeaponArgs args) => destroy();
    public override void Subscribe()
    {
        EventManager.Instance.AddListener<useWeldedWeaponArgs>(this, useLaser);
        EventManager.Instance.AddListener<removeWeldedWeaponArgs>(this, destroy);
    }
    public void Update()
    {
        if (InputHelper.Player.GetButtonDown("WeldedWeaponUse"))
        {
            useLaser();
        }
    }
    private void useLaser()
    {
        Instantiate(_Laser,_shotOrigin.position,_shotOrigin.rotation);
    }
    private void destroy()
    {
        EventManager.Instance.RemoveListener<useWeldedWeaponArgs>(this, useLaser);
        EventManager.Instance.RemoveListener<removeWeldedWeaponArgs>(this,destroy);
        Destroy(gameObject);
    }
}
