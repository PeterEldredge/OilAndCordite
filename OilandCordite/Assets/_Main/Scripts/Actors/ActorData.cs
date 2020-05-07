using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActorData : MonoBehaviour
{
    public GameObject Object => gameObject;
    public Vector3 WorldSpacePosition => transform.position;
    public Quaternion WorldSpaceRotation => transform.rotation;

}
