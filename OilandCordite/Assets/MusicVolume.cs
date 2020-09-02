using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Events
{
    public struct MusicVolumeChangedEventArgs : IGameEvent
    {
        public int MusicVolume { get; private set; }

        public MusicVolumeChangedEventArgs(int musicVolume)
        {
            MusicVolume = musicVolume;
        }
    }
}

public class MusicVolume : GameEventUserObject
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
        _slider.value = Settings.Instance.MusicVolume;
        _text.text = Settings.Instance.MusicVolume.ToString();
    }

    public void UpdateMusicVolume(float volume)
    {
        int iVolume = Mathf.RoundToInt(volume);

        _text.text = iVolume.ToString();

        EventManager.Instance.TriggerEvent(new Events.MusicVolumeChangedEventArgs(iVolume));
    }
}
