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

#endregion

    [SerializeField] private float fovMaxIncrease = 5.0f;
    [SerializeField] private float fovMaxDecrease = 5.0f;
    [SerializeField] private float fovStep = 0.1f;
    [SerializeField] private float _fovSpeed = 10f;

    [SerializeField] private AnimationCurve _speedToFOVCurve;

    private float initialFov;
    private float maxFov;
    private float minFov;
    private Camera _main;

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

    //public void IncreaseCameraFov() 
    //{
    //    float fov = _main.fieldOfView;
    //    _main.fieldOfView = (fov < maxFov) ? (fov += fovStep * Time.deltaTime) : maxFov; 
    //}

    //public void DecreaseCameraFov() 
    //{
    //    float fov = _main.fieldOfView;
    //    _main.fieldOfView = (fov > currentFov) ? (fov -= fovStep * Time.deltaTime) : currentFov; 
    //}

    private void UpdateCameraFOV()
    {
        var currentFOV = _main.fieldOfView;
        var targetFOV = initialFov + _speedToFOVCurve.Evaluate(PlayerData.Instance.Speed / PlayerData.Instance.MaxSpeed) * fovMaxIncrease;
        _main.fieldOfView = Mathf.Lerp(currentFOV, Mathf.Clamp(targetFOV, initialFov, maxFov), _fovSpeed * Time.fixedDeltaTime);
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
        initialFov = _main.fieldOfView;
        maxFov = initialFov + fovMaxIncrease;
        minFov = initialFov - fovMaxDecrease;
        IgniteImageEffectMaterial.SetFloat("_VignetteSoftness", ignitionMinSoftness);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCameraFOV();
    }
}
