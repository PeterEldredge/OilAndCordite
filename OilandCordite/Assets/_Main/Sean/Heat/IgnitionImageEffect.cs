using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class IgnitionImageEffect : MonoBehaviour
{
    public Material material;

    void OnRenderImage (RenderTexture source, RenderTexture destination) 
    {
        Graphics.Blit(source, destination, material);
    }
}
