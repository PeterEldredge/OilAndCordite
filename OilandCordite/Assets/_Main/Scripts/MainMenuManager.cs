using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private float _openAnimTime;

    [SerializeField] private List<Transform> _childrenTransforms;

    private GameObject _holder;

    private RectTransform _rectTransform;

    private Transform _maskTransform;

    private float _maxWidth;

    private void Awake()
    {
        _holder = new GameObject("Holder");

        _rectTransform = GetComponent<RectTransform>();

        _maskTransform = GetComponentInChildren<RectMask2D>().transform;

        _maxWidth = _rectTransform.sizeDelta.x;
    }

    private void OnEnable()
    {
        Open();
    }

    public void Open()
    {
        StartCoroutine(OpenAnim());
    }

    private IEnumerator OpenAnim()
    {
        float timer = 0f;

        while (timer < _openAnimTime)
        {
            foreach(Transform childTransform in _childrenTransforms)
            {
                childTransform.parent = _holder.transform;
            }

            _rectTransform.sizeDelta = new Vector2(
                Mathf.Lerp(0, _maxWidth, timer / _openAnimTime),
                _rectTransform.sizeDelta.y);

            foreach (Transform childTransform in _childrenTransforms)
            {
                childTransform.parent = _maskTransform;
            }

            yield return null;

            timer += Time.deltaTime;
        }
    }
}
