using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControlBasic : MonoBehaviour
{

    [Header("Physics")]
    [Tooltip("Force to push plane forwards with")] public float igniteThrust = 100f;
    [Tooltip("Pitch, Yaw, Roll")] public Vector3 turnTorque = new Vector3(60f, 25f, 45f);
    [Tooltip("Multiplier for all forces")] public float forceMult = 100f;
    [Tooltip("Increase gravity")]public float GravMult = 30.0f;

    [Header("Options")]
    [Tooltip("Invert the pitching of the ship to avoid \"normal\" plane controls")] public bool invertY = false;

    [Header("Input")]
    [SerializeField] [Range(-1f, 1f)] private float _pitch = 0f;
    [SerializeField] [Range(-1f, 1f)] private float _roll = 0f;

    //States
    private bool _igniting = false;
    private bool _overheating = false;

    Rigidbody rb;

    //-1 or 1, depending on whether invertY is true or false
    private int _invertYControl = 1;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();   
    }

    // Update is called once per frame
    void Update()
    {
        _igniting = Input.GetKey(KeyCode.LeftShift);

        if (invertY){ _invertYControl = -1; }
        else { _invertYControl = 1; }

        _pitch = Input.GetAxis("Vertical") * _invertYControl;
        _roll = Input.GetAxis("Horizontal");

        if (_igniting)
        {
            rb.velocity += transform.forward * igniteThrust * forceMult * Time.deltaTime;
        }

        transform.Rotate(new Vector3(turnTorque.x * _pitch, 0, -turnTorque.z * _roll) * forceMult * Time.deltaTime, Space.Self);
        
        //transform.Rotate(new Vector3( 1f, 0, 0) * forceMult * Time.deltaTime, Space.World);

        float RotationMult = Mathf.Abs(transform.forward.y);
        //rb.AddForce(Physics.gravity * GravMult * RotationMult, ForceMode.Acceleration);

        if (transform.position.y <= 0)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }

    private void FixedUpdate()
    {
    }

    private float CalculateVelocity()
    {
        return 0f;
    }

}
