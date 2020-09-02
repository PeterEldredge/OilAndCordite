using UnityEngine;
using UnityEngine.EventSystems;

public class DeathMenuUIController : BaseUIController
{
    [SerializeField] private GameObject _deathMenuUI;
    [SerializeField] private GameObject _deathMenuUIDefaultSelected;

    #region Events

    protected override void OnPlayerDeath(Events.PlayerDeathEventArgs args) => StartCoroutine(ActionOnDelay(3f, () => OpenDeathScreen()));
    protected override void OnMissionFailed(Events.MissionFailedEventArgs args) => StartCoroutine(ActionOnDelay(3f, () => OpenDeathScreen()));

    public void OpenDeathScreen()
    {
        Time.timeScale = 0f;

        _deathMenuUI.SetActive(true);

        CursorSetter.UnlockCursor();

        EventSystem.current.SetSelectedGameObject(_deathMenuUIDefaultSelected);
    }

    #endregion
}
