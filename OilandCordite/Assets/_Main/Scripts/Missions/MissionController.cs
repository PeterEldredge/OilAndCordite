﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public struct MissionFailedEventArgs : IGameEvent { }
}

public abstract class MissionController : GameEventUserObject
{
    [SerializeField] protected Level _levelData;
    [SerializeField] protected float _missionTimer = 0;

    public float Timer { get; private set; } = 0f;
    public float ComboTimer { get; private set; } = -1f;

    public int Score { get; private set; } = 0;
    public int Combo { get; private set; } = 0;

    public BaseScoring.Rank Rank { get; private set; } = BaseScoring.Rank.None;


    protected bool _missionComplete = false;

    private void AddScore(Events.PlayerDefeatedEnemyEventArgs args)
    {
        Score += args.Score + BaseScoring.COMBO_BONUS * Mathf.Clamp(Combo, 0, BaseScoring.MAX_COMBO);
        Combo += 1;

        ComboTimer = BaseScoring.COMBO_TIME;
    }

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.PlayerDefeatedEnemyEventArgs>(this, AddScore);
    }

    public override void Unsubscribe()
    {
        EventManager.Instance.RemoveListener<Events.PlayerDefeatedEnemyEventArgs>(this, AddScore);
    }

    protected void Start()
    {
        InvokeRepeating("MissionTimer", 0f, 16.66f);

        StartCoroutine(ComboTimerRoutine());
    }

    [ContextMenu("CompleteMission")]
    protected void MissionComplete()
    {
        _missionComplete = true;

        CalculateScore();

        _levelData.HighScore = Score;
        _levelData.BestTime = Mathf.RoundToInt(Timer);

        EventManager.Instance.TriggerEvent(new Events.MissionCompleteEventArgs());
    }

    protected void CalculateScore()
    {
        Score += CalculateTimeScore();
        
        //This sucks dick, refactor later with Rank class
        for(int i = 0; i < Level.NUMBER_OF_RANKS; i++)
        {
            if (_levelData.ScoreRequirements[i] > Score) continue;

            switch(i)
            {
                case 0:
                    Rank = BaseScoring.Rank.Emerald;
                    break;
                case 1:
                    Rank = BaseScoring.Rank.Gold;
                    break;
                case 2:
                    Rank = BaseScoring.Rank.Silver;
                    break;
                case 3:
                    Rank = BaseScoring.Rank.Bronze;
                    break;
            }

            break;
        }
    }

    protected int CalculateTimeScore() => Mathf.RoundToInt(BaseScoring.PAR_TIME_SCORE * (_levelData.ParTime / Timer));  

    protected void MissionTimer()
    {
        if(!_missionComplete)
        {
            Timer += 16.66f;

            if(_missionTimer > 0f)
            {
                _missionTimer -= 16.66f;

                if(_missionTimer <= 0f)
                {
                    EventManager.Instance.TriggerEvent(new Events.MissionFailedEventArgs());
                }
            }
        }
    }

    protected IEnumerator ComboTimerRoutine()
    {
        while(true)
        {
            yield return null;

            if(ComboTimer >= 0f)
            {
                ComboTimer -= Time.deltaTime;
            }
            else
            {
                Combo = 0;
            }
        }
    }

}
