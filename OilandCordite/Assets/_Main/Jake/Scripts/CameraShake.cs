using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraShake : GameEventUserObject
{
    [SerializeField] float _duration = 1f;
    [SerializeField] float _magnitude = 1f;

    private void OnEnemyDefeated(Events.PlayerDefeatedEnemyEventArgs args) => ScreenShake();
    private void OnObstacleHit(Events.ObstacleHitEventArgs args) => ScreenShake();
    private FlightCam _flightCam;

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.PlayerDefeatedEnemyEventArgs>(this, OnEnemyDefeated);
        EventManager.Instance.AddListener<Events.ObstacleHitEventArgs>(this, OnObstacleHit);
    }

    private void ScreenShake()
    {
        transform.gameObject.GetComponent<Camera>().DOShakePosition(1f);
    }
}
