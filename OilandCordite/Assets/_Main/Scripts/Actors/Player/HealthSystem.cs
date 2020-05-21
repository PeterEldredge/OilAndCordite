using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct PlayerDeathEventArgs : IGameEvent { }

public class HealthSystem : GameEventUserObject
{
    [SerializeField]
    private float _maxHealth = 100f;
    [SerializeField]
    private float _invincibilityWindow = 1f;

    [SerializeField]
    private float _overheatDamage = 5f;
    [SerializeField]
    private float _overheatDamageRate = 1f;

    [SerializeField]
    private float _damageOnCollision = 20f;

    //Public
    public float Health { get; private set; }

    //Private
    private HeatSystem _heatSystem;

    private bool _invincible = false;

    private void Awake()
    {
        Health = _maxHealth;

        _heatSystem = GetComponent<HeatSystem>();
    }

    private void OnOverheat(OverheatedEventArgs args) => StartCoroutine(OverheatRoutine());
    private void OnAttacked(PlayerAttackedEventArgs args) => TakeDamage(args.Damage);
    private void OnPlayerDefeatedEnemy(PlayerDefeatedEnemyEvent args) => AddHealth(args.HealthGain);
    private void OnObstacleHit(ObstacleHitEventArgs args) => TakeDamage(_damageOnCollision, true);
    private void OnDeath(PlayerDeathEventArgs args) => StartCoroutine(DeathDelay(1f));

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<OverheatedEventArgs>(this, OnOverheat);
        EventManager.Instance.AddListener<PlayerAttackedEventArgs>(this, OnAttacked);
        EventManager.Instance.AddListener<PlayerDefeatedEnemyEvent>(this, OnPlayerDefeatedEnemy);
        EventManager.Instance.AddListener<ObstacleHitEventArgs>(this, OnObstacleHit);
        EventManager.Instance.AddListener<PlayerDeathEventArgs>(this, OnDeath);
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
        if (Health <= 0)
        {
            EventManager.Instance.TriggerEvent(new PlayerDeathEventArgs());
        }
        else
        {
            if (!ignoreInvincibility) StartCoroutine(InvincibilityRoutine());
        }
    }
    private IEnumerator DeathDelay(float time)
    {
        //Still got to mess around with values but adds a slow down
        while (Time.timeScale > .1f)
        {
            Time.timeScale -= .1f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return new WaitForSecondsRealtime(.5f);
        }
        Time.timeScale = 0f;
        Time.fixedDeltaTime = 0f;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private IEnumerator InvincibilityRoutine()
    {
        _invincible = true;

        yield return new WaitForSeconds(_invincibilityWindow);

        _invincible = false;
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
