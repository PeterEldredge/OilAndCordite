using UnityEngine;
using UnityEditor;
using System.IO;

[System.Serializable]
public class SavedLevelData
{
    public BaseScoring.Rank BestMedal = BaseScoring.Rank.None;
    public int HighScore = 0;
    public int BestTime = 999;
}

[CreateAssetMenu(menuName = "Level")]
public class Level : ScriptableObject
{
    public const int NUMBER_OF_RANKS = 4;

    private string _filePath => Path.Combine(Application.persistentDataPath, "Levels", _levelName + ".lvl");

    public enum LevelType
    {
        DestroyEnemies,
        RaceToTheFinish,
    }

    #region Data

    [SerializeField] private string _levelName;
    public string LevelName => _levelName;

    [SerializeField] private string _sceneName;
    public string SceneName => _sceneName;

    [SerializeField] private LevelType _type;
    public LevelType Type => _type;

    [SerializeField] private Sprite _levelSprite;
    public Sprite LevelSprite => _levelSprite;

    [SerializeField, TextArea(4, 8)] private string _levelDescription;
    public string LevelDescription => _levelDescription;

    [SerializeField, TextArea(4, 8)] private string _levelNotificationText;
    public string LevelNotificationText => _levelNotificationText;

    [SerializeField] private float _parTime;
    public float ParTime => _parTime;

    [SerializeField] private int[] _scoreRequirements = new int[NUMBER_OF_RANKS];
    public int[] ScoreRequirements => _scoreRequirements;

    #endregion

    #region Saved Data

    [Space, Header("Saved Data")]

    [SerializeField] private BaseScoring.Rank _bestMedal = BaseScoring.Rank.None;
    public BaseScoring.Rank BestMedal => _bestMedal;

    [SerializeField] private int _highScore;
    public int HighScore
    {
        get
        {
            return _highScore;
        }
        set
        {
            if (value > _highScore)
            {
                UpdateMedal(value);

                _highScore = value;

                Save();
            }
        }
    }

    [SerializeField] private int _bestTime;
    public int BestTime
    {
        get
        {
            return _bestTime;
        }
        set
        {
            if (value < _bestTime)
            {
                _bestTime = value;

                Save();
            }
        }
    }

    #endregion

    private void UpdateMedal(int score)
    {
        if (score > _scoreRequirements[(int)BaseScoring.Rank.Emerald]) _bestMedal = BaseScoring.Rank.Emerald;
        else if (score > _scoreRequirements[(int)BaseScoring.Rank.Gold]) _bestMedal = BaseScoring.Rank.Gold;
        else if (score > _scoreRequirements[(int)BaseScoring.Rank.Silver]) _bestMedal = BaseScoring.Rank.Silver;
        else if (score > _scoreRequirements[(int)BaseScoring.Rank.Bronze]) _bestMedal = BaseScoring.Rank.Bronze;
        else _bestMedal = BaseScoring.Rank.None;
    }

    public void Reset()
    {
        SavedLevelData emptyData = new SavedLevelData();

        _bestMedal = emptyData.BestMedal;
        _highScore = emptyData.HighScore;
        _bestTime = 999;

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif

        Save();
    }

    public void Save()
    {
        if(!Directory.Exists(Path.Combine(Application.persistentDataPath, "Levels")))
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Levels"));
        }

        SavedLevelData data = new SavedLevelData()
        {
            BestMedal = _bestMedal,
            HighScore = _highScore,
            BestTime = _bestTime,
        };

        File.WriteAllText(_filePath, JsonUtility.ToJson(data));

        Debug.Log(_levelName + " Saved: " + _filePath);
    }

    public void Load()
    {
        if (!File.Exists(_filePath)) Save();

        string json = File.ReadAllText(_filePath);

        SavedLevelData savedData = JsonUtility.FromJson<SavedLevelData>(json);

        _bestMedal = savedData.BestMedal;
        _highScore = savedData.HighScore;
        _bestTime = savedData.BestTime;
    }
}
