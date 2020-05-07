using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugVolume : MonoBehaviour
{
    /* 
    * Disables the mesh renderer used to visualize cloud volume in play mode
    */
    private MeshRenderer _renderer;
    // Start is called before the first frame update
    void Start()
    {
        _renderer = this.GetComponent<MeshRenderer>();
        _renderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
