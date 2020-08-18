using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeatUIController : BaseUIController
{
    [SerializeField] private GameObject _heatBar;
    [SerializeField] private GameObject _heatSlider;
    [SerializeField] private GameObject _heatMask;
    [SerializeField] private TMP_Text _warningText;
    [SerializeField] private float _minMaskSize;
    [SerializeField] private float _maxMaskSize;
    [SerializeField] private float _warningThreshold;
    [SerializeField] private AnimationCurve _warningAlphaAnimaiton;

    private Slider _sliderComp;
    private RectTransform _maskTransform;

    private float _maskRange;
    private float _maskY;

    private float _warningAlphaTime;
    private bool _warningAnimationRunning = false;

    private void Awake()
    {
        _maskRange = _maxMaskSize - _minMaskSize;

        _sliderComp = _heatSlider.GetComponent<Slider>();
        _maskTransform = _heatMask.GetComponent<RectTransform>();
        _maskY = _maskTransform.sizeDelta.y;

        _warningAlphaTime = _warningAlphaAnimaiton.keys[_warningAlphaAnimaiton.length - 1].time;
        _warningText.alpha = 0f;
    }

    private void Update()
    {
        _heatBar.transform.SetParent(transform, true);
        _heatSlider.transform.SetParent(transform, true);

        _maskTransform.sizeDelta = new Vector2(_minMaskSize + PlayerData.Instance.Heat / 100 * _maskRange, _maskY);

        _heatBar.transform.SetParent(_heatMask.transform, true);
        _heatSlider.transform.SetParent(_heatMask.transform, true);

        _sliderComp.value = PlayerData.Instance.Heat;

        if (PlayerData.Instance.Heat >= _warningThreshold && !_warningAnimationRunning) StartCoroutine(WarningRoutine());
    }

    private IEnumerator WarningRoutine()
    {
        _warningAnimationRunning = true;

        float warningUptime = 0f;

        while(PlayerData.Instance.Heat >= _warningThreshold)
        {
            warningUptime = (warningUptime + Time.deltaTime * (1 + (PlayerData.Instance.IsOverHeated ? 1 : 0))) % _warningAlphaTime;

            _warningText.alpha = _warningAlphaAnimaiton.Evaluate(warningUptime);

            yield return null;
        }

        _warningText.alpha = 0f;

        _warningAnimationRunning = false;
    }
}
