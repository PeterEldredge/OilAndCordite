using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipRotate : MonoBehaviour
{
    [SerializeField] private GameObject ship;

    [Tooltip("Pitch, Yaw, Roll")] public Vector3 turnTorque = new Vector3(60f, 45f, 45f);
    [Tooltip("Multiplier for all forces")] public float rotateMult = 3f;

    [Header("Options")]
    [SerializeField] private bool _startInverted = false;

    [Header("Input")]
    [SerializeField] private bool _mouseMovement = false;
    [SerializeField] [Range(-1f, 1f)] private float _pitch = 0f;
    [SerializeField] [Range(-1f, 1f)] private float _roll = 0f;
    [SerializeField] [Range(-1f, 1f)] private float _turn = 0f;

    [Header("Mouse Input")]
    [SerializeField] private float _deadzone = .1f;
    [SerializeField] private float _xSensitivity = .4f;
    [SerializeField] private float _ySensitivity = .6f;

    //Private
    private Action _inputCalculation;

    private int _invertYControl;

    private float _xMousePosition;
    private float _yMousePosition;

    #region Input Calculations 

    private void SetInputType()
    {
        ResetVirtualJoystick();

        if (_mouseMovement) _inputCalculation = MouseCalculation;
        else _inputCalculation = KeyboardCalculation;
    }

    private void ResetVirtualJoystick()
    {
        _xMousePosition = 0;
        _yMousePosition = 0;
    }

    private void MouseCalculation()
    {
        _xMousePosition += Input.GetAxisRaw("Mouse X") / 100f * _xSensitivity;
        _yMousePosition += Input.GetAxisRaw("Mouse Y") / 100f * _ySensitivity * _invertYControl;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetVirtualJoystick();
        }

        if (Mathf.Abs(_xMousePosition) > _deadzone) _roll = (Mathf.Abs(_xMousePosition) - _deadzone) * Mathf.Sign(_xMousePosition);
        else _roll = 0;

        if (Mathf.Abs(_yMousePosition) > _deadzone) _pitch = (Mathf.Abs(_yMousePosition) - _deadzone) * Mathf.Sign(_yMousePosition);
        else _pitch = 0;
    }

    private void KeyboardCalculation()
    {
        _pitch = InputHelper.Player.GetAxis("Pitch") * _invertYControl;
        _roll = InputHelper.Player.GetAxis("Roll Right") + InputHelper.Player.GetAxis("Roll Left");
        _turn = InputHelper.Player.GetAxis("Turn");
    }

    #endregion

    private void Start()
    {
        _invertYControl = _startInverted ? 1 : -1;

        SetInputType();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _invertYControl *= -1;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            _mouseMovement = !_mouseMovement;

            SetInputType();
        }

        _inputCalculation.Invoke();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ship.transform.rotation = ship.transform.rotation * transform.rotation * Quaternion.Euler(new Vector3(turnTorque.x * _pitch, turnTorque.y * _turn, -turnTorque.z * _roll) * rotateMult * Time.fixedDeltaTime);
    }
}
