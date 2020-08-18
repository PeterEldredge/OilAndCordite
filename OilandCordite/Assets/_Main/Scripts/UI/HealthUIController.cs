using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUIController : BaseUIController
{
    [SerializeField] private GameObject _healthBar;
    [SerializeField] private GameObject _healthMask;
    [SerializeField] private float _minMaskSize;
    [SerializeField] private float _maxMaskSize;

    private RectTransform _maskTransform;

    private float _maskRange;
    private float _maskY;

    private void Awake()
    {
        _maskRange = _maxMaskSize - _minMaskSize;

        _maskTransform = _healthMask.GetComponent<RectTransform>();
        _maskY = _maskTransform.sizeDelta.y;
    }

    private void Update()
    {
        _healthBar.transform.SetParent(transform, true);

        _maskTransform.sizeDelta = new Vector2(_minMaskSize + Mathf.Clamp01(PlayerData.Instance.Health / 100) * _maskRange, _maskY);

        _healthBar.transform.SetParent(_healthMask.transform, true);
    }
}
