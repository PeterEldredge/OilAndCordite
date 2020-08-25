using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ResolutionSettings : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _dropdown;

    private List<Resolution> _availableResolutions = new List<Resolution>();

    private void Start()
    {
        _dropdown.options = new List<TMP_Dropdown.OptionData>();

        //IEnumerable<Resolution> availableResolutions = Screen.resolutions.AsEnumerable().Where(r => r.refreshRate == Screen.currentResolution.refreshRate);

        foreach (Resolution resolution in Screen.resolutions)
        {
            _dropdown.options.Insert(0, new TMP_Dropdown.OptionData(resolution.ToString()));
            _availableResolutions.Insert(0, resolution);
        }

        for (int i = 0; i < _dropdown.options.Count; i++)
        {
            if (Screen.currentResolution.ToString() == _dropdown.options[i].text)
            {
                _dropdown.value = i;
                break;
            }
        }
    }

    public void UpdateResolution(int option)
    {
        Screen.SetResolution(_availableResolutions[option].width, _availableResolutions[option].height, true, _availableResolutions[option].refreshRate);
    }
}
