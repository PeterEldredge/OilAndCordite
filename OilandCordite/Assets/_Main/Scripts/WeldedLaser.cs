using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeldedLaser : WeldedWeapon
{
    [SerializeField] private GameObject _projectile; 

    private Transform _shotOrigin;
    private List<int> _creationPoints;

    public override void Use()
    {
        Instantiate(_projectile, _shotOrigin);
        EventManager.Instance.TriggerEvent(new Events.PlayerUseWeaponEventArgs(this));
    }

    public override void Remove()
    {
        //Later, should play destruction animation and sounds
        foreach (GameObject weapon in _createdObjects)
        {
            Destroy(weapon);
        }
    }

    public override void Create(List<Transform> transforms)
    {
        foreach(int point in _creationPoints)
        {
            _createdObjects.Add(Instantiate(weaponObject, transforms[point]));
        }
    }
}
