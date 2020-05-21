using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : GameEventUserObject
{
    private void StartImpactTimeRoutine(PlayerDefeatedEnemyEventArgs args) => StartCoroutine(ImpactTimeRoutine()); 

    private IEnumerator ImpactTimeRoutine()
    {
        Time.timeScale = .2f;

        yield return new WaitForSecondsRealtime(.04f);

        Time.timeScale = 1f;
    }

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<PlayerDefeatedEnemyEventArgs>(this, StartImpactTimeRoutine);
    }

    public override void Unsubscribe()
    {
        EventManager.Instance.RemoveListener<PlayerDefeatedEnemyEventArgs>(this, StartImpactTimeRoutine);
    }
}
