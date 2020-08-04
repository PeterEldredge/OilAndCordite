using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightCam : MonoBehaviour
{

    [SerializeField] private Transform ship;
    [SerializeField] private float springBias = .96f;
    [SerializeField] private float distanceFromShip = 20f;
    [SerializeField] private float upFromShip = 15f;
    [SerializeField] private float lookingPointFromShip = 15f;
    [SerializeField] private float rotateSpeed = 100f;
    [SerializeField] private float damping = 1f;
    [SerializeField] private float _speedRotationDivisor = 30f;
    [SerializeField] private float _maxDistanceFromShip = 20f;
    [SerializeField] private float _minDistanceFromShip = 5f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        

        Vector3 moveCamTo = ship.position - ship.forward * distanceFromShip + Vector3.up * upFromShip;
        Vector3 newPosition = transform.position * springBias + moveCamTo * (1f - springBias);
        //transform.position = transform.position * springBias + moveCamTo * (1f - springBias);
        var newDistance = Vector3.Distance(PlayerData.Instance.WorldSpacePosition, newPosition);

        if (newDistance <= _maxDistanceFromShip)
        {
            if (newDistance >= _minDistanceFromShip)
            {
                transform.position = newPosition;
            }
            else
            {
                transform.position = ship.position + (transform.position - ship.position).normalized * _minDistanceFromShip;
            }
        }
        else
        {
            transform.position = ship.position + (transform.position - ship.position).normalized * _maxDistanceFromShip;
        }

        var newRotation = Quaternion.LookRotation((ship.position + ship.forward * (PlayerData.Instance.Speed / _speedRotationDivisor) * lookingPointFromShip) - transform.position);
        newRotation = newRotation * Quaternion.Euler(new Vector3(-InputHelper.Player.GetAxis("Camera Pan Vertical") * 45, InputHelper.Player.GetAxis("Camera Pan Horizontal") * 60, 0));
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * damping);
        
    }
}
