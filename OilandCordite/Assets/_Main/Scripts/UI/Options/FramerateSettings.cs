using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FramerateSettings : MonoBehaviour
{
    private const float _TRANSITION_TIME = .16f; 

    [SerializeField] private Slider _slider;
    
    [SerializeField] private TMP_Text _value;

    [SerializeField] private Image _sliderBackground;
    [SerializeField] private Image _sliderFill;
    [SerializeField] private Image _sliderKnob;

    [SerializeField] private Color _enabledColor;
    [SerializeField] private Color _disabledColor;


    private void Start()
    {
        _slider.value = Application.targetFrameRate;
        _value.text = Application.targetFrameRate.ToString();
    }

    private void Update()
    {
        if(QualitySettings.vSyncCount > 0)
        {
            if(_slider.interactable)
            {
                _slider.interactable = false;

                DisableTrasition();
            }

            _value.text = (Screen.currentResolution.refreshRate / QualitySettings.vSyncCount).ToString();
        }
        else
        {
            if(!_slider.interactable)
            {
                _slider.interactable = true;

                EnableTransition();
            }

            _value.text = Application.targetFrameRate.ToString();
        }
    }

    public void UpdateFramerateCap(float cap)
    {
        Application.targetFrameRate = (int)cap;

        Time.fixedDeltaTime = 1f / Application.targetFrameRate;

        _value.text = Application.targetFrameRate.ToString();
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
