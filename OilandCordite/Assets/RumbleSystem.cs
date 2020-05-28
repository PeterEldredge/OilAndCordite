using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RumbleSystem : GameEventUserObject
{
    [SerializeField] private AnimationCurve _heatCurve;
    [SerializeField] private float _explosionRumbleTime;

    private bool _paused = false;
    private float _rumbleLevel = 0f;

    private void OnGamePaused(Events.GamePausedEventArgs args) => _paused = true;
    private void OnGameUnpaused(Events.GameUnpausedEventArgs args) => _paused = false;
    private void OnPlayerDefeatedEnemy(PlayerDefeatedEnemyEventArgs args) => InputHelper.Player.SetVibration(0, 1, _explosionRumbleTime);
    private void OnPlayerDeath(PlayerDeathEventArgs args) => InputHelper.Player.SetVibration(0, 1, _explosionRumbleTime);

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
    }

    public override void Unsubscribe()
    {
        EventManager.Instance.RemoveListener<Events.GamePausedEventArgs>(this, OnGamePaused);
        EventManager.Instance.RemoveListener<Events.GameUnpausedEventArgs>(this, OnGameUnpaused);
        EventManager.Instance.RemoveListener<PlayerDefeatedEnemyEventArgs>(this, OnPlayerDefeatedEnemy);
        EventManager.Instance.RemoveListener<PlayerDeathEventArgs>(this, OnPlayerDeath);
    }

    private IEnumerator RumbleRoutine()
    {
        while(true)
        {
            _rumbleLevel = _paused ? 0 : _heatCurve.Evaluate(PlayerData.Instance.Heat / 100);

            InputHelper.Player.SetVibration(1, _rumbleLevel);

            yield return null;
        }
    }
}
