using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : GameEventUserObject
{
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _invincibilityWindow = 1f;

    [SerializeField] private float _overheatDamage = 5f;
    [SerializeField] private float _overheatDamageRate = 1f;

    //Public
    public float Health { get; private set; }

    //Private
    private HeatSystem _heatSystem;

    private bool _invincible  = false;

    private void Awake()
    {
        Health = _maxHealth;

        _heatSystem = GetComponent<HeatSystem>();
    }

    private void OnOverheat(OverheatedEventArgs args) => StartCoroutine(OverheatRoutine());
    private void OnAttacked(PlayerAttackedEventArgs args) => TakeDamage(args.Damage);

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<OverheatedEventArgs>(this, OnOverheat);
        EventManager.Instance.AddListener<PlayerAttackedEventArgs>(this, OnAttacked);
    }

    private void AddHealth(float amount)
    {
        Health = Mathf.Min(_maxHealth, Health + amount);
    }

    private void TakeDamage(float amount, bool ignoreInvincibility = false)
    {
        if (_invincible && !ignoreInvincibility) return;

        Health -= amount;

        //Trigger PlayerDestroyedEvent
        if (Health <= 0) UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        else
        {
            if(!ignoreInvincibility) StartCoroutine(InvincibilityRoutine());
        }
    }

    private IEnumerator InvincibilityRoutine()
    {
        _invincible = true;

        yield return new WaitForSeconds(_invincibilityWindow);

        _invincible = false;
    }

    private IEnumerator OverheatRoutine()
    {
        while(_heatSystem.OverHeated)
        {
            TakeDamage(_overheatDamage, true);

            yield return new WaitForSeconds(_overheatDamageRate);
        }
    }

}
