using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class CloudsController : MonoBehaviour
{
    [Header("General Shader Attributes")]
    public Shader shader;
    public Transform cloudContainer;
    private Material material;

    [Header("Weather and Cloud Formation Maps")]
    public Texture3D noiseMap;
    public Texture3D detailMap;
    public Texture2D weatherMap;
    public Texture2D OffsetNoiseMap;

    public float windSpeed;
    private float internalWindSpeed = 0.0f;

    [Header("Cloud Shape Settings")]
    public float cloudScale = 1;

    [Range(0, 1.5f)]
    public float cloudDensity = 1;

    [Range(0, 1.5f)]
    public float detailNoiseWeight;
    
    [Range(0, 100)]
    public float densityOffset;

    private Vector4 noiseWeight = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
    private Vector4 decompositionWeight = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
 
    [Header("Rendering / Optimization Settings")]
    
    [Range(0, 100)]
    public int raySteps = 1;

    [Range(0,20)]
    public int lightSteps = 1;

    [Header("Cloud Lighting Settings")]

    [Range(-0.5f, 1.5f)]
    public float lightAbsorptionThroughCloud;

    [Range(0.0f, 2.0f)]
    public float darknessFactor;

    [Range(0.0f, 1.0f)]
    public float equatorIntensity = 1.0f;

    [Range(0.0f, 1.0f)]
    public float skyIntensity = 1.0f;

    [Header("Cloud Fade Effect Settings")]
    public float maxCameraDistance;
    public float minCameraDistance;

    private float currentDistanceToClouds;
    private CloudFadeImageEffect _cloudFade;
    private Transform _main;

    [ImageEffectOpaque]
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if(material == null) 
        {
           material = new Material(shader);
        }

        if (cloudContainer == null)
            return;

        material.SetVector("_VolumeBoundaryMin", cloudContainer.position - cloudContainer.localScale / 2);
        material.SetVector("_VolumeBoundaryMax", cloudContainer.position + cloudContainer.localScale / 2);
        material.SetInt("_RaymarchSteps", raySteps);
        material.SetTexture("_NoiseTexture", noiseMap);
        material.SetTexture("_DecompositionTexture", detailMap);
        material.SetFloat("_DensityOffset", densityOffset);
        material.SetTexture("_WeatherMap", weatherMap);
        material.SetFloat("_InternalNoiseSpeed", internalWindSpeed);
        material.SetFloat("_WindSpeed", windSpeed);
        material.SetTexture("_OffsetNoise", OffsetNoiseMap);
        material.SetFloat("_CloudLightAbsorptionFactor", lightAbsorptionThroughCloud);
        material.SetFloat("_CloudScale", cloudScale);
        material.SetFloat("_DarknessThreshold", darknessFactor);
        material.SetVector("noiseWeight", noiseWeight);
        material.SetVector("decompositionWeight", decompositionWeight);
        material.SetFloat("_AmbientSky", RenderSettings.ambientIntensity);
        material.SetFloat("detailNoiseWeight", detailNoiseWeight);
        material.SetFloat("_DensityMultiplier", cloudDensity);
        material.SetInt("_lightMarchSteps", lightSteps);
        material.SetFloat("_SkyIntensity", skyIntensity);
        material.SetFloat("_EquatorIntensity", equatorIntensity);

        Graphics.Blit(src, dest, material);
    }

    private void Start()
    {
        _cloudFade = this.GetComponent<CloudFadeImageEffect>();
        _main = this.transform;
    }

    private void Update() 
    {
        if(Application.isPlaying && _cloudFade != null) 
        {
            float distanceToCloudVolume = Vector3.Distance(_main.position, cloudContainer.position);
            
            // Uncomment to help assist with determining distance to volume
            // Debug.Log(distanceToCloudVolume);

            if (distanceToCloudVolume < maxCameraDistance && distanceToCloudVolume > minCameraDistance)
            {
                float normalizedDistance = (distanceToCloudVolume - minCameraDistance) / (maxCameraDistance - minCameraDistance);
                Debug.Log(normalizedDistance);
                _cloudFade.UpdateCloudFade(normalizedDistance);
            }
            else 
            {
                _cloudFade.ClearCloudFade();
            }
        }
    }
}
