using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : GameEventUserObject
{
    private void OnMissionCompleted(MissionCompleteEventArgs args)
    {
        SceneManager.LoadScene(0);        
    }

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<MissionCompleteEventArgs>(this, OnMissionCompleted);
    }
}
