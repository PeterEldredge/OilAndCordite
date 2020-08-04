using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeldedLaser : WeldedWeapon
{
    [SerializeField] private GameObject _projectile;
    [SerializeField] private int shots = 20;
    [SerializeField] private List<Transform> _attackPoints;
    [SerializeField] private float _rangeX = 2f;
    [SerializeField] private float _rangeY = 2f;
    [SerializeField] private float _shotTimer = .2f;

    private List<int> _creationPoints;
    private int _shotsRemaining;
    private bool _used = false;

    private AudioCuePlayer _acp; 

    private void Awake()
    {
        _acp = GetComponent<AudioCuePlayer>();
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
            StartCoroutine(ScatterShots());
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
        _shotsRemaining = shots;
        _used = false;
    }

    private IEnumerator ScatterShots()
    {

        //EventManager.Instance.TriggerEvent(new Events.PlayerUseWeaponEventArgs(_type));
        int count = 0;
        Transform ap;

        while (_shotsRemaining >= 0)
        {
            ap = _attackPoints[count++ % 2];

            Vector3 euler = new Vector3(0, 0, 0);
            euler.x = Random.Range(-_rangeX, _rangeX);
            euler.y = Random.Range(-_rangeY, _rangeY);

            Instantiate(_projectile, ap.position, Quaternion.LookRotation(ap.transform.forward + euler));
            _shotsRemaining--;
            yield return new WaitForSeconds(_shotTimer);
        }

        EventManager.Instance.TriggerEvent(new Events.PlayerRemoveWeaponEventArgs());
    }

}
