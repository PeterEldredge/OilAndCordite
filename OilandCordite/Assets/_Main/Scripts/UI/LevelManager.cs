using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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

    private MapBorder _mapBorder;
    private RightInfoBorder _rightInfoBorder;
    private FadePanel _fadePanel;

    private void Awake()
    {
        _mapBorder = GetComponentInChildren<MapBorder>();
        _rightInfoBorder = GetComponentInChildren<RightInfoBorder>();
        _fadePanel = GetComponentInChildren<FadePanel>();

        LoadLevelData();

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        StartCoroutine(OpenAnims());
    }

    public void UpdateCurrentLevel(Level level)
    {
        if(CurrentLevel != level)
        {
            CurrentLevel = level;

            UpdateUI();

            if(_rightInfoBorder.gameObject.activeSelf) _rightInfoBorder.Open();
        }
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

        //FIX LATER BECAUSE THIS IS NOT GOOD
        if ((int)CurrentLevel.BestMedal < _medals.Count)
        {
            _medalImage.enabled = true;
            _medalImage.sprite = GetMedalImage(CurrentLevel.BestMedal);
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
        StartCoroutine(CloseAnims());
    }

    private IEnumerator OpenAnims()
    {
        _rightInfoBorder.gameObject.SetActive(false);

        _startingLevel.OnClicked();
        UpdateUI();

        yield return new WaitForSeconds(_mapBorder.Open());

        _rightInfoBorder.gameObject.SetActive(true);

        yield return new WaitForSeconds(_rightInfoBorder.Open());
    }

    private IEnumerator CloseAnims()
    {
        float fadeTime = _fadePanel.FadeOut();

        float rightInfoTime = _rightInfoBorder.Close();
        yield return new WaitForSeconds(rightInfoTime);

        _rightInfoBorder.gameObject.SetActive(false);

        float mapBorderTime = _mapBorder.Close();
        yield return new WaitForSeconds(mapBorderTime);

        yield return new WaitForSeconds(Mathf.Max(fadeTime - rightInfoTime - mapBorderTime, 0));

        SceneController.SwitchScene(CurrentLevel.SceneName);
    }

    //HELPERS
    public Sprite GetMedalImage(BaseScoring.Rank rank)
    {
        return _medals[(int) rank];
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
