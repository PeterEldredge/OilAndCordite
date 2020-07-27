using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightInfoBorder : MonoBehaviour
{
    [SerializeField] private float _openAnimTime;
    [SerializeField] private float _minWidth;

    private float _maxWidth;

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        _maxWidth = _rectTransform.sizeDelta.x;
    }

    public float Open()
    {
        StartCoroutine(OpenAnim());

        return _openAnimTime;
    }

    private IEnumerator OpenAnim()
    {
        float timer = 0f;

        while (timer < _openAnimTime)
        {
            _rectTransform.sizeDelta = new Vector2(
                Mathf.Lerp(_minWidth, _maxWidth, timer / _openAnimTime),
                _rectTransform.sizeDelta.y);

            timer += Time.deltaTime;

            yield return null;
        }

        _rectTransform.sizeDelta = new Vector2(_maxWidth, _rectTransform.sizeDelta.y);
    }
}
