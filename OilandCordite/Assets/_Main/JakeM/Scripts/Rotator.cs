using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    [Range(-1, 1)]
    private int direction;

    [SerializeField]
    private Vector3 rotation;


    void Update()
    {
        transform.Rotate(rotation * speed * direction * Time.deltaTime);
    }
}
