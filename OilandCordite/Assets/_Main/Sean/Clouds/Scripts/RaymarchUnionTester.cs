using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class RaymarchUnionTester : MonoBehaviour
{
    public Material cloudVolume;

    public Transform frontOfShip;
    public Transform backOfShip;
    // Update is called once per frame
    void Update()
    {
        cloudVolume.SetVector("_IntersectionPosition", this.frontOfShip.position);
        cloudVolume.SetVector("_IntersectionOrientation", this.backOfShip.position);
    }
}
