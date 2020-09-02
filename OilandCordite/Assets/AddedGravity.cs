using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddedGravity : MonoBehaviour
{
    [SerializeField] float _gravityMult = 5f;

    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        _rb.AddForce(Physics.gravity * _gravityMult);
    }
}
