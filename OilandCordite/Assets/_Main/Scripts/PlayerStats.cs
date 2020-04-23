using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    static public float health = 100;
    static public float heat = 0;
    static public bool invincible = false;
    public static void setHealth(float newHealth)
    {
        health = newHealth;
    }
    public static void changeHealth(float change)
    {
        health += change;
    }
    public static void setHeat(float newHeat)
    {
        heat = newHeat;
    }
    public static void changeHeat(float change)
    {
        heat += change;
    }
    void OnCollisionStay(Collision collision)
    {
        if (!invincible)
        {
            changeHealth(-10);
            StartCoroutine(invincibleTimer());
        }
    }
    IEnumerator invincibleTimer()
    {
        invincible = true;
        yield return new WaitForSecondsRealtime(.5f);
        invincible = false;
    }
}
