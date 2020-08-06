using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadeIn : MonoBehaviour
{
    [SerializeField] private float _fadeTime;

    [SerializeField] private Color _startingImageColor;
    [SerializeField] private Color _startingTextColor;

    private Color _endingImageColor;
    private Color _endingTextColor;

    private Image _image;
    private TMP_Text _text;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _text = GetComponentInChildren<TMP_Text>();

        _endingImageColor = _image.color;
        _endingTextColor = _text.color;
    }

    private void OnEnable()
    {
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        float timer = 0;

        while(timer < _fadeTime)
        {
            _image.color = Color.Lerp(_startingImageColor, _endingImageColor, timer / _fadeTime);
            _text.color = Color.Lerp(_startingTextColor, _endingTextColor, timer / _fadeTime);

            yield return null;

            timer += Time.deltaTime;
        }

        _image.color = _endingImageColor;
        _text.color = _endingTextColor;
    }
}
