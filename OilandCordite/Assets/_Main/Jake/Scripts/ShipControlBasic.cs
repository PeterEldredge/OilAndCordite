using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControlBasic : GameEventUserObject
{
    [Header("Physics")]
    [Tooltip("Force to push plane forwards with")]
    [SerializeField] public float _igniteThrust = 1000f;
    [Tooltip("Pitch, Yaw, Roll")]
    [SerializeField] private Vector3 _turnTorque = new Vector3(60f, 45f, 90f);
    [Tooltip("Multiplier for all forces")]
    [SerializeField] private float _rotateMult = 3f;
    [Tooltip("Amount the multiplier will decrease by at max speed")]
    [SerializeField] private float _rotateMultDecrease = 1.5f;
    [SerializeField] private float _spinoutAddedTurnTorque = 120f;
    [SerializeField] private float _maxAcceleration = 150f;
    [SerializeField] private float _minAcceleration = -125f;
    [SerializeField] private float _baseMaxSpeed = 150f;
    [SerializeField] private float _explosionMaxSpeed = 200f;
    [SerializeField] private float _explosionSpeedLossRate = 20f;
    [SerializeField] private float _minSmogSpeed = 0f;
    [SerializeField] private float _minAirSpeed = 50f;
    [SerializeField] private float _airResistance = 1f;
    [SerializeField] private float _minSmogIgnitionHeat = 0f;
    [SerializeField] private float _gravityMultiplier = 50f;
    [SerializeField] private float _smogHeatToSpeedRatio = .4f;
    [SerializeField] private float _baseGasBoost = 300f;
    [SerializeField] private float _bounceMult = 20f;
    [Tooltip("The speed at which gravity stops affecting the ship")]
    [SerializeField] private float _noGravitySpeed = 200f;
    [Tooltip("When calculating the amount of thrust to receive, Gas Clouds should give a substantial boost even if the player's heat is 0")]
    [SerializeField] private float _minGasIgnitionHeat = 20f;
    [SerializeField] private AnimationCurve _positiveAccelerationCurve;
    [SerializeField] private AnimationCurve _negativeAccelerationCurve;
    [SerializeField] private AnimationCurve _gravityAccelerationCurve;
    [SerializeField] private AnimationCurve _rotateMultSpeedCurve;

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
    public float MaxSpeed => _explosionMaxSpeed;
    public bool SpinningOut { get; private set; }

    //Private
    private Rigidbody _rb;
    private Transform _shipForRotation;
    private AudioCuePlayer _acp;

    private Action _inputCalculation;

    private int _invertYControl;
    private int _shipFlippedControl = 1;
    private bool _shipFlipped = false;

    private float _xMousePosition;
    private float _yMousePosition;

    private bool _spinningOut = false;
    private bool _bouncing = false;
    private bool _gasExploding = false;

    private float _activeMaxSpeed;

    private Vector3 _turnTorqueCopy;

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
        _yMousePosition += Input.GetAxisRaw("Mouse Y") / 100f * _ySensitivity * _invertYControl * _shipFlippedControl;

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
        _pitch = InputHelper.Player.GetAxis("Pitch") * _invertYControl * _shipFlippedControl;
        _roll = InputHelper.Player.GetAxis("Roll Right") + InputHelper.Player.GetAxis("Roll Left");
        _turn = InputHelper.Player.GetAxis("Turn");
    }

    #endregion

    private void Awake()
    {
        if(!FindObjectOfType<Settings>())
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        Cursor.visible = false;

        _rb = GetComponent<Rigidbody>();
        _shipForRotation = GetComponentsInChildren<Transform>()[1];
        _acp = GetComponent<AudioCuePlayer>();
        _turnTorqueCopy = _turnTorque;
    }

    private void OnObstacleHit(Events.ObstacleHitEventArgs args) => StartCoroutine(BounceBackRoutine(args));
    private void OnGasExplosion(Events.GasExplosionEventArgs args) => GasExplosion(args);
    private void OnSawBladeBoost(Events.SawBladeBoostEventArgs args) => SawBladeBoost(args);

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.ObstacleHitEventArgs>(this, OnObstacleHit);
        EventManager.Instance.AddListener<Events.GasExplosionEventArgs>(this, OnGasExplosion);
        EventManager.Instance.AddListener<Events.SawBladeBoostEventArgs>(this, OnSawBladeBoost);
    }

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

        if (Input.GetKeyDown(KeyCode.V))
        {
            QualitySettings.vSyncCount = 2;
        }

        _inputCalculation.Invoke();
    }

    private void FixedUpdate()
    {
        var newRotateMult = _rotateMult - _rotateMultSpeedCurve.Evaluate(Mathf.Clamp(_rb.velocity.magnitude / _explosionMaxSpeed, 0, 1)) * _rotateMultDecrease;

        transform.Rotate(new Vector3(_turnTorqueCopy.x * _pitch, 0, 0) * newRotateMult * Time.fixedDeltaTime, Space.Self);
        transform.rotation = Quaternion.Euler(new Vector3(0, _turnTorqueCopy.y * _turn, 0) * newRotateMult * Time.fixedDeltaTime) * transform.rotation;
        _shipForRotation.Rotate(new Vector3(0, 0, -_turnTorqueCopy.z * _roll) * _rotateMult * Time.fixedDeltaTime, Space.Self);

        float minMomentum = _minSmogSpeed;
        if (!PlayerData.Instance.InSmog)
            minMomentum = _minAirSpeed;
        if (_rb.velocity.magnitude > _baseMaxSpeed)
        {
            _rb.velocity -= transform.forward * _explosionSpeedLossRate * Time.fixedDeltaTime;
        }

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
            _rb.velocity -= transform.forward * _airResistance * Time.fixedDeltaTime;

            if (PlayerData.Instance.InGas)
            {
                //_rb.velocity += transform.forward * _igniteThrust * (Mathf.Clamp(PlayerData.Instance.Heat, _minGasIgnitionHeat * (PlayerData.Instance.IsIgniting ? 1 : 0), 100) / 100);
            }
            else if (PlayerData.Instance.InSmog)
            {
                _rb.velocity += transform.forward * _igniteThrust * _smogHeatToSpeedRatio * (Mathf.Clamp(PlayerData.Instance.Heat, _minSmogIgnitionHeat, 100) / 100) * Time.fixedDeltaTime;
            }

            if (transform.position.y <= 15)
            {
                transform.position = new Vector3(transform.position.x, 15, transform.position.z);
            }

            float gravity = -_gravityMultiplier * _gravityAccelerationCurve.Evaluate(Mathf.Clamp(_rb.velocity.magnitude/_noGravitySpeed, 0f, 1f)) * Time.fixedDeltaTime;
            
            transform.Translate(new Vector3(0, gravity, 0), Space.World);

            if (_gasExploding && (_rb.velocity.magnitude > _baseMaxSpeed))
            {
                _activeMaxSpeed = _explosionMaxSpeed;
            }
            else if (_rb.velocity.magnitude <= _baseMaxSpeed)
            {
                _activeMaxSpeed = _baseMaxSpeed;
                _gasExploding = false;
            }

            _rb.velocity = Mathf.Clamp(_rb.velocity.magnitude, minMomentum, _activeMaxSpeed) * transform.forward;

        }

        if (Mathf.Abs(InputHelper.Player.GetAxis("Pitch")) < .2)
        {
            if (Vector3.Dot(transform.up, Vector3.down) > .3 * _shipFlippedControl)
            {
                if (!_shipFlipped)
                {
                    _shipFlippedControl *= -1;
                    _shipFlipped = true;
                }
            }
            else
            {
                if (_shipFlipped)
                {
                    _shipFlippedControl *= -1;
                    _shipFlipped = false;
                }
            }
        }

        if (!_spinningOut)
        {
           _turnTorqueCopy.x = Mathf.Lerp(_turnTorque.x, _turnTorque.y, 1 - Mathf.Abs(Vector3.Dot(_shipForRotation.transform.up, transform.up)));
           _turnTorqueCopy.y = Mathf.Lerp(_turnTorque.y, _turnTorque.x, 1 - Mathf.Abs(Vector3.Dot(_shipForRotation.transform.up, transform.up)));
        }

        if(transform.localEulerAngles.z < 179 || transform.localEulerAngles.z > 181) transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);

        Speed = (int)_rb.velocity.magnitude;
        SpinningOut = _spinningOut;
    }

    private void GasExplosion(Events.GasExplosionEventArgs args)
    {
        _rb.velocity += transform.forward * (_baseGasBoost + 100 * (PlayerData.Instance.Heat / 100) * args.ExplosionMagnitude);
        _acp.PlaySound("GasExplosion");
        _gasExploding = true;
    }

    private void SawBladeBoost(Events.SawBladeBoostEventArgs args)
    {
        _rb.velocity = transform.forward * (args.BoostSpeed);
        _gasExploding = true;
    }

    private IEnumerator SpinoutRoutine()
    {
        _spinningOut = true;
        _anim.SetBool("spinningOut", _spinningOut);
        var initialVelocity = _rb.velocity;
        var targetVelocity = PlayerData.Instance.InSmog ? _minSmogSpeed : _minAirSpeed;
        _turnTorqueCopy += new Vector3(0, _spinoutAddedTurnTorque, 0);

        float timer = 0;

        //Without the +1 here, the ship will get stuck in the Smog Sea for a reason I haven't figured out yet.
        while ( timer < .25f )
        {
            timer += Time.deltaTime;
            _rb.velocity = Vector3.Lerp(initialVelocity, new Vector3(0, 0, 0), timer/.25f);
            yield return null;
        }
        yield return null;
        _turnTorqueCopy -= new Vector3(0, _spinoutAddedTurnTorque, 0);
        _spinningOut = false; 
        _anim.SetBool("spinningOut", _spinningOut);
    }

    private IEnumerator BounceBackRoutine(Events.ObstacleHitEventArgs args)
    {
        _bouncing = true;

        float timer = 0;

        Vector3 bounceVelocity = args.CollisionNormal * _bounceMult;
        Vector3 endVelocity = args.CollisionNormal * _minAirSpeed;

        while (timer < .75f)
        {
            timer += Time.deltaTime;
            _rb.velocity = Vector3.Lerp(bounceVelocity, endVelocity,  timer/.75f);
            yield return null;
        }

        _bouncing = false;
        _rb.velocity = endVelocity;

        yield return null;
    }
}
