using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Events
{
    public struct MotionBlurToggledEventArgs : IGameEvent
    {
        public bool MotionBlur { get; private set; }

        public MotionBlurToggledEventArgs(bool motionBlur)
        {
            MotionBlur = motionBlur;
        }
    }
}

public class ToggleButton : GameEventUserObject
{
    [SerializeField] private float _toggleTime;

    [SerializeField] private float _offXPosition;

    [SerializeField] private Color _onColor;
    [SerializeField] private Color _offColor;

    public bool IsOn { get; private set; } = true;

    private Image _toggleImage;
    private Button _toggleButton;

    private float _onXPosition;

    private float _yPosition;
    private float _zPosition;

    private void Awake()
    {
        _toggleImage = GetComponent<Image>();
        _toggleButton = GetComponent<Button>();

        _onXPosition = transform.localPosition.x;

        _yPosition = transform.localPosition.y;
        _zPosition = transform.localPosition.z;
    }

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
        if (Settings.Instance.MotionBlur)
        {
            transform.localPosition = new Vector3(_onXPosition, _yPosition, _zPosition);

            _toggleImage.color = _onColor;

            _toggleButton.interactable = true;

            IsOn = true;
        }
        else
        {
            transform.localPosition = new Vector3(_offXPosition, _yPosition, _zPosition);

            _toggleImage.color = _offColor;

            _toggleButton.interactable = true;

            IsOn = false;
        }
    }

    public void Toggle() => StartCoroutine(ToggleRoutine());

    private IEnumerator ToggleRoutine()
    {
        _toggleButton.interactable = false;

        float startXPosition;
        float endXPosition;

        Color startColor;
        Color endColor;

        float timer = 0f;

        IsOn = !IsOn;

        EventManager.Instance.TriggerEvent(new Events.MotionBlurToggledEventArgs(IsOn));

        if(IsOn)
        {
            startXPosition = _offXPosition;
            endXPosition = _onXPosition;

            startColor = _offColor;
            endColor = _onColor;
        }
        else
        {
            startXPosition = _onXPosition;
            endXPosition = _offXPosition;

            startColor = _onColor;
            endColor = _offColor;
        }

        while(timer < _toggleTime)
        {
            transform.localPosition = new Vector3(Mathf.Lerp(startXPosition, endXPosition, timer / _toggleTime), _yPosition, _zPosition);

            _toggleImage.color = Color.Lerp(startColor, endColor, timer / _toggleTime);

            yield return null;

            timer += Time.deltaTime;
        }

        transform.localPosition = new Vector3(endXPosition, _yPosition, _zPosition);

        _toggleImage.color = endColor;

        _toggleButton.interactable = true;
    }
}
