using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ResolutionSettings : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _dropdown;

    private void Awake()
    {
        _dropdown.options = new List<TMP_Dropdown.OptionData>();

        IEnumerable<Resolution> availableResolutions = Screen.resolutions.AsEnumerable()
            .Where(r => r.refreshRate == Screen.currentResolution.refreshRate);

        foreach (Resolution resolution in availableResolutions)
        {
            _dropdown.options.Insert(0, new TMP_Dropdown.OptionData(resolution.width + " x " + resolution.height));
        }

        for(int i = 0; i < _dropdown.options.Count; i++)
        {
            string[] option = _dropdown.options[i].text.Split(' ');

            if (int.Parse(option[0]) == Screen.currentResolution.width &&
               int.Parse(option[2]) == Screen.currentResolution.height)
            {
                _dropdown.value = i;
                break;
            }
        }
    }

    public void UpdateResolution(int option)
    {
        string[] selectedOptions = _dropdown.options[option].text.Split(' ');

        Screen.SetResolution(int.Parse(selectedOptions[0]), int.Parse(selectedOptions[2]), true);
    }
}
