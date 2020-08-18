using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraShake : GameEventUserObject
{

    private void OnEnemyDefeated(Events.PlayerDefeatedEnemyEventArgs args) => ScreenShake(args.ShakeDuration, args.ShakeMagnitude);
    private void OnObstacleHit(Events.ObstacleHitEventArgs args) => ScreenShake(args.ShakeDuration, args.ShakeMagnitude);
    private void OnGasCloudExplosion(Events.GasExplosionEventArgs args) => ScreenShake(args.ShakeDuration, args.ShakeMagnitude);

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.PlayerDefeatedEnemyEventArgs>(this, OnEnemyDefeated);
        EventManager.Instance.AddListener<Events.ObstacleHitEventArgs>(this, OnObstacleHit);
        EventManager.Instance.AddListener<Events.GasExplosionEventArgs>(this, OnGasCloudExplosion);
    }

    private void ScreenShake(float duration, float magnitude)
    {
        transform.gameObject.GetComponent<Camera>().DOShakeRotation(duration, magnitude);
    }
}
