using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeedUIController : BaseUIController
{
    [SerializeField] private TMP_Text _speedText;

    private void Update()
    {
        _speedText.text = PlayerData.Instance.Speed.ToString();
    }
}
