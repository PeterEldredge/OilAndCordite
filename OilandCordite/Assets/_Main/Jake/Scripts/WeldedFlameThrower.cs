using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeldedFlameThrower : WeldedWeapon
{
    [SerializeField] private float _timerMax = 6f;
    [SerializeField] private float _timeBonus = 2f;
    [SerializeField] private GameObject _shieldObject;

    public bool HeatShielded => _shielded;

    private float _timer;

    private bool _used = false;
    private bool _shielded = false; 

    private AudioCuePlayer _acp;

    private void Awake()
    {
        _acp = GetComponent<AudioCuePlayer>();
    }

    private void AddTimeToShield(Events.PlayerDefeatedEnemyEventArgs args) => AddTime();

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.PlayerDefeatedEnemyEventArgs>(this, AddTimeToShield);
    }

    public override void Create()
    {
        //Play Animations and Sounds

        Clean();
        gameObject.SetActive(true);
    }

    public override void Use()
    {
        if (!_used)
        {
            StartCoroutine(HeatShield());
            _used = true;
        }
    }

    public override void Remove()
    {
        //Play animation, sound

        gameObject.SetActive(false);
    }

    public override void Clean()
    {
        _shieldObject.SetActive(false);
        _used = false;
        _shielded = false;
        _timer = _timerMax;
    }

    private void AddTime()
    {
        if (_shielded)
        {
            _timer += _timeBonus;
        }
    }

    private IEnumerator HeatShield()
    {
        _shieldObject.SetActive(true);
        _shielded = true;

        while (_timer >= 0)
        {
            _timer -= Time.deltaTime;

            yield return null;
        }

        _shielded = false;
        EventManager.Instance.TriggerEvent(new Events.PlayerRemoveWeaponEventArgs());
    }

}
