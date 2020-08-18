using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Events
{
    public struct VSyncChangedEventArgs : IGameEvent
    {
        public int VSyncCount { get; private set; }

        public VSyncChangedEventArgs(int vsyncCount)
        {
            VSyncCount = vsyncCount;
        }
    }
}

public class VSyncSettings : GameEventUserObject
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text _value;

    private int _vsyncCount;

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.RefreshSettingsUIArgs>(this, Refresh);
    }

    public override void Unsubscribe()
    {
        EventManager.Instance.AddListener<Events.RefreshSettingsUIArgs>(this, Refresh);
    }

    private void Refresh(Events.RefreshSettingsUIArgs args)
    {
        _slider.value = QualitySettings.vSyncCount;
        _value.text = QualitySettings.vSyncCount.ToString();
    }

    public void UpdateVSync(float count)
    {
        _vsyncCount = (int)count;

        _value.text = _vsyncCount.ToString();

        EventManager.Instance.TriggerEvent(new Events.VSyncChangedEventArgs(_vsyncCount));
    }
}
