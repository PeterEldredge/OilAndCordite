using UnityEngine;

public class DeathSystem : GameEventUserObject
{
    [SerializeField] private GameObject _shipObject;
    [SerializeField] private GameObject _particles;
    [SerializeField] private FlightCam _flightCam;

    private ShipControlBasic _shipControlBasic;
    private AudioCuePlayer _acp;

    private void Awake()
    {
        _shipControlBasic = GetComponent<ShipControlBasic>();
        _acp = GetComponent<AudioCuePlayer>();
    }

    private void OnDeath(Events.PlayerDeathEventArgs args)
    {
        _shipControlBasic.enabled = false;
        _flightCam.enabled = false;
        
        _shipObject.SetActive(false);

        _acp.PlaySound("COM-Buster_Destroyed");

        Instantiate(_particles, transform.position, Quaternion.Euler(Vector3.zero));
    }

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.PlayerDeathEventArgs>(this, OnDeath);
    }

    public override void Unsubscribe()
    {
        EventManager.Instance.AddListener<Events.PlayerDeathEventArgs>(this, OnDeath);
    }
}
