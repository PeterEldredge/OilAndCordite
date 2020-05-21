using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MissionController : GameEventUserObject
{
    [SerializeField] protected Level _levelData;

    protected float _timer = 0f;
    public float Timer => _timer;

    protected int _score = 0;
    public int Score => _score;

    protected int _combo = 0;
    public int Combo => _combo;

    protected float _comboTimer = -1f;

    protected bool _missionComplete = false;

    private void AddScore(PlayerDefeatedEnemyEventArgs args)
    {
        _score += args.Score + _combo * Mathf.Clamp(_combo, 0, BaseScoring.MAX_COMBO);
        _combo += 1;

        _comboTimer = BaseScoring.COMBO_TIME;
    }

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<PlayerDefeatedEnemyEventArgs>(this, AddScore);
    }

    public override void Unsubscribe()
    {
        EventManager.Instance.RemoveListener<PlayerDefeatedEnemyEventArgs>(this, AddScore);
    }

    protected void Start()
    {
        StartCoroutine(MissionTimer());
        StartCoroutine(ComboTimer());
    }

    protected void MissionCompelete()
    {
        _missionComplete = true;

        EventManager.Instance.TriggerEvent(new MissionCompleteEventArgs());
    }

    protected int CalculateTimeScore() => Mathf.RoundToInt(BaseScoring.PAR_TIME_SCORE * (_levelData.ParTime / _timer));        

    protected IEnumerator MissionTimer()
    {
        while(!_missionComplete)
        {
            yield return null;

            _timer += Time.deltaTime;
        }
    }

    protected IEnumerator ComboTimer()
    {
        while(true)
        {
            yield return null;

            if(_comboTimer >= 0f)
            {
                _comboTimer -= Time.deltaTime;
            }
            else
            {
                _combo = 0;
            }
        }
    }

}
