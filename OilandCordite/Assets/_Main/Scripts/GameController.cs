﻿using System.Collections;
using UnityEngine;

public class GameController : GameEventUserObject
{
    private void StartImpactTimeRoutine(Events.PlayerDefeatedEnemyEventArgs args) => StartCoroutine(ImpactTimeRoutine()); 

    private IEnumerator ImpactTimeRoutine()
    {
        Time.timeScale = 0f;

        yield return null;
        yield return null;

        Time.timeScale = 1f;
    }

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.PlayerDefeatedEnemyEventArgs>(this, StartImpactTimeRoutine);
    }

    public override void Unsubscribe()
    {
        EventManager.Instance.RemoveListener<Events.PlayerDefeatedEnemyEventArgs>(this, StartImpactTimeRoutine);
    }
}
