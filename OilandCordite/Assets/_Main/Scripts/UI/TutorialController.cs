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
    private AudioCuePlayer _acp;
    [SerializeField]private GameObject _textContainer;
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
        _textContainer.SetActive(false);
        CursorSetter.LockCursor();
    }
    //Idea 1, pull up a screen that pauses the game
    void PopUpTutorial(string text)
    {
        Time.timeScale = 0f;
        _acp.PlaySound("Pause");
        _textContainer.SetActive(true);
        _text.text = text;
        CursorSetter.UnlockCursor();
    }
}
