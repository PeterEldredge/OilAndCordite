using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RumbleSystem : GameEventUserObject
{
    private bool _paused = false;
    private float _rumbleLevel = 0f;
    [SerializeField] private float _speedThreshold = 300;

    private void OnGamePaused(Events.GamePausedEventArgs args) => _paused = true;
    private void OnGameUnpaused(Events.GameUnpausedEventArgs args) => _paused = false;

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.GamePausedEventArgs>(this, OnGamePaused);
        EventManager.Instance.AddListener<Events.GameUnpausedEventArgs>(this, OnGameUnpaused);
    }

    public override void Unsubscribe()
    {
        EventManager.Instance.RemoveListener<Events.GamePausedEventArgs>(this, OnGamePaused);
        EventManager.Instance.RemoveListener<Events.GameUnpausedEventArgs>(this, OnGameUnpaused);
    }

    void Update()
    {
        _rumbleLevel = _paused ? 0 : (PlayerData.Instance.Speed - _speedThreshold) / 1000;

        InputHelper.Player.SetVibration(0, _rumbleLevel);
        

    }
}
