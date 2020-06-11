using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Image _levelImage;
    [SerializeField] private Image _medalImage;
    [SerializeField] private TextMeshProUGUI _levelDescription;
    [SerializeField] private TextMeshProUGUI _missionObjective;
    [SerializeField] private TextMeshProUGUI _rankRequirements;

    [SerializeField] private List<Sprite> _medals;

    [SerializeField] private List<Level> _levels;

    //Mission Type Strings
    private const string _DESTROY_ENEMIES_MISSION = "Mission Type: Destroy All Enemies";
    private const string _RACE_TO_THE_FINISH_MISSION = "Mission Type: Race To The Finish";

    private int _currentLevelIndex = 0;
    private Level _currentLevel;

    private void Start()
    {
        UpdateCurrentLevel(_currentLevelIndex);
    }

    private void UpdateCurrentLevel(int level)
    {
        int tempIndex = level;

        if (level < 0 || level >= _levels.Count) return;

        _currentLevelIndex = tempIndex;
        _currentLevel = _levels[_currentLevelIndex];

        UpdateUI();
    }

    private void UpdateUI()
    {
        _levelImage.sprite = _currentLevel.LevelSprite;
        _levelDescription.text = _currentLevel.LevelDescription;

        switch(_currentLevel.Type)
        {
            case Level.LevelType.DestroyEnemies:
                _missionObjective.text = _DESTROY_ENEMIES_MISSION;
                break;
            case Level.LevelType.RaceToTheFinish:
                _missionObjective.text = _RACE_TO_THE_FINISH_MISSION;
                break;
        }

        //FIX LATER
        if ((int)_currentLevel.BestMedal < _medals.Count)
        {
            _medalImage.enabled = true;
            _medalImage.sprite = _medals[(int)_currentLevel.BestMedal];
        }
        else _medalImage.enabled = false;

        //Update Later
        _rankRequirements.text =
            $"High Score - {_currentLevel.HighScore} {System.Environment.NewLine} {System.Environment.NewLine}" +
            $"Par Time - {_currentLevel.ParTime} seconds {System.Environment.NewLine} {System.Environment.NewLine}" +
            $"Emerald - {_currentLevel.ScoreRequirements[0]}{System.Environment.NewLine}" +
            $"Gold - {_currentLevel.ScoreRequirements[1]}{System.Environment.NewLine}" +
            $"Silver - {_currentLevel.ScoreRequirements[2]}{System.Environment.NewLine}" +
            $"Bronze - {_currentLevel.ScoreRequirements[3]}{System.Environment.NewLine}";
    }

    public void IncrementLevel() => UpdateCurrentLevel(_currentLevelIndex + 1);
    public void DecrementLevel() => UpdateCurrentLevel(_currentLevelIndex - 1);

    public void LoadLevel()
    {
        SceneController.SwitchScene(_currentLevel.SceneName);
    }

    //DEBUG

    [ContextMenu("RESET")]
    public void Reset()
    {
        foreach(Level level in _levels)
        {
            level.Reset();
        }
    }
}
