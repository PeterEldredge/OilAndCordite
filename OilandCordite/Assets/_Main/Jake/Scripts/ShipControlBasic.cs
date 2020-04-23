using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControlBasic : MonoBehaviour
{

    [Header("Physics")]
    [Tooltip("Force to push plane forwards with")] public float igniteThrust = 200f;
    [Tooltip("Pitch, Yaw, Roll")] public Vector3 turnTorque = new Vector3(60f, 25f, 45f);
    [Tooltip("Multiplier for all forces")] public float rotateMult = 3f;
    [Tooltip("Increase gravity")]public float gravMult = 3.0f;
    public float maxAcceleration = 250f;
    public float minAcceleration = -125;

    [Header("Options")]
    [Tooltip("Invert the pitching of the ship to avoid \"normal\" plane controls")] public bool invertY = false;

    [Header("Input")]
    [SerializeField] [Range(-1f, 1f)] private float _pitch = 0f;
    [SerializeField] [Range(-1f, 1f)] private float _roll = 0f;

    //States
    private bool _igniting = false;
    private bool _overheating = false;

    private Rigidbody rb;
    [SerializeField] private AnimationCurve ac;

    //-1 or 1, depending on whether invertY is true or false
    private int _invertYControl = 1;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        _igniting = Input.GetMouseButton(0);
        if (invertY) { _invertYControl = -1; }
        else { _invertYControl = 1; }

        _pitch = Input.GetAxis("Vertical") * _invertYControl;
        _roll = Input.GetAxis("Horizontal");

        transform.Rotate(new Vector3(turnTorque.x * _pitch, 0, -turnTorque.z * _roll) * rotateMult * Time.fixedDeltaTime, Space.Self);

        float RotationMult = (transform.forward.y + 1)/2;
        //Debug.Log(RotationMult);
        
        rb.velocity = Mathf.Clamp(rb.velocity.magnitude, 60, 300) * transform.forward;
        //rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.magnitude, 20, 200), rb.velocity.y, rb.velocity.z);

        //Mapping RotationMult to AnimationCurve
        float acceleration = (ac.Evaluate(RotationMult) * (maxAcceleration - minAcceleration)) + minAcceleration;

        //Gravity
        //rb.velocity += (-Vector3.up * 9.8f) * gravMult * Time.fixedDeltaTime;
        rb.velocity += (transform.forward * acceleration ) * Time.fixedDeltaTime;

        //rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.x, 0, 10f), rb.velocity.y, rb.velocity.z);

        if (transform.position.y <= 0)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (_igniting)
        {
            //Debug.Log(_igniting);
            //Debug.Log(other.tag);
            if (other.tag == "Smog") {
                rb.velocity += transform.forward * igniteThrust * ((PlayerStats.heat / 100) + 20) * Time.fixedDeltaTime;
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

    private float CalculateVelocity()
    {
        return 0f;
    }

}
