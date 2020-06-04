using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Slot { RIGHT, LEFT, TOP};

public abstract class WeldedWeapon : ScriptableObject
{

    public List<Slot> slots;


    public abstract void Use();
    public abstract void Remove();
}
