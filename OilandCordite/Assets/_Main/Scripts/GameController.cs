using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : GameEventUserObject
{
    private void StartImpactTimeRoutine(Events.PlayerDefeatedEnemyEventArgs args) => StartCoroutine(ImpactTimeRoutine()); 

    private IEnumerator ImpactTimeRoutine()
    {
        Time.timeScale = .5f;

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
