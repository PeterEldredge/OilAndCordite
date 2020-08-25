using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Events
{
    public struct PlayerDeathEventArgs : IGameEvent { }
    public struct BeginInvincibilityArgs : IGameEvent { }
    public struct EndInvincibilityArgs : IGameEvent { }
}

public class HealthSystem : GameEventUserObject
{
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _invincibilityWindow = 1f;

    [SerializeField] private float _overheatDamage = 5f;
    [SerializeField] private float _overheatDamageRate = 1f;

    [SerializeField] private float _damageOnBump = 20f;
    [SerializeField] private float _damageOnBounce = 40f;

    //Public
    public float Health { get; private set; }
    public bool IsDead { get; private set; }

    //Private
    private HeatSystem _heatSystem;

    private bool _invincible = false;

    private void Awake()
    {
        Health = _maxHealth;
        IsDead = false;

        _heatSystem = GetComponent<HeatSystem>();
    }

    private void OnOverheat(Events.OverheatedEventArgs args) => StartCoroutine(OverheatRoutine());
    private void OnAttacked(Events.PlayerAttackedEventArgs args) => TakeDamage(args.Damage);
    private void OnPlayerDefeatedEnemy(Events.PlayerDefeatedEnemyEventArgs args) => AddHealth(args.HealthGain);
    private void OnObstacleHit(Events.ObstacleHitEventArgs args) => TakeDamage(args.Bounce ? _damageOnBounce : _damageOnBump, true);

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.OverheatedEventArgs>(this, OnOverheat);
        EventManager.Instance.AddListener<Events.PlayerAttackedEventArgs>(this, OnAttacked);
        EventManager.Instance.AddListener<Events.PlayerDefeatedEnemyEventArgs>(this, OnPlayerDefeatedEnemy);
        EventManager.Instance.AddListener<Events.ObstacleHitEventArgs>(this, OnObstacleHit);
    }

    private void AddHealth(float amount)
    {
        Health = Mathf.Min(_maxHealth, Health + amount);
    }

    private void TakeDamage(float amount, bool ignoreInvincibility = false)
    {
        if ((_invincible && !ignoreInvincibility) || IsDead) return;

        Health -= amount;

        if (Health <= 0)
        {
            IsDead = true;
            EventManager.Instance.TriggerEvent(new Events.PlayerDeathEventArgs());
        }
        else
        {
            if (!ignoreInvincibility) StartCoroutine(InvincibilityRoutine());
        }
    }

    private IEnumerator InvincibilityRoutine()
    {
        _invincible = true;

        EventManager.Instance.TriggerEvent(new Events.BeginInvincibilityArgs());

        yield return new WaitForSeconds(_invincibilityWindow);

        _invincible = false;

        EventManager.Instance.TriggerEvent(new Events.EndInvincibilityArgs());
    }

    private IEnumerator OverheatRoutine()
    {
        while (_heatSystem.OverHeated)
        {
            TakeDamage(_overheatDamage, true);

            yield return new WaitForSeconds(_overheatDamageRate);
        }
    }

}
