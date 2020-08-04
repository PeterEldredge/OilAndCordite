using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeldedLaser : WeldedWeapon
{
    [SerializeField] private GameObject _projectile; 
    [SerializeField] private int shots;
    [SerializeField] private List<Transform> _attackPoints;

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
    }

    private IEnumerator ScatterShots()
    {

        //EventManager.Instance.TriggerEvent(new Events.PlayerUseWeaponEventArgs(_type));
        int count = 0;
        Transform ap;

        while (_shotsRemaining >= 0)
        {
            ap = _attackPoints[count++ % 2];
            Instantiate(_projectile, ap.position, Quaternion.LookRotation(ap.transform.forward));
            _shotsRemaining--;
            yield return new WaitForSeconds(.2f);
        }

        EventManager.Instance.TriggerEvent(new Events.PlayerRemoveWeaponEventArgs());
    }

}
