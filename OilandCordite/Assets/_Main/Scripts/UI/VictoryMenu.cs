using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryMenu : MonoBehaviour
{
    [SerializeField] private Text _scoreText;
    [SerializeField] private Text _rankText;

    private void OnEnable()
    {
        string rank = "None";
        
        //Terrible, replace later 
        switch(MissionControllerData.Instance.MissionController.Rank)
        {
            case BaseScoring.Rank.Emerald:
                rank = "Emerald";
                break;
            case BaseScoring.Rank.Gold:
                rank = "Gold";
                break;
            case BaseScoring.Rank.Silver:
                rank = "Silver";
                break;
            case BaseScoring.Rank.Bronze:
                rank = "Bronze";
                break;
        }

        _scoreText.text = "Score: " + MissionControllerData.Instance.MissionController.Score;
        _rankText.text = "Rank: " + rank;
    }
}
