using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreUIController : BaseUIController
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _comboText;
    [SerializeField] private Slider _comboSlider;

    [SerializeField] private GameObject _comboPanel;
    [SerializeField] private GameObject _staticScorePanel;
    [SerializeField] private GameObject _scorePanel;
    [SerializeField] private GameObject _comboBarPanel;

    [SerializeField] private float _maxScale;
    [SerializeField] private AnimationCurve _uiFontAnimation;

    private float _uiSizeAnimTime;
    private bool _textAnimationRunning = false;
    private Coroutine _textAnimationRoutine;

    private void Awake()
    {
        _comboSlider.maxValue = BaseScoring.COMBO_TIME;

        _uiSizeAnimTime = _uiFontAnimation.keys[_uiFontAnimation.length - 1].time;
    }

    private void Update()
    {
        _scoreText.text = MissionControllerData.Instance.MissionController.Score.ToString();
        _comboText.text = "x " + MissionControllerData.Instance.MissionController.Combo.ToString();
        _comboSlider.value = MissionControllerData.Instance.MissionController.ComboTimer;
    }

    #region Events

    protected override void OnPlayerDefeatedEnemy(Events.PlayerDefeatedEnemyEventArgs args)
    {
        if (_textAnimationRunning) StopCoroutine(_textAnimationRoutine);
        _textAnimationRoutine = StartCoroutine(TextAnimation());
    }

    private IEnumerator TextAnimation()
    {
        _textAnimationRunning = true;

        float timer = 0f;

        float scaleDifference = _maxScale - 1f;

        while (timer < _uiSizeAnimTime)
        {
            _comboPanel.transform.localScale = Vector3.one * (1 + _uiFontAnimation.Evaluate(timer / scaleDifference) * scaleDifference);
            _staticScorePanel.transform.localScale = _comboPanel.transform.localScale;
            _scorePanel.transform.localScale = _comboPanel.transform.localScale;
            _comboBarPanel.transform.localScale = _comboPanel.transform.localScale;

            timer += Time.deltaTime;

            yield return null;
        }

        _comboPanel.transform.localScale = Vector3.one;
        _staticScorePanel.transform.localScale = Vector3.one;
        _scorePanel.transform.localScale = Vector3.one;
        _comboBarPanel.transform.localScale = Vector3.one;

        _textAnimationRunning = false;
    }

    #endregion
}
