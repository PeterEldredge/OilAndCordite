using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    static public float health = 100;
    static public float heat = 0;
    static public bool invincible = false;
    static public int Speed = 0;

    private void Start()
    {
        health = 100;
        heat = 0;
    }

    public static void setHealth(float newHealth)
    {
        health = Mathf.Clamp(newHealth, 0, 100);
    }
    public static void changeHealth(float change)
    {
        health = Mathf.Clamp(health + change, 0, 100);
    }
    public static void setHeat(float newHeat)
    {
        heat = Mathf.Clamp(newHeat, 0, 99.9f);
    }
    public static void changeHeat(float change)
    {
        heat = Mathf.Clamp(heat + change, 0, 99.9f);
    }
    
}
