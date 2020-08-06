using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class CloudFadeImageEffect : MonoBehaviour
{
    [SerializeField] private Shader cloudDistortionShader;

    [Header("Shader Attributes/Varyings")]
    [SerializeField] private Texture2D mainTex;
    [SerializeField] private Texture2D distortionMap;
    [SerializeField] private Texture2D overlayTex;
    [SerializeField] private Color vignetteColor;
    [SerializeField] private float vignetteRadius;
    [SerializeField] private float vignetteSoftness;
    [SerializeField] private float distortionMultiplier;
    [SerializeField] private float overlayMultiplier;

    public Material cloudDistortionMaterial { get; private set; }

    public void UpdateCloudFade(float normDistance)
    {
        float strength = (1.0f - normDistance);
        this.vignetteRadius = 0.35f + (0.7f * normDistance);
        this.overlayMultiplier = 0.0f + (1.3f * strength);
        this.vignetteColor = RenderSettings.ambientEquatorColor;
    }

    public void ClearCloudFade() 
    {
        this.vignetteRadius = 1.0f;
        this.overlayMultiplier = 0.0f;
    }

    void OnRenderImage (RenderTexture source, RenderTexture destination) 
    {
        if (cloudDistortionMaterial == null) 
        {
            cloudDistortionMaterial = new Material(cloudDistortionShader);
        }

        //cloudDistortionMaterial.SetTexture("_MainTex", mainTex);
        cloudDistortionMaterial.SetTexture("_DistortionMap", distortionMap);
        cloudDistortionMaterial.SetTexture("_OverlayTex", overlayTex);
        cloudDistortionMaterial.SetColor("_VignetteColor", vignetteColor);
        cloudDistortionMaterial.SetFloat("_VignetteRadius", vignetteRadius);
        cloudDistortionMaterial.SetFloat("_VignetteSoftness", vignetteSoftness);
        cloudDistortionMaterial.SetFloat("_DistortionMultiplier", distortionMultiplier);
        cloudDistortionMaterial.SetFloat("_OverlayMultiplier", overlayMultiplier);

        Graphics.Blit(source, destination, cloudDistortionMaterial);
    }
}
