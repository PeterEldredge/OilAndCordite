using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _lookSpeed = 1f;

    // Update is called once per frame
    void FixedUpdate()
    {
        //this.transform.LookAt(_cameraTransform);
    }

    private void Update()
    {
        Quaternion lookRotation = Quaternion.LookRotation(_cameraTransform.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, _lookSpeed * Time.deltaTime);
    }
}
