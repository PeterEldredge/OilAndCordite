using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Events
{
    public struct TutorialArgs : IGameEvent {
        public string Text { get; }
        public TutorialArgs(string text)
        {
            Text = text;
        }
    }
}

public class TutorialController : GameEventUserObject
{
    [SerializeField] private GameObject _textContainer;
    [SerializeField] private GameObject _tutorialWindow;

    [HideInInspector] public bool tutorialOut = false;

    private AudioCuePlayer _acp;
    private TextMeshProUGUI _text;

    private void PopUpTutorial(Events.TutorialArgs args) => PopUpTutorial(args.Text);
    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.TutorialArgs>(this, PopUpTutorial);
    }
    void Start()
    {
        _acp = gameObject.GetComponent<AudioCuePlayer>();
        _text = _textContainer.GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        if (InputHelper.Player.GetButtonDown("UIConfirm")&&_textContainer.activeInHierarchy){ 
            Continue();
        }
    }
    void Continue(){
        Time.timeScale = 1f;
        _acp.PlaySound("Menu_Item_Select");
        tutorialOut = false;
        _textContainer.SetActive(false);
        _tutorialWindow.SetActive(false);
        CursorSetter.LockCursor();
    }
    //Idea 1, pull up a screen that pauses the game
    void PopUpTutorial(string text)
    {
        Time.timeScale = 0f;
        _acp.PlaySound("Pause");
        tutorialOut = true;
        _textContainer.SetActive(true);
        _tutorialWindow.SetActive(true);
        _text.text = text;
        CursorSetter.UnlockCursor();
    }
}
