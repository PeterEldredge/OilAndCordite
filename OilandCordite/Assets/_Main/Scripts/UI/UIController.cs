using UnityEngine;

public class UIController : BaseUIController
{
    private bool _paused =  false;
    private bool _dead = false;
    private bool _uiHidden = false;

    private AudioCuePlayer _acp;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _acp = gameObject.GetComponent<AudioCuePlayer>();
        _canvasGroup = gameObject.GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if ((InputHelper.Player.GetButtonDown("Start") || InputHelper.Player.GetButtonDown("UICancel")) && !_dead) 
        {
            if (!_paused) Pause();
            else Resume();
        }  

        if (InputHelper.Player.GetButtonDown("UIToggle"))
        {
            if (!_uiHidden) HideUI();             
            else ShowUI();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;

        _paused = true;

        _acp.PlaySound("Pause");

        AudioListener.pause = true;

        CursorSetter.UnlockCursor();

        EventManager.Instance.TriggerEvent(new Events.GamePausedEventArgs());
    }

    public void Resume()
    {
        Time.timeScale = 1f;

        _paused = false;

        _acp.PlaySound("Menu_Item_Select");

        AudioListener.pause = false;

        CursorSetter.LockCursor();

        EventManager.Instance.TriggerEvent(new Events.GameUnpausedEventArgs());
    }

    #region Events

    protected override void OnPlayerDeath(Events.PlayerDeathEventArgs args) => _dead = true;
    protected override void OnUIInteraction(Events.UIInteractionEventArgs args) => _acp.PlaySound("Menu_Item_Select");

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

    #region Misc

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

    #endregion
}
