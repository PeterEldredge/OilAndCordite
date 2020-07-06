using UnityEngine;
using UnityEngine.UI;

public class SpeedUIController : BaseUIController
{
    [SerializeField] private Text _speedText;

    private void Update()
    {
        _speedText.text = "Speed: " + PlayerData.Instance.Speed;
    }
}
