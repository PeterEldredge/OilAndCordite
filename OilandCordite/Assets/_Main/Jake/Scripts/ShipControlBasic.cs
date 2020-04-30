using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControlBasic : MonoBehaviour
{
    [Header("Physics")]
    [Tooltip("Force to push plane forwards with")] public float igniteThrust = 200f;
    [Tooltip("Pitch, Yaw, Roll")] public Vector3 turnTorque = new Vector3(60f, 25f, 45f);
    [Tooltip("Multiplier for all forces")] public float rotateMult = 3f;
    [Tooltip("Increase gravity")] public float gravMult = 3.0f;
    [SerializeField] private float _maxAcceleration = 150;
    [SerializeField] private float _minAcceleration = -125;
    [SerializeField] private AnimationCurve _positiveAccelerationCurve;
    [SerializeField] private AnimationCurve _negativeAccelerationCurve;

    [Header("Options")]
    [Tooltip("Invert the pitching of the ship to avoid \"normal\" plane controls")] public bool invertY = false;

    [Header("Input")]
    [SerializeField] private bool _startingInversion = false;
    [SerializeField] private bool _mouseMovement = false;
    [SerializeField] [Range(-1f, 1f)] private float _pitch = 0f;
    [SerializeField] [Range(-1f, 1f)] private float _roll = 0f;

    [Header("Mouse Input")]
    [SerializeField] private float _deadzone = .1f;
    [SerializeField] private float _xSensitivity = .4f;
    [SerializeField] private float _ySensitivity = .6f;

    //States
    private Rigidbody _rb;

    private Action _inputCalculation;

    private bool _igniting = false;
    private bool _overheating = false;

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
        _xMousePosition += Input.GetAxisRaw("Mouse X") * Time.deltaTime * _xSensitivity;
        _yMousePosition += Input.GetAxisRaw("Mouse Y") * Time.deltaTime * _ySensitivity * _invertYControl;

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
        _pitch = Input.GetAxis("Vertical") * _invertYControl;
        _roll = Input.GetAxis("Horizontal");
    }

    #endregion

    void Start()
    {
        _rb = GetComponent<Rigidbody>();

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        _invertYControl = _startingInversion ? 1 : -1;

        SetInputType();
    }

    private void Update()
    {
        _igniting = Input.GetMouseButton(0);

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

    private void FixedUpdate()
    {
        transform.Rotate(new Vector3(turnTorque.x * _pitch, 0, -turnTorque.z * _roll) * rotateMult * Time.fixedDeltaTime, Space.Self);

        _rb.velocity = Mathf.Clamp(_rb.velocity.magnitude, 60, 300) * transform.forward;

        float forwardAngle = transform.forward.y;
        AnimationCurve accelerationCurve;
        float acceleration;

        if (forwardAngle < 0)
        {
            accelerationCurve = _positiveAccelerationCurve;
            forwardAngle *= -1;

            acceleration = (accelerationCurve.Evaluate(forwardAngle) * _maxAcceleration);
        }
        else
        {
            accelerationCurve = _negativeAccelerationCurve;

            acceleration = (accelerationCurve.Evaluate(forwardAngle) * _minAcceleration);
        }

        _rb.velocity += transform.forward * acceleration * Time.fixedDeltaTime;

        PlayerStats.Speed = (int)_rb.velocity.magnitude;

        if (transform.position.y <= 0)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (_igniting)
        {
            if (other.tag == "Smog")
            {
                _rb.velocity += transform.forward * igniteThrust * ((PlayerStats.heat / 100) + 20) * Time.fixedDeltaTime;
            }
        }

        if (other.tag == "Attack")
        {
            TakeDamage();
        }
    }

    void OnCollisionStay(Collision collision)
    {
        TakeDamage();
    }

    IEnumerator invincibleTimer()
    {
        PlayerStats.invincible = true;
        yield return new WaitForSecondsRealtime(1f);
        PlayerStats.invincible = false;
    }

    public void TakeDamage()
    {
        if (!PlayerStats.invincible)
        {
            PlayerStats.changeHealth(-10);
            if (PlayerStats.health == 0)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
            StartCoroutine(invincibleTimer());
        }
    }

}
