﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeldedWeapon : MonoBehaviour
{
    //public GameObject weaponObject;
    [SerializeField] protected WeldedWeaponType _type;
    //protected List<GameObject> _createdObjects;

    public WeldedWeaponType WeaponType => _type;

    private void OnEnable()
    {
        Clean();
    }

    public abstract void Use();
    public abstract void Remove();
    public abstract void Clean();
    //public abstract void Create(List<Transform> transforms);
}
