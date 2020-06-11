using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

namespace Events
{
    public struct GamePausedEventArgs : IGameEvent { }
    public struct GameUnpausedEventArgs : IGameEvent { }
}

public class UIController : GameEventUserObject
{
    [SerializeField] private Text _healthText;
    [SerializeField] private Text _heatText;
    [SerializeField] private Slider _healthBar;
    [SerializeField] private Slider _heatBar;
    [SerializeField] private Text _scoreText;
    [SerializeField] private Text _comboText;
    [SerializeField] private Text _speedText;
    [SerializeField] private Slider _comboSlider;
    [SerializeField] private GameObject _pauseMenuUI;
    [SerializeField] private GameObject _pauseMenuDefaultSelected;
    [SerializeField] private GameObject _victoryMenuUIDefaultSelected;
    [SerializeField] private GameObject _victoryMenuUI;
    [SerializeField] private GameObject _deathMenuUIDefaultSelected;
    [SerializeField] private GameObject _deathMenuUI;
    [SerializeField] private GameObject _controlsUIDefaultSelected;
    [SerializeField] private GameObject _controlsUI;
    [SerializeField] private GameObject _comboPanel;
    [SerializeField] private GameObject _scorePanel;

    [SerializeField] private float _maxScale;
    [SerializeField] private AnimationCurve _uiFontAnimation;

    private GameObject _currentMenu;

    private bool _paused =  false;
    private bool _dead = false;
    private bool _uiHidden = false;

    private float _uiSizeAnimTime;
    private bool _textAnimationRunning = false;
    private Coroutine _textAnimationRoutine;

    private AudioCuePlayer _acp;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _acp = gameObject.GetComponent<AudioCuePlayer>();
        _canvasGroup = gameObject.GetComponent<CanvasGroup>();

        _comboSlider.maxValue = BaseScoring.COMBO_TIME;

        _uiSizeAnimTime = _uiFontAnimation.keys[_uiFontAnimation.length - 1].time;
    }

    private IEnumerator ActionOnDelay(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);

        action.Invoke();
    }

    private void OnPlayerDeath(Events.PlayerDeathEventArgs args) => StartCoroutine(ActionOnDelay(3f, () => OpenDeathScreen()));
    private void OnMissionCompleted(Events.MissionCompleteEventArgs args) => StartCoroutine(ActionOnDelay(.5f, () => OpenVictoryScreen()));
    private void OnPlayerDefeatedEnemy(Events.PlayerDefeatedEnemyEventArgs args)
    {
        if(_textAnimationRunning) StopCoroutine(_textAnimationRoutine);
        _textAnimationRoutine = StartCoroutine(TextAnimation());
    }

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.PlayerDeathEventArgs>(this, OnPlayerDeath);
        EventManager.Instance.AddListener<Events.MissionCompleteEventArgs>(this, OnMissionCompleted);
        EventManager.Instance.AddListener<Events.PlayerDefeatedEnemyEventArgs>(this, OnPlayerDefeatedEnemy);
    }

    public override void Unsubscribe()
    {
        EventManager.Instance.RemoveListener<Events.PlayerDeathEventArgs>(this, OnPlayerDeath);
        EventManager.Instance.RemoveListener<Events.MissionCompleteEventArgs>(this, OnMissionCompleted);
        EventManager.Instance.RemoveListener<Events.PlayerDefeatedEnemyEventArgs>(this, OnPlayerDefeatedEnemy);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealth();
        UpdateHeat();
        UpdateScore();
        UpdateCombo();
        UpdateSpeed();

        if ((InputHelper.Player.GetButtonDown("Start") || InputHelper.Player.GetButtonDown("UICancel")) && !_dead) {
            if (!_paused)
                Pause();
            else
                Resume();
        }  

        if (InputHelper.Player.GetButtonDown("UIToggle"))
        {
            if (!_uiHidden)
            {
                HideUI();             
            }
            else
            {
                ShowUI();
            }
        }
    }

    void UpdateHealth()
    {
        _healthText.text = "Health:" + PlayerData.Instance.Health.ToString("F0");
        _healthBar.value = PlayerData.Instance.Health > 0f ? PlayerData.Instance.Health : 0f;
    }

    void UpdateHeat()
    {
        _heatText.text = "Heat:" + PlayerData.Instance.Heat.ToString("F1");
        _heatBar.value = PlayerData.Instance.Heat;
    }

    void UpdateScore()
    {
        _scoreText.text = "Score: " + MissionControllerData.Instance.MissionController.Score.ToString();
    }

    void UpdateCombo()
    {
        _comboText.text = "Combo: " + MissionControllerData.Instance.MissionController.Combo.ToString();
        _comboSlider.value = MissionControllerData.Instance.MissionController.ComboTimer;
    }

    void UpdateSpeed()
    {
        _speedText.text = "Speed: " + PlayerData.Instance.Speed;
    }

    public void Pause()
    {
        AudioListener.pause = true;

        _acp.PlaySound("Pause");

        if (_currentMenu != null)
        {
            _currentMenu.SetActive(false);
        }
        _pauseMenuUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_pauseMenuUI);
        _currentMenu = _pauseMenuUI;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        EventSystem.current.SetSelectedGameObject(_pauseMenuDefaultSelected);
        Time.timeScale = 0f;
        _paused = true;

        EventManager.Instance.TriggerEvent(new Events.GamePausedEventArgs());
    }

    public void Resume()
    {
        _acp.PlaySound("Menu_Item_Select");

        AudioListener.pause = false;
        _currentMenu.SetActive(false);
        _currentMenu = null;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        Time.timeScale = 1f;
        _paused = false;

        EventManager.Instance.TriggerEvent(new Events.GameUnpausedEventArgs());
    }

    public void OpenVictoryScreen()
    {
        _victoryMenuUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        EventSystem.current.SetSelectedGameObject(_victoryMenuUIDefaultSelected);
        Time.timeScale = 0f;
    }

    public void OpenDeathScreen()
    {
        _dead = true;
        _deathMenuUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        EventSystem.current.SetSelectedGameObject(_deathMenuUIDefaultSelected);
        Time.timeScale = 0f;
    }

    public void Controls()
    {
        _acp.PlaySound("Menu_Item_Select");
        _currentMenu.SetActive(false);
        _controlsUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_controlsUIDefaultSelected);
        _currentMenu = _controlsUI;
    }

    public void ToMainMenu()
    {
        Time.timeScale = 1f;
        SceneController.SwitchScene(0);
    }

    public void ReloadScene()
    {
        Time.timeScale = 1f;
        SceneController.ReloadScene();
    }

    private void HideUI()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        _uiHidden = true;
    }

    private void ShowUI()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        _uiHidden = false;
    }

    private IEnumerator TextAnimation()
    {
        _textAnimationRunning = true;

        float timer = 0f;

        float scaleDifference = _maxScale - 1f;

        while(timer < _uiSizeAnimTime)
        {
            _comboPanel.transform.localScale = Vector3.one * (1 + _uiFontAnimation.Evaluate(timer / scaleDifference) * scaleDifference);
            _scorePanel.transform.localScale = _comboPanel.transform.localScale;

            timer += Time.deltaTime;

            yield return null;
        }

        _comboPanel.transform.localScale = Vector3.one;
        _scorePanel.transform.localScale = Vector3.one;

        _textAnimationRunning = false;
    }
}
