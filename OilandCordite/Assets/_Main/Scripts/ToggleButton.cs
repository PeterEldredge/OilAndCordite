using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
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
