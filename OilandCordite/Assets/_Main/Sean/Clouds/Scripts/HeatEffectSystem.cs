using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Heat Effect manager used primarily for testing
/// Cross thresholds, update effects and shaders, etc
public class HeatEffectSystem : MonoBehaviour
{
    [SerializeField] private Material PlayerShipMaterial;
    [SerializeField] private float upperHeatThreshold;
    [SerializeField] private float minimumHeatThreshold;
    [SerializeField] private List<TrailRenderer> HeatTrailObjects;
    [SerializeField] private List<TrailRenderer> ThrustTrailObjects;
    [SerializeField] private Gradient ThrustHeatGradient;

    private HeatSystem _hs;

    void Start()
    {
        _hs = this.GetComponent<HeatSystem>();
    }

    private void UpdateHeat() 
    {
        float heat = _hs != null ? ((_hs.Heat) / 50.0f) : 0.0f;
        this.PlayerShipMaterial.SetFloat("_Temperature", heat);
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

    // TODO sample heat value against a gradient to apply to 
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
