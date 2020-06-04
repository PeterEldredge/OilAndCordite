using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeldedWeapon : ScriptableObject
{
    public GameObject weaponObject;
    public WeldedWeaponType weaponType;

    public abstract void Use();
    public abstract void Remove();
    public abstract void Create(Transform transform);
}
