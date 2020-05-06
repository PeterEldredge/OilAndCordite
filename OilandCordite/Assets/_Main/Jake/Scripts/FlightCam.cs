using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightCam : MonoBehaviour
{

    public Transform ship;
    public float springBias = .96f;
    public float distanceFromShip = 20f;
    public float upFromShip = 15f;
    public float lookingPointFromShip = 15f;
    public float rotateSpeed = 100f;
    public float damping = 1f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        Vector3 moveCamTo = ship.position - ship.forward * distanceFromShip + Vector3.up * upFromShip;
        transform.position = transform.position * springBias + moveCamTo * (1f - springBias);

        var newRotation = Quaternion.LookRotation((ship.position + ship.forward * lookingPointFromShip) - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * damping);
    }
}
