using UnityEngine;
using UnityEngine.EventSystems;

public class ControlsMenuUIController : BaseUIController
{
    [SerializeField] private GameObject _controlsUI;
    [SerializeField] private GameObject _controlsUIDefaultSelected;

    public void OpenControlsScreen()
    {
        _controlsUI.SetActive(true);

        EventSystem.current.SetSelectedGameObject(_controlsUIDefaultSelected);

        EventManager.Instance.TriggerEventImmediate(new Events.UIInteractionEventArgs());
    }
}
