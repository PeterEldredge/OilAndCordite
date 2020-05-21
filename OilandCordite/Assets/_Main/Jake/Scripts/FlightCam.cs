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

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 moveCamTo = ship.position - ship.forward * distanceFromShip + Vector3.up * upFromShip;
        transform.position = transform.position * springBias + moveCamTo * (1f - springBias);

        var newRotation = Quaternion.LookRotation((ship.position + (Quaternion.Euler(new Vector3(-InputHelper.Player.GetAxis("Camera Pan Vertical") * 45, InputHelper.Player.GetAxis("Camera Pan Horizontal") * 60, 0)) * ship.forward) * (PlayerData.Instance.Speed/30) * lookingPointFromShip) - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * damping);
        //var newRotation = Quaternion.LookRotation((ship.position + (Quaternion.Euler(new Vector3(-InputHelper.Player.GetAxis("Camera Pan Vertical") * 45, InputHelper.Player.GetAxis("Camera Pan Horizontal") * 60, 0)) * ship.forward) * (PlayerData.Instance.Speed / 30) * lookingPointFromShip) - transform.position );
        //var slerpedRotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * damping);
        //transform.Rotate(new Vector3(slerpedRotation.x, slerpedRotation.y, slerpedRotation.z), Space.Self);
    }
}
