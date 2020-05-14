using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class UIController : MonoBehaviour
{
    [SerializeField] private Text _healthText;
    [SerializeField] private Text _heatText;
    [SerializeField] private Slider _healthBar;
    [SerializeField] private Slider _heatBar;
    [SerializeField] private Text _speedText;
    [SerializeField] private GameObject _pauseMenuUI;
    [SerializeField] private string _mainMenuName = "Main Menu";

    private bool _paused =  false;

    // Update is called once per frame
    void Update()
    {
        UpdateHealth();
        UpdateHeat();
        UpdateSpeed();

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
        _healthBar.value = PlayerData.Instance.Health;
    }

    void UpdateHeat()
    {
        _heatText.text = "Heat:" + PlayerData.Instance.Heat.ToString("F1");
        _heatBar.value = PlayerData.Instance.Heat;
    }

    void UpdateSpeed()
    {
        _speedText.text = "Speed: " + PlayerData.Instance.Speed.ToString();
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

    public void Controls()
    {

    }

    public void ToMainMenu()
    {
        Time.timeScale = 1f;
        SceneController.Instance.SwitchScene(_mainMenuName);

    }
}
