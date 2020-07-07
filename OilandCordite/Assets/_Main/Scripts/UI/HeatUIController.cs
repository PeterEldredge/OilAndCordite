using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeatUIController : BaseUIController
{
    [SerializeField] private GameObject _heatBar;
    [SerializeField] private GameObject _heatSlider;
    [SerializeField] private GameObject _heatMask;
    [SerializeField] private float _minMaskSize;
    [SerializeField] private float _maxMaskSize;

    private Slider _sliderComp;
    private RectTransform _maskTransform;

    private float _maskRange;
    private float _maskY;

    private void Awake()
    {
        _maskRange = _maxMaskSize - _minMaskSize;

        _sliderComp = _heatSlider.GetComponent<Slider>();
        _maskTransform = _heatMask.GetComponent<RectTransform>();
        _maskY = _maskTransform.sizeDelta.y;
    }

    private void Update()
    {
        _heatBar.transform.parent = transform;
        _heatSlider.transform.parent = transform;

        _maskTransform.sizeDelta = new Vector2(_minMaskSize + PlayerData.Instance.Heat / 100 * _maskRange, _maskY);

        _heatBar.transform.parent = _heatMask.transform;
        _heatSlider.transform.parent = _heatMask.transform;

        _sliderComp.value = PlayerData.Instance.Heat;
    }
}
