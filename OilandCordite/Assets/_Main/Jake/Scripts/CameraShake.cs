using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraShake : GameEventUserObject
{
    private bool _shaking = false;

    private void OnEnemyDefeated(Events.PlayerDefeatedEnemyEventArgs args) => ScreenShake(args.ShakeDuration, args.ShakeMagnitude);
    private void OnObstacleHit(Events.ObstacleHitEventArgs args) => ScreenShake(args.ShakeDuration, args.ShakeMagnitude);
    private void OnGasCloudExplosion(Events.GasExplosionEventArgs args) => ScreenShake(args.ShakeDuration, args.ShakeMagnitude);
    private void OnSmokeUpdraft(Events.SmokeUpdraftEventArgs args) => ScreenShake(args.ShakeDuration, args.ShakeMagnitude);
    private void OnAttacked(Events.PlayerAttackedEventArgs args) => ScreenShake(args.ShakeDuration, args.ShakeMagnitude);

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.PlayerDefeatedEnemyEventArgs>(this, OnEnemyDefeated);
        EventManager.Instance.AddListener<Events.ObstacleHitEventArgs>(this, OnObstacleHit);
        EventManager.Instance.AddListener<Events.GasExplosionEventArgs>(this, OnGasCloudExplosion);
        EventManager.Instance.AddListener<Events.SmokeUpdraftEventArgs>(this, OnSmokeUpdraft);
        EventManager.Instance.AddListener<Events.PlayerAttackedEventArgs>(this, OnAttacked);
    }

    private void ScreenShake(float duration, float magnitude)
    {
        if (!_shaking)
        {
            transform.gameObject.GetComponent<Camera>().DOShakeRotation(duration, magnitude);
            _shaking = true;
            StartCoroutine(ShakeWaitRoutine(duration));
            _shaking = false;
        }
    }

    private IEnumerator ShakeWaitRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
    }
}

