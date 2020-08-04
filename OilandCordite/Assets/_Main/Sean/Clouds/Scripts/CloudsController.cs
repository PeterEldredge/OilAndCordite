using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class CloudsController : MonoBehaviour
{
    public Shader shader;
    public Transform cloudContainer;
    private Material material;
    public Texture3D noiseMap;
    public Texture3D detailMap;
    public Texture2D weatherMap;
    public Texture2D OffsetNoiseMap;
    public Color upperGradient;
    public Color lowerGradient;
    public float internalWindSpeed;
    public float windSpeed;
    [Range(0, 100)]
    public float densityOffset; 
    [Range(0, 100)]
    public int raySteps = 1;

    [Range(0,20)]
    public int lightSteps = 1;

    public float cloudScale = 1;
    public float cloudDensity = 1;

    public float detailNoiseWeight;

    public float lightAbsorptionThroughCloud;
    public float darknessFactor;

    public float equatorIntensity = 1.0f;
    public float skyIntensity = 1.0f;

    public float tintAmount = 1.0f;

    [Header("Cloud Fade Effect Settings")]
    public float maxCameraDistance;
    public float minCameraDistance;

    private float currentDistanceToClouds;
    private CloudFadeImageEffect _cloudFade;
    private Color _skyColor;
    private Color _equatorColor;
    private Transform _main;

    [ImageEffectOpaque]
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if(material == null) 
        {
           material = new Material(shader);
        }
        material.SetVector("_VolumeBoundaryMin", cloudContainer.position - cloudContainer.localScale / 2);
        material.SetVector("_VolumeBoundaryMax", cloudContainer.position + cloudContainer.localScale / 2);
        material.SetInt("_RaymarchSteps", raySteps);
        material.SetTexture("_NoiseTexture", noiseMap);
        material.SetTexture("_DecompositionTexture", detailMap);
        material.SetVector("_CloudPeakColor", upperGradient);
        material.SetVector("_CloudBaseColor", lowerGradient);
        material.SetFloat("_DensityOffset", densityOffset);
        material.SetTexture("_WeatherMap", weatherMap);
        material.SetFloat("_InternalNoiseSpeed", internalWindSpeed);
        material.SetFloat("_WindSpeed", windSpeed);
        material.SetTexture("_OffsetNoise", OffsetNoiseMap);
        material.SetFloat("_CloudLightAbsorptionFactor", lightAbsorptionThroughCloud);
        material.SetFloat("_CloudScale", cloudScale);
        material.SetFloat("_DarknessThreshold", darknessFactor);
        material.SetFloat("_AmbientSky", RenderSettings.ambientIntensity);
        material.SetFloat("detailNoiseWeight", detailNoiseWeight);
        material.SetFloat("_DensityMultiplier", cloudDensity);
        material.SetInt("_lightMarchSteps", lightSteps);
        material.SetFloat("_TintAmount", tintAmount);
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
        if(Application.isPlaying) 
        {
            if (_skyColor == null)
            {
                _skyColor = RenderSettings.ambientSkyColor;
            }

            if (_equatorColor == null)
            {
                _equatorColor = RenderSettings.ambientEquatorColor;
            }

            float distanceToCloudVolume = Vector3.Distance(_main.position, cloudContainer.position);
            Debug.Log(distanceToCloudVolume);

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
