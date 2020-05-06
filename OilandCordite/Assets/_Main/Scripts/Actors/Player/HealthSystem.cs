using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _invincibilityWindow = 1f;

    //Public
    public float Health { get; private set; }

    //Private
    private bool _invincible  = false;

    private void Awake()
    {
        Health = _maxHealth;
    }

    private void AddHealth(float amount)
    {
        Health = Mathf.Min(_maxHealth, Health + amount);
    }

    private void TakeDamage(float amount)
    {
        if (_invincible) return;

        Health -= amount;

        //Trigger PlayerDestroyedEvent
        if (Health < 0) UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        else StartCoroutine(InvincibilityRoutine());
    }

    private IEnumerator InvincibilityRoutine()
    {
        _invincible = true;

        yield return new WaitForSeconds(_invincibilityWindow);

        _invincible = false;
    }

}
