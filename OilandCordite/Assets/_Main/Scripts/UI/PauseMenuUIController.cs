using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenuUIController : BaseUIController
{
    [SerializeField] private GameObject _pauseMenuUI;
    [SerializeField] private GameObject _pauseMenuDefaultSelected;

    #region Events

    protected override void OnGamePaused(Events.GamePausedEventArgs args)
    {
        _pauseMenuUI.SetActive(true);

        EventSystem.current.SetSelectedGameObject(_pauseMenuUI);
        EventSystem.current.SetSelectedGameObject(_pauseMenuDefaultSelected);
    }

    protected override void OnGameUnpaused(Events.GameUnpausedEventArgs args)
    {
        _pauseMenuUI.SetActive(false);
    }

    #endregion

    #region Button Actions

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

    #endregion
}
