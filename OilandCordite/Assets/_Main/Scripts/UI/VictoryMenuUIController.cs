using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VictoryMenuUIController : BaseUIController
{
    [SerializeField] private GameObject _victoryMenuUI;
    [SerializeField] private GameObject _victoryMenuUIDefaultSelected;

    #region Events

    protected override void OnMissionCompleted(Events.MissionCompleteEventArgs args) => StartCoroutine(ActionOnDelay(.5f, () => OpenVictoryScreen()));

    public void OpenVictoryScreen()
    {
        Time.timeScale = 0f;

        _victoryMenuUI.SetActive(true);

        CursorSetter.UnlockCursor();

        EventSystem.current.SetSelectedGameObject(_victoryMenuUIDefaultSelected);
    }

    #endregion

}
