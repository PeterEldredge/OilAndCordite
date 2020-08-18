using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Events
{
    public struct FramerateChangedEventArgs : IGameEvent
    {
        public int FramerateCap { get; private set; }

        public FramerateChangedEventArgs(int framerateCap)
        {
            FramerateCap = framerateCap;
        }
    }
}

public class FramerateSettings : GameEventUserObject
{
    private const float _TRANSITION_TIME = .16f; 

    [SerializeField] private Slider _slider;
    
    [SerializeField] private TMP_Text _value;

    [SerializeField] private Image _sliderBackground;
    [SerializeField] private Image _sliderFill;
    [SerializeField] private Image _sliderKnob;

    [SerializeField] private Color _enabledColor;
    [SerializeField] private Color _disabledColor;

    private int _framerateCap;

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
        _framerateCap = Application.targetFrameRate;

        _slider.value = _framerateCap;
        _value.text = _framerateCap.ToString();
    }

    private void Update()
    {
        if(Settings.Instance.VSyncCount > 0)
        {
            if(_slider.interactable)
            {
                _slider.interactable = false;

                DisableTrasition();
            }

            _value.text = (Screen.currentResolution.refreshRate / Settings.Instance.VSyncCount).ToString();
        }
        else
        {
            if(!_slider.interactable)
            {
                _slider.interactable = true;

                EnableTransition();
            }

            _value.text = _framerateCap.ToString();
        }
    }

    public void UpdateFramerateCap(float cap)
    {
        _framerateCap = (int)cap;

        _value.text = _framerateCap.ToString();

        EventManager.Instance.TriggerEvent(new Events.FramerateChangedEventArgs(_framerateCap));
    }

    private void EnableTransition() => StartCoroutine(TransitionRoutine(_disabledColor, _enabledColor));
    private void DisableTrasition() => StartCoroutine(TransitionRoutine(_enabledColor, _disabledColor));

    private IEnumerator TransitionRoutine(Color startingColor, Color endingColor)
    {
        float timer = 0;

        Color currentColor;

        while(timer < _TRANSITION_TIME)
        {
            currentColor = Color.Lerp(startingColor, endingColor, timer / _TRANSITION_TIME);

            _sliderBackground.color = currentColor;
            _sliderFill.color = currentColor;
            _sliderKnob.color = currentColor;

            yield return null;

            timer += Time.deltaTime;
        }

        _sliderBackground.color = endingColor;
        _sliderFill.color = endingColor;
        _sliderKnob.color = endingColor;
    }
}
