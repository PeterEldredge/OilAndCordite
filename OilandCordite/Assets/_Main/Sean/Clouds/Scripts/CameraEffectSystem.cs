using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffectSystem : MonoBehaviour
{
    public Material IgniteImageEffectMaterial;
    public CloudFadeImageEffect cloudFadeImageEffect;

#region Ignition Effect Settings

    [SerializeField] private float ignitionMaxSoftness = 0.65f;
    [SerializeField] private float ignitionMinSoftness = 0.32f;
    [SerializeField] private float ignitionStepSize = 0.1f;
    [SerializeField] private Gradient ignitionColorGradient;

    [SerializeField] private float fovMaxIncrease = 5.0f;
    [SerializeField] private float fovMaxDecrease = 5.0f;
    [SerializeField] private float fovStep = 0.1f;

#endregion

#region FOV can Cam Settings

    private float currentFov;
    private float maxFov;
    private float minFov;
    private Camera _main;

#endregion

    public void UpdateIgniteEffectColor() 
    {
        IgniteImageEffectMaterial.SetColor("_VignetteColor", new Color(0.0f,1.0f,0.0f,1.0f));
    }

    public void IncreaseIgnitionSoftness() 
    {
        float softness = IgniteImageEffectMaterial.GetFloat("_VignetteSoftness");
        softness = softness < ignitionMaxSoftness ? (softness += ignitionStepSize * Time.deltaTime) : ignitionMaxSoftness;
        IgniteImageEffectMaterial.SetFloat("_VignetteSoftness", softness);
    }

    public void IncreaseCameraFov() 
    {
        float fov = _main.fieldOfView;
        _main.fieldOfView = (fov < maxFov) ? (fov += fovStep * Time.deltaTime) : maxFov; 
    }

    public void DecreaseCameraFov() 
    {
        float fov = _main.fieldOfView;
        _main.fieldOfView = (fov > currentFov) ? (fov -= fovStep * Time.deltaTime) : currentFov; 
    }

    public void DecreaseIgnitionSoftness() 
    {
        float softness = IgniteImageEffectMaterial.GetFloat("_VignetteSoftness");
        softness = softness > ignitionMinSoftness ? (softness -= ignitionStepSize * Time.deltaTime) : ignitionMinSoftness;
        IgniteImageEffectMaterial.SetFloat("_VignetteSoftness", softness);
    }

    // Samples a color gradient based on ignition softness
    public void UpdateIgitionColor() 
    {
        float softness = IgniteImageEffectMaterial.GetFloat("_VignetteSoftness");
        float samplePos = (softness - ignitionMinSoftness) / (ignitionMaxSoftness - ignitionMinSoftness);
        IgniteImageEffectMaterial.SetColor("_VignetteColor", ignitionColorGradient.Evaluate(samplePos));
    }

    private void Awake()
    {
        _main = this.GetComponent<Camera>();
        currentFov = _main.fieldOfView;
        maxFov = currentFov + fovMaxIncrease;
        minFov = currentFov - fovMaxDecrease;
        IgniteImageEffectMaterial.SetFloat("_VignetteSoftness", ignitionMinSoftness);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
