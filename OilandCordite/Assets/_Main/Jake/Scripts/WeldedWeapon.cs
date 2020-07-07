using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeldedWeapon : ScriptableObject
{
    public GameObject weaponObject;
    protected List<GameObject> _createdObjects;

    public abstract void Use();
    public abstract void Remove();
    public abstract void Create(List<Transform> transforms);
}
