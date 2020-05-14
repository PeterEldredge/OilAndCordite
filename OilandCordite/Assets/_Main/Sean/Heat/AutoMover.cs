using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Testing script for automatically moving an object at a constant speed
/// used for testing trail renderers 
public class AutoMover : MonoBehaviour
{
    private float _moveSpeed = 120.0f;
    void Update()
    {
        this.transform.position += new Vector3(0.0f, 0.0f, _moveSpeed * Time.deltaTime);
    }
}
