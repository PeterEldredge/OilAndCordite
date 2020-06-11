using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public abstract class CameraEvent : MonoBehaviour
{
    public delegate void CameraEventHandler(Camera camera);
    protected Camera Camera { get; private set; }

    void Start()
    {
        Camera = GetComponent<Camera>();
    }
}

