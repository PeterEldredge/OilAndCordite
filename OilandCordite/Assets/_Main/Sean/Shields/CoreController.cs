using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreController : MonoBehaviour
{
    [Header("Particle Settings")]
    
    [SerializeField]
    private ParticleSystem warpParticles;

    [SerializeField]
    private ParticleSystem flareParticles;

    [Header("Animation Settings Settings")]
    
    [SerializeField]
    private string chargeAnimName;

    [SerializeField]
    private string fireAnimName;

    [SerializeField]
    private float chargeSpeedModifier = 1.0f;

    [SerializeField]
    private float fireSpeedModifier = 1.0f;

    [Header("Behaviour Settings")]
    
    [SerializeField]
    private bool repeatable;

    [SerializeField]
    private float repeatableInterval;

    private bool isRepeating = true;

    private Animator _anim;

    public void StartLasers()
    {
        StartCoroutine(FireLaserRoutine());
    }

    // Used by animator to stop chargeup particle effects before disabling them
    public void StopEffects()
    {
        flareParticles.Stop();
        warpParticles.Stop();
    }

    public void StopLasers() 
    {
        StopAllCoroutines();
    }

    void Start()
    {
        _anim = this.GetComponent<Animator>();
    }


    private IEnumerator FireLaserRoutine()
    {
        while (isRepeating)
        {
            _anim.speed = chargeSpeedModifier;
            _anim.Play(chargeAnimName);
            yield return new WaitForSeconds(_anim.GetCurrentAnimatorStateInfo(0).length + _anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
            _anim.speed = fireSpeedModifier;
            yield return new WaitForSeconds(_anim.GetCurrentAnimatorStateInfo(0).length + _anim.GetCurrentAnimatorStateInfo(0).normalizedTime);

            if (!repeatable) 
            {
                isRepeating = false;
            }

            yield return new WaitForSeconds(repeatableInterval);
        }
    }
}
