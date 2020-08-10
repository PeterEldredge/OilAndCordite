﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadePanel : MonoBehaviour
{
    [SerializeField] private float _fadeSpeed;

    [SerializeField] private Color _inColor;
    [SerializeField] private Color _outColor;

    [SerializeField] private bool _fadeInOnAwake;

    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();

        if(_fadeInOnAwake) FadeIn();
    }

    public float FadeIn()
    {
        StartCoroutine(FadeInAnim());

        return _fadeSpeed;
    }

    public float FadeOut()
    {
        StartCoroutine(FadeOutAnim());

        return _fadeSpeed;
    }

    private IEnumerator FadeInAnim()
    {
        float timer = 0;

        while (timer < _fadeSpeed)
        {
            _image.color = Color.Lerp(_outColor, _inColor, timer / _fadeSpeed);

            timer += Time.deltaTime;

            yield return null;
        }

        _image.color = _inColor;
    }

    private IEnumerator FadeOutAnim()
    {
        float timer = 0;

        while(timer < _fadeSpeed)
        {
            _image.color = Color.Lerp(_inColor, _outColor, timer / _fadeSpeed);

            timer += Time.deltaTime;

            yield return null;
        }

        _image.color = _outColor;
    }
}