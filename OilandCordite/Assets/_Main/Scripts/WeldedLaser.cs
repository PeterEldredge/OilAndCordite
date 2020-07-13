using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeldedLaser : WeldedWeapon
{
    [SerializeField] private GameObject _projectile; 
    [SerializeField] private int shots;
    [SerializeField] private Transform _attackPoint;

    private List<int> _creationPoints;
    private int _shotsRemaining;

    

    private AudioCuePlayer _acp; 

    private void Awake()
    {
        _acp = GetComponent<AudioCuePlayer>();

    }

    public override void Use()
    {
        Instantiate(_projectile, _attackPoint.position, Quaternion.LookRotation(_attackPoint.transform.forward));
        EventManager.Instance.TriggerEvent(new Events.PlayerUseWeaponEventArgs(_type));
        _shotsRemaining--;

        //Play animation, sound

        if(_shotsRemaining == 0)
        {
            EventManager.Instance.TriggerEvent(new Events.PlayerRemoveWeaponEventArgs());
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

    //public override void Create(List<Transform> transforms)
    //{
    //    foreach(int point in _creationPoints)
    //    {
    //        _createdObjects.Add(Instantiate(weaponObject, transforms[point]));
    //    }
    //}
}
