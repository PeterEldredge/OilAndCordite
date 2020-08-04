using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Heat Effect manager used primarily for testing
/// Cross thresholds, update effects and shaders, etc
public class HeatEffectSystem : GameEventUserObject
{
    [SerializeField] private GameObject PlayerShipBody;
    [SerializeField] private float upperHeatThreshold;
    [SerializeField] private float minimumHeatThreshold;
    [SerializeField] private List<TrailRenderer> HeatTrailObjects;
    [SerializeField] private List<TrailRenderer> ThrustTrailObjects;
    [SerializeField] private Gradient ThrustHeatGradient;
    [SerializeField] private float maxThrusterWidthScale;
    [SerializeField] private float minThrusterWidthScale;


    private void OnBeginIgnition(Events.BeginIgniteEventArgs args) => StartCoroutine(BeginIgnition());
    private void OnEndIgnition(Events.EndIgniteEventArgs args) => StartCoroutine(EndIgnition());
    
    private bool isIgniting;
     [SerializeField] private float ignitionTimer = 1.0f;
    private float stepSize = 2.0f;
    private Material _playerShipMaterial;


    private HeatSystem _hs;
    private CameraEffectSystem _cam;

    private void Awake()
    {
        _hs = this.GetComponent<HeatSystem>();
        _cam = Camera.main.GetComponent<CameraEffectSystem>();
        _playerShipMaterial = this.PlayerShipBody.GetComponent<MeshRenderer>().material;

    }

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.BeginIgniteEventArgs>(this, OnBeginIgnition);
        EventManager.Instance.AddListener<Events.EndIgniteEventArgs>(this, OnEndIgnition);
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
       foreach(TrailRenderer trail in ThrustTrailObjects) 
        {
            trail.widthMultiplier = amount;
        } 
    }

    private void UpdateThrustTrails(float heat) 
    {
        foreach(TrailRenderer trail in ThrustTrailObjects) 
        {
            trail.startColor = ThrustHeatGradient.Evaluate(heat / 2);
        }
    }

    private void EnableHeatTrails() 
    {
        foreach(TrailRenderer trail in HeatTrailObjects) 
        {
            trail.enabled = true;
        }
    }

    private void UpdateHeatTrailWidth(float heat) 
    {
        foreach(TrailRenderer trail in HeatTrailObjects) 
        {
            trail.widthMultiplier = heat;
        }
    }

    private void DisableHeatTrails() 
    {
        foreach(TrailRenderer trail in HeatTrailObjects) 
        {
            trail.enabled = false;
            trail.Clear();
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHeat();
    }
}
