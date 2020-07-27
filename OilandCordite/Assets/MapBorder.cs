﻿using System.Collections;
using UnityEngine;

public class MapBorder : MonoBehaviour
{
    [SerializeField] private float _openAnimTime;

    [SerializeField] private float _minWidth;
    [SerializeField] private float _minHeight;

    private float _maxWidth;
    private float _maxHeight;

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        
        _maxWidth = _rectTransform.sizeDelta.x;
        _maxHeight = _rectTransform.sizeDelta.y;
    }

    public float Open()
    {
        StartCoroutine(OpenAnim());

        return _openAnimTime;
    }

    private IEnumerator OpenAnim()
    {
        float timer = 0f;

        while(timer < _openAnimTime)
        {
            _rectTransform.sizeDelta = new Vector2(
                Mathf.Lerp(_minWidth, _maxWidth, timer / _openAnimTime),
                Mathf.Lerp(_minHeight, _maxHeight, timer / _openAnimTime));

            timer += Time.deltaTime;

            yield return null;
        }

        _rectTransform.sizeDelta = new Vector2(_maxWidth, _maxHeight);
    }
}
