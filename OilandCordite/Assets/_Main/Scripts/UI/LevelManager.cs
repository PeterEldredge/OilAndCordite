using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    //[SerializeField] private Image _levelImage;
    [SerializeField] private Image _medalImage;

    [SerializeField] private TextMeshProUGUI _levelName;
    [SerializeField] private TextMeshProUGUI _levelDescription;
    [SerializeField] private TextMeshProUGUI _levelNotifications;
    [SerializeField] private TextMeshProUGUI _rankRequirements;
    [SerializeField] private TextMeshProUGUI _parTime;

    [SerializeField] private TextMeshProUGUI _highScore;
    [SerializeField] private TextMeshProUGUI _bestTime;

    [SerializeField] private List<Sprite> _medals;
    [SerializeField] private List<LevelSelector> _levels;

    [SerializeField] private LevelSelector _startingLevel;

    public Level CurrentLevel { get; private set; }

    private void Awake()
    {
        LoadLevelData();
    }

    private void Start()
    {
        _startingLevel.OnClicked();

        UpdateUI();
    }

    public void UpdateCurrentLevel(Level level)
    {
        CurrentLevel = level;

        UpdateUI();
    }

    private void UpdateUI()
    {
        //_levelImage.sprite = _currentLevel.LevelSprite;
        _levelName.text = CurrentLevel.LevelName;
        _levelDescription.text = CurrentLevel.LevelDescription;
        _levelNotifications.text = CurrentLevel.LevelNotificationText;
        
        _parTime.text = $"PAR TIME: {CurrentLevel.ParTime} SEC";
        _highScore.text = $"{CurrentLevel.HighScore}";
        _bestTime.text = $"{CurrentLevel.BestTime}";

        //FIX LATER
        if ((int)CurrentLevel.BestMedal < _medals.Count)
        {
            _medalImage.enabled = true;
            _medalImage.sprite = _medals[(int)CurrentLevel.BestMedal];
        }
        else _medalImage.enabled = false;

        //Update Later
        _rankRequirements.text =
            $"- {CurrentLevel.ScoreRequirements[0]}{System.Environment.NewLine}" +
            $"- {CurrentLevel.ScoreRequirements[1]}{System.Environment.NewLine}" +
            $"- {CurrentLevel.ScoreRequirements[2]}{System.Environment.NewLine}" +
            $"- {CurrentLevel.ScoreRequirements[3]}{System.Environment.NewLine}";
    }

    public void LoadLevel()
    {
        SceneController.SwitchScene(CurrentLevel.SceneName);
    }

    //SAVING

    [ContextMenu("Save Levels")]
    public void SaveLevelData()
    {
        foreach (LevelSelector levelSelector in _levels)
        {
            levelSelector.SelectorLevel.Save();
        }
    }

    [ContextMenu("Load Levels")]
    public void LoadLevelData()
    {
        foreach (LevelSelector levelSelector in _levels)
        {
            levelSelector.SelectorLevel.Load();
        }
    }

    //DEBUG

    [ContextMenu("RESET")]
    public void Reset()
    {
        foreach(LevelSelector levelSelector in _levels)
        {
            levelSelector.SelectorLevel.Reset();
        }
    }
}
