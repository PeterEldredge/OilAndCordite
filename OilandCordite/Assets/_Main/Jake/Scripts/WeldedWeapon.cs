using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeldedWeapon : MonoBehaviour
{
    [SerializeField] private WeldedWeaponType _type;

    public WeldedWeaponType WeaponType => _type;

    private void OnEnable()
    {
        Clean();
    }

    public abstract void Create();
    public abstract void Use();
    public abstract void Remove();
    public abstract void Clean();
}
