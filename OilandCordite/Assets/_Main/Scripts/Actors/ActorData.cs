using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActorData : MonoBehaviour
{
    public Vector3 WorldSpacePosition => transform.position;
}
