using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class SliceViewer : MonoBehaviour
{
    public Shader sliceShader;
    public Texture3D noise;
    private Material material;
    public int slice;
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if(material == null) 
        {
           material = new Material(sliceShader);
        }
        material.SetTexture("_NoiseTexture", noise);
        material.SetInt("_SliceIndex", slice);
    }
}
