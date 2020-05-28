using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RumbleSystem : GameEventUserObject
{
    [SerializeField] private AnimationCurve _heatCurve;
    
    [SerializeField, Range(0, 1)] private float _explosionLevel;
    [SerializeField] private float _explosionRumbleTime;
    
    [SerializeField, Range(0, 1)] private float _attackedLevel;
    [SerializeField] private float _attackedRumbleTime;

    [SerializeField, Range(0, 1)] private float _obstacleHitLevel;
    [SerializeField] private float _obstacleHitTime;

    private bool _paused = false;
    private float _rumbleLevel = 0f;

    private void OnGamePaused(Events.GamePausedEventArgs args) => _paused = true;
    private void OnGameUnpaused(Events.GameUnpausedEventArgs args) => _paused = false;
    private void OnPlayerDefeatedEnemy(PlayerDefeatedEnemyEventArgs args) => InputHelper.Player.SetVibration(0, _explosionLevel, _explosionRumbleTime);
    private void OnPlayerDeath(PlayerDeathEventArgs args) => InputHelper.Player.SetVibration(0, _explosionLevel, _explosionRumbleTime);
    private void OnPlayerAttacked(PlayerAttackedEventArgs args) => InputHelper.Player.SetVibration(0, _attackedLevel, _attackedRumbleTime);
    private void OnObstacleHit(ObstacleHitEventArgs args) => InputHelper.Player.SetVibration(0, _obstacleHitLevel, _obstacleHitTime);

    protected override void OnEnable()
    {
        base.OnEnable();

        StartCoroutine(RumbleRoutine());
    }

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.GamePausedEventArgs>(this, OnGamePaused);
        EventManager.Instance.AddListener<Events.GameUnpausedEventArgs>(this, OnGameUnpaused);
        EventManager.Instance.AddListener<PlayerDefeatedEnemyEventArgs>(this, OnPlayerDefeatedEnemy);
        EventManager.Instance.AddListener<PlayerDeathEventArgs>(this, OnPlayerDeath);
        EventManager.Instance.AddListener<PlayerAttackedEventArgs>(this, OnPlayerAttacked);
        EventManager.Instance.AddListener<ObstacleHitEventArgs>(this, OnObstacleHit);
    }

    public override void Unsubscribe()
    {
        EventManager.Instance.RemoveListener<Events.GamePausedEventArgs>(this, OnGamePaused);
        EventManager.Instance.RemoveListener<Events.GameUnpausedEventArgs>(this, OnGameUnpaused);
        EventManager.Instance.RemoveListener<PlayerDefeatedEnemyEventArgs>(this, OnPlayerDefeatedEnemy);
        EventManager.Instance.RemoveListener<PlayerDeathEventArgs>(this, OnPlayerDeath);
        EventManager.Instance.RemoveListener<PlayerAttackedEventArgs>(this, OnPlayerAttacked);
        EventManager.Instance.RemoveListener<ObstacleHitEventArgs>(this, OnObstacleHit);
    }

    private IEnumerator RumbleRoutine()
    {
        while(true)
        {
            _rumbleLevel = (_paused || PlayerData.Instance.IsDead) ? 0 : _heatCurve.Evaluate(PlayerData.Instance.Heat / 100);

            InputHelper.Player.SetVibration(1, _rumbleLevel);

            yield return null;
        }
    }
}
