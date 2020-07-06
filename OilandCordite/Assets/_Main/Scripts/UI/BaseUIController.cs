using System.Collections;
using UnityEngine;
using System;

namespace Events
{
    public struct GamePausedEventArgs : IGameEvent { }
    public struct GameUnpausedEventArgs : IGameEvent { }
    public struct UIInteractionEventArgs : IGameEvent { }
}

public abstract class BaseUIController : GameEventUserObject
{
    #region Events

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.PlayerDeathEventArgs>(this, OnPlayerDeath);
        EventManager.Instance.AddListener<Events.MissionCompleteEventArgs>(this, OnMissionCompleted);
        EventManager.Instance.AddListener<Events.PlayerDefeatedEnemyEventArgs>(this, OnPlayerDefeatedEnemy);
        EventManager.Instance.AddListener<Events.GamePausedEventArgs>(this, OnGamePaused);
        EventManager.Instance.AddListener<Events.GameUnpausedEventArgs>(this, OnGameUnpaused);
        EventManager.Instance.AddListener<Events.UIInteractionEventArgs>(this, OnUIInteraction);
    }

    public override void Unsubscribe()
    {
        EventManager.Instance.RemoveListener<Events.PlayerDeathEventArgs>(this, OnPlayerDeath);
        EventManager.Instance.RemoveListener<Events.MissionCompleteEventArgs>(this, OnMissionCompleted);
        EventManager.Instance.RemoveListener<Events.PlayerDefeatedEnemyEventArgs>(this, OnPlayerDefeatedEnemy);
        EventManager.Instance.RemoveListener<Events.GamePausedEventArgs>(this, OnGamePaused);
        EventManager.Instance.RemoveListener<Events.GameUnpausedEventArgs>(this, OnGameUnpaused);
        EventManager.Instance.RemoveListener<Events.UIInteractionEventArgs>(this, OnUIInteraction);
    }

    protected virtual void OnPlayerDeath(Events.PlayerDeathEventArgs args) { }
    protected virtual void OnMissionCompleted(Events.MissionCompleteEventArgs args) { }
    protected virtual void OnPlayerDefeatedEnemy(Events.PlayerDefeatedEnemyEventArgs args) { }
    protected virtual void OnGamePaused(Events.GamePausedEventArgs args) { }
    protected virtual void OnGameUnpaused(Events.GameUnpausedEventArgs args) { }
    protected virtual void OnUIInteraction(Events.UIInteractionEventArgs args) { }

    #endregion

    #region Helpers

    protected IEnumerator ActionOnDelay(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);

        action.Invoke();
    }

    #endregion
}
