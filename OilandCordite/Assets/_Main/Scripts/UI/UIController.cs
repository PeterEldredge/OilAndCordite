using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;
using System;

public class UIController : GameEventUserObject
{
    [SerializeField] private Text _healthText;
    [SerializeField] private Text _heatText;
    [SerializeField] private Slider _healthBar;
    [SerializeField] private Slider _heatBar;
    [SerializeField] private Text _scoreText;
    [SerializeField] private Text _comboText;
    [SerializeField] private GameObject _pauseMenuUI;
    [SerializeField] private GameObject _deathMenuUI;
    [SerializeField] private GameObject _controlsUI;

    private GameObject _currentMenu;

    private bool _paused =  false;
    private bool _dead = false;

    private AudioCuePlayer _acp;

    private void Awake()
    {
        _acp = gameObject.GetComponent<AudioCuePlayer>();
    }

    private IEnumerator ActionOnDelay(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);

        action.Invoke();
    }

    private void OnPlayerDeath(PlayerDeathEventArgs args) => StartCoroutine(ActionOnDelay(3f, () => OpenDeathScreen()));

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<PlayerDeathEventArgs>(this, OnPlayerDeath);
    }

    public override void Unsubscribe()
    {
        EventManager.Instance.RemoveListener<PlayerDeathEventArgs>(this, OnPlayerDeath);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealth();
        UpdateHeat();
        UpdateScore();
        UpdateCombo();

        if ((InputHelper.Player.GetButtonDown("Start") || InputHelper.Player.GetButtonDown("UICancel")) && !_dead) {
            if (!_paused)
                Pause();
            else
                Resume();
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
    }

    public void Pause()
    {
        if (_currentMenu != null)
        {
            _currentMenu.SetActive(false);
        }
        _pauseMenuUI.SetActive(true);
        _currentMenu = _pauseMenuUI;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        _paused = true;
    }

    public void Resume()
    {
        _currentMenu.SetActive(false);
        _currentMenu = null;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
        _paused = false;
    }

    public void OpenDeathScreen()
    {
        _dead = true;
        _deathMenuUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
    }

    public void Controls()
    {
        _currentMenu.SetActive(false);
        _controlsUI.SetActive(true);
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
}
