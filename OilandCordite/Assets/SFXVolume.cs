using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Events
{
    public struct SFXVolumeChangedEventArgs : IGameEvent
    {
        public int SFXVolume { get; private set; }

        public SFXVolumeChangedEventArgs(int sfxVolume)
        {
            SFXVolume = sfxVolume;
        }
    }
}

public class SFXVolume : GameEventUserObject
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Slider _slider;

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
        _slider.value = Settings.Instance.SFXVolume;
        _text.text = Settings.Instance.SFXVolume.ToString();
    }

    public void UpdateSFXVolume(float volume)
    {
        int iVolume = Mathf.RoundToInt(volume);

        _text.text = iVolume.ToString();

        EventManager.Instance.TriggerEvent(new Events.SFXVolumeChangedEventArgs(iVolume));
    }
}
