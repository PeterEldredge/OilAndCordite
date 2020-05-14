using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControlBasic : GameEventUserObject
{
    [Header("Physics")]
    [Tooltip("Force to push plane forwards with")] public float igniteThrust = 1000f;
    [Tooltip("Pitch, Yaw, Roll")] public Vector3 turnTorque = new Vector3(60f, 45f, 90f);
    [Tooltip("Multiplier for all forces")] public float rotateMult = 3f;
    [Tooltip("Increase gravity")] public float gravMult = 3.0f;
    [SerializeField] private float _maxAcceleration = 150f;
    [SerializeField] private float _minAcceleration = -125f;
    [SerializeField] private float _minSmogMomentum = 0f;
    [SerializeField] private float _minAirMomentum = 30f;
    [SerializeField] private float _minSmogIgnitionHeat = 0f;
    [SerializeField] private float _gravityMultiplier = 50f;
    [SerializeField] private float _noGravitySpeed = 200f;
    [Tooltip("When calculating the amount of thrust to receive, Gas Clouds should give a substantial boost even if the player's heat is 0")] [SerializeField] private float _minGasIgnitionHeat = 20f;
    [SerializeField] private AnimationCurve _positiveAccelerationCurve;
    [SerializeField] private AnimationCurve _negativeAccelerationCurve;
    [SerializeField] private AnimationCurve _gravityAccelerationCurve;

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

    [Header("Animation")]
    [SerializeField] private Animator _anim;

    //Public
    public int Speed { get; private set; }

    //Private
    private Rigidbody _rb;

    private Action _inputCalculation;

    private int _invertYControl;

    private float _xMousePosition;
    private float _yMousePosition;

    private bool _spinningOut = false;
    private bool _bouncing = false;

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

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnObstacleHit(ObstacleHitEventArgs args) => StartCoroutine(BounceBackRoutine(args));
    public override void Subscribe()
    {
        EventManager.Instance.AddListener<ObstacleHitEventArgs>(this, OnObstacleHit);
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

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

    private void FixedUpdate()
    {
        transform.Rotate(new Vector3(turnTorque.x * _pitch, turnTorque.y * _turn, -turnTorque.z * _roll) * rotateMult * Time.fixedDeltaTime, Space.Self);

        float minMomentum = _minSmogMomentum;
        if (!PlayerData.Instance.InSmog)
            minMomentum = _minAirMomentum;

        float forwardAngle = transform.forward.y;
        float acceleration;
        AnimationCurve accelerationCurve;

        if (InputHelper.Player.GetButtonDown("Spin Out") && !_spinningOut)
        {
            StartCoroutine(SpinoutRoutine());
        }

        if (!_spinningOut && !_bouncing)
        {
            if (!PlayerData.Instance.InSmog && forwardAngle < 0)
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

            if (PlayerData.Instance.InGas)
            {
                _rb.velocity += transform.forward * igniteThrust * (Mathf.Clamp(PlayerData.Instance.Heat, _minGasIgnitionHeat * (PlayerData.Instance.IsIgniting ? 1 : 0), 100) / 100) * Time.fixedDeltaTime;
            }
            else if (PlayerData.Instance.InSmog)
            {
                _rb.velocity += transform.forward * igniteThrust * .2f * (Mathf.Clamp(PlayerData.Instance.Heat, _minSmogIgnitionHeat, 100) / 100) * Time.fixedDeltaTime;
            }

            if (transform.position.y <= 15)
            {
                transform.position = new Vector3(transform.position.x, 15, transform.position.z);
            }

            float gravity = -_gravityMultiplier * _gravityAccelerationCurve.Evaluate(Mathf.Clamp(_rb.velocity.magnitude/_noGravitySpeed, 0f, 1f)) * Time.fixedDeltaTime;
        
            transform.Translate(new Vector3(0, gravity, 0), Space.World);
            _rb.velocity = Mathf.Clamp(_rb.velocity.magnitude, minMomentum, 300) * transform.forward;
        }

        Speed = (int)_rb.velocity.magnitude;
    }

    private IEnumerator SpinoutRoutine()
    {
        _spinningOut = true;
        _anim.SetBool("spinningOut", _spinningOut);
        var targetVelocity = PlayerData.Instance.InSmog ? _minSmogMomentum : _minAirMomentum;
        
        //Without the +1 here, the ship will get stuck in the Smog Sea for a reason I haven't figured out yet.
        while (_rb.velocity.magnitude > targetVelocity + 1)
        {
            _rb.velocity = Vector3.Lerp(_rb.velocity, new Vector3(0, 0, 0), .1f);
            yield return null;
            targetVelocity = PlayerData.Instance.InSmog ? _minSmogMomentum : _minAirMomentum;
        }
        yield return null;
        _spinningOut = false; 
        _anim.SetBool("spinningOut", _spinningOut);
    }

    private IEnumerator BounceBackRoutine(ObstacleHitEventArgs args)
    {
        _bouncing = true;

        float timer = 0;

        Vector3 bounceVelocity = args.ContactPoint.normal * 200f;
        Vector3 endVelocity = args.ContactPoint.normal * _minAirMomentum;

        while (timer < .75f)
        {
            timer += Time.deltaTime;
            _rb.velocity = Vector3.Lerp(bounceVelocity, endVelocity,  timer/.75f);
            yield return null;
        }

        _rb.velocity = endVelocity;
        _bouncing = false;
    }
}
