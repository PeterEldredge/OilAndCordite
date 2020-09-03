using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Heat Effect manager used primarily for testing
/// Cross thresholds, update effects and shaders, etc
public class HeatEffectSystem : GameEventUserObject
{
    [SerializeField] private GameObject playerShipBody;
    [SerializeField] private float upperHeatThreshold;
    [SerializeField] private float minimumHeatThreshold;
    [SerializeField] private List<TrailRenderer> heatTrailObjects;
    [SerializeField] private List<TrailRenderer> thrustTrailObjects;
    [SerializeField] private Gradient thrustHeatGradient;
    [SerializeField] private float maxThrusterWidthScale;
    [SerializeField] private float minThrusterWidthScale;

    [SerializeField] private ParticleSystem _heatTrailParticles;
    [SerializeField] private AnimationCurve _heatCurve;
    [SerializeField] private float _minLifetime, _maxLifetime;
    [SerializeField] private float _minSize, _maxSize;


    private void OnBeginIgnition(Events.BeginIgniteEventArgs args) => StartCoroutine(BeginIgnition());
    private void OnEndIgnition(Events.EndIgniteEventArgs args) => StartCoroutine(EndIgnition());
    private void OnBeginInvincibility(Events.BeginInvincibilityArgs args) => StartCoroutine(BeginInvincibility());
    private void OnEndInvincibility(Events.EndInvincibilityArgs args) => EndInvincibility();
    
    [SerializeField] private float ignitionTimer = 1.0f;

    private float stepSize = 2.0f;
    private bool isIgniting;
    private bool isInvincible;
    private Material _playerShipMaterial;
    private float _currentHeatCurve;
    private float _lifetimeDifference;
    private float _sizeDifference;


    private HeatSystem _hs;
    private CameraEffectSystem _cam;

    private void Awake()
    {
        _hs = this.GetComponent<HeatSystem>();
        _cam = Camera.main.GetComponent<CameraEffectSystem>();
        _playerShipMaterial = this.playerShipBody.GetComponent<MeshRenderer>().material;

        _currentHeatCurve = _maxLifetime;
        _lifetimeDifference = _maxLifetime - _minLifetime;
        _sizeDifference = _maxSize - _minSize;
    }

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.BeginIgniteEventArgs>(this, OnBeginIgnition);
        EventManager.Instance.AddListener<Events.EndIgniteEventArgs>(this, OnEndIgnition);
        EventManager.Instance.AddListener<Events.BeginInvincibilityArgs>(this, OnBeginInvincibility);
        EventManager.Instance.AddListener<Events.EndInvincibilityArgs>(this, OnEndInvincibility);
    }

    private IEnumerator BeginIgnition() 
    {
        isIgniting = true;
        ignitionTimer = 1.0f;
        while(isIgniting) 
        {
            ignitionTimer += Time.deltaTime * stepSize;
            float widthMultiplier = (ignitionTimer < maxThrusterWidthScale) ? ignitionTimer : maxThrusterWidthScale;
            UpdateThrusterWidth(widthMultiplier);
            _cam?.IncreaseIgnitionSoftness();
            _cam?.UpdateIgitionColor();
            //_cam?.IncreaseCameraFov();
            yield return null;
        }        
    }

    private IEnumerator EndIgnition() 
    {
        this.isIgniting = false;
        ignitionTimer = maxThrusterWidthScale;
        while(!isIgniting) 
        {
            ignitionTimer -= Time.deltaTime * stepSize;
            float widthMultiplier = (ignitionTimer > minThrusterWidthScale) ? ignitionTimer : minThrusterWidthScale;
            UpdateThrusterWidth(widthMultiplier);
            _cam?.DecreaseIgnitionSoftness();
            _cam?.UpdateIgitionColor();
            //_cam?.DecreaseCameraFov();
            yield return null;
        }
    }

    private IEnumerator BeginInvincibility() 
    {
        isInvincible = true;
        while (isInvincible)
        {
            this._playerShipMaterial.SetColor("_AdditiveColor", Color.white);
            
            yield return new WaitForSeconds(0.1f);

            this._playerShipMaterial.SetColor("_AdditiveColor", Color.black);

            yield return new WaitForSeconds(0.1f);

        }
    }

    private void EndInvincibility()
    {
        isInvincible = false;
        this._playerShipMaterial.SetColor("_AdditiveColor", Color.black);
    }

    private void UpdateHeat() 
    {
        float heat = _hs != null ? ((_hs.Heat) / 50.0f) : 0.0f;
        this._playerShipMaterial.SetFloat("_Temperature", heat);
        if(heat > minimumHeatThreshold) 
        {
            EnableHeatTrails();
        }
        if(heat < minimumHeatThreshold) 
        {
            DisableHeatTrails();
        }
        UpdateHeatTrailWidth(heat);
        UpdateThrustTrails(heat);
    }

    private void UpdateThrusterWidth(float amount) 
    {
       foreach(TrailRenderer trail in thrustTrailObjects) 
        {
            trail.widthMultiplier = amount;
        } 
    }

    private void UpdateThrustTrails(float heat) 
    {
        foreach(TrailRenderer trail in thrustTrailObjects) 
        {
            trail.startColor = thrustHeatGradient.Evaluate(heat / 2);
        }
    }

    private void EnableHeatTrails() 
    {
        foreach(TrailRenderer trail in heatTrailObjects) 
        {
            trail.enabled = true;
        }

    }

    private void UpdateHeatTrailWidth(float heat) 
    {
        
        foreach (TrailRenderer trail in heatTrailObjects) 
        {
            trail.widthMultiplier = heat;
        }

        _currentHeatCurve = _heatCurve.Evaluate(heat / 2f);

        _heatTrailParticles.startLifetime = _currentHeatCurve * _lifetimeDifference + _minLifetime;
        _heatTrailParticles.startSize = _currentHeatCurve * _sizeDifference + _minSize;
    }

    private void DisableHeatTrails() 
    {
        if (heatTrailObjects != null)
        {
            foreach(TrailRenderer trail in heatTrailObjects) 
            {
                trail.enabled = false;
                trail.Clear();
            }
        }

         
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHeat();
    }
}
