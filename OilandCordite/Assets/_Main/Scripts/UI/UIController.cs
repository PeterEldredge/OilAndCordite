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

    private bool _paused =  false;

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

        if (InputHelper.Player.GetButtonDown("Start")) {
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

    private void Pause()
    {
        _pauseMenuUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        _paused = true;
    }

    public void Resume()
    {
        _pauseMenuUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
        _paused = false;
    }

    public void OpenDeathScreen()
    {
        _deathMenuUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
    }

    public void Controls()
    {

    }

    public void ToMainMenu()
    {
        Time.timeScale = 1f;
        SceneController.SwitchScene(0);
    }

    public void ReloadScene()
    {
        SceneController.ReloadScene();
    }
}
