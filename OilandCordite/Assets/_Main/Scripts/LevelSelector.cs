using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    private const float MAX_SCALE = 1.6f;
    private const float ROTATION_SPEED = 250f;
    private const float SCALE_UP_SPEED = 4f;
    private const float SCALE_DOWN_SPEED = 2f;

    [SerializeField] private Level _selectorLevel;
    public Level SelectorLevel => _selectorLevel;

    private LevelManager _levelManager;
    private Image _image;

    private bool _animRunning = false;
    private bool _runAnimAfter = false;

    private void Awake()
    {
        _levelManager = GetComponentInParent<LevelManager>();
        _image = GetComponentInChildren<Image>();
    }

    private void OnEnable()
    {
        transform.localScale = Vector3.one;

        _animRunning = false;
        _runAnimAfter = false;
    }

    private void Update()
    {
        if(!_animRunning && _runAnimAfter)
        {
            if(_levelManager.CurrentLevel == _selectorLevel)
            {
                StartCoroutine(SelectedAnimation());
            }
        }
    }

    public void OnClicked()
    {
        _levelManager.UpdateCurrentLevel(_selectorLevel);

        if (_animRunning) _runAnimAfter = true;
        else StartCoroutine(SelectedAnimation());
    }

    private IEnumerator SelectedAnimation()
    {
        _animRunning = true;

        float timer = 0f;

        while(_levelManager.CurrentLevel == _selectorLevel)
        {
            timer += Time.deltaTime * SCALE_UP_SPEED;

            transform.localScale = Vector3.one * Mathf.Lerp(1, MAX_SCALE, timer);
            _image.transform.localEulerAngles += new Vector3(0, 0, Time.deltaTime * ROTATION_SPEED);

            yield return null;
        }

        while(transform.localScale.x > 1)
        {
            transform.localScale -= Vector3.one * Time.deltaTime * SCALE_DOWN_SPEED;

            yield return null;
        }

        transform.localScale = Vector3.one;

        _animRunning = false;
    }
}
