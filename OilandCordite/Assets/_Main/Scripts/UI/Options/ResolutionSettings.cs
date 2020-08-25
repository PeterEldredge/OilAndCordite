using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ResolutionSettings : GameEventUserObject
{
    [SerializeField] private TMP_Dropdown _dropdown;

    private List<Resolution> _availableResolutions = new List<Resolution>();

    private void Refresh(Events.RefreshSettingsUIArgs args) => SetCurrentSetting();

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.RefreshSettingsUIArgs>(this, Refresh);
    }

    public override void Unsubscribe()
    {
        EventManager.Instance.RemoveListener<Events.RefreshSettingsUIArgs>(this, Refresh);
    }

    private void Start()
    {
        _dropdown.options = new List<TMP_Dropdown.OptionData>();

        //IEnumerable<Resolution> availableResolutions = Screen.resolutions.AsEnumerable().Where(r => r.refreshRate == Screen.currentResolution.refreshRate);

        foreach (Resolution resolution in Screen.resolutions)
        {
            _dropdown.options.Insert(0, new TMP_Dropdown.OptionData(resolution.ToString()));
            _availableResolutions.Insert(0, resolution);
        }

        SetCurrentSetting();
    }

    private void SetCurrentSetting()
    {
        //I'm sorry god
        string currentResolution = Screen.currentResolution.ToString();
        string currentResolutionMinusOne = $"{currentResolution.Substring(0, currentResolution.Length - 5)}{int.Parse(currentResolution.Substring(currentResolution.Length - 5, 3)) - 1}Hz";

        for (int i = 0; i < _dropdown.options.Count; i++)
        {
            if (currentResolution == _dropdown.options[i].text ||
                currentResolutionMinusOne == _dropdown.options[i].text)
            {
                _dropdown.value = i;
                break;
            }
        }
    }

    public void UpdateResolution()
    {
        Screen.SetResolution(_availableResolutions[_dropdown.value].width, _availableResolutions[_dropdown.value].height, true, _availableResolutions[_dropdown.value].refreshRate);

        Settings.Instance.SaveCurrentSettingsOnDelay();
    }
}
