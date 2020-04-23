using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightCamBasic : MonoBehaviour
{

    public Transform ship;
    public float springBias = .96f;
    public float distanceFromShip = 30f;
    public float upFromShip = 15f;
    public float lookingPointFromShip = 15f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveCamTo = ship.position - ship.forward * distanceFromShip + Vector3.up * upFromShip;
        transform.position = transform.position * springBias + moveCamTo * (1f - springBias);

        transform.LookAt(ship.position + ship.forward * lookingPointFromShip);

    }
}
