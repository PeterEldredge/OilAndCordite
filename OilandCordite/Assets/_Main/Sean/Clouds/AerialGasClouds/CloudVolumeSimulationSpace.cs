using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudVolumeSimulationSpace : MonoBehaviour
{
    MarchingCubesGPU_4DNoise volume;

    private void Start() 
    {
        volume = this.GetComponent<MarchingCubesGPU_4DNoise>();
    }

    private void OnTriggerEnter(Collider other) 
    {
        Debug.Log(other.tag);
        if(other.tag == "CloudCollider") 
        {
            volume.simulate = true; 
        }

    }   

    private void OnTriggerExit(Collider other) 
    {
        if(other.tag == "CloudCollider") 
        {
            volume.simulate = false; 
        }
    }
}
