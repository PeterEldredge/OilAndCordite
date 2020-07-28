using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadePanel : MonoBehaviour
{
    [SerializeField] private float _fadeSpeed;

    [SerializeField] private Color _inColor;
    [SerializeField] private Color _outColor;

    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public float FadeOut()
    {
        StartCoroutine(FadeOutAnim());

        return _fadeSpeed;
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
