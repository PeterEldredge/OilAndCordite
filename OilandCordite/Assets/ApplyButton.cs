using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplyButton : MonoBehaviour
{
    [SerializeField] private ResolutionSettings _resolutionSettings;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();

        _button.onClick.AddListener(Apply);
    }

    private void Apply()
    {
        _resolutionSettings.UpdateResolution();
    }
}
