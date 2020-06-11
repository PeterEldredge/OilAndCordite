using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Level")]
public class Level : ScriptableObject
{
    public const int NUMBER_OF_RANKS = 4;

    public enum LevelType
    {
        DestroyEnemies,
        RaceToTheFinish,
    }

    [SerializeField] private string _sceneName;
    public string SceneName => _sceneName;

    [SerializeField] private LevelType _type;
    public LevelType Type => _type;

    [SerializeField] private Sprite _levelSprite;
    public Sprite LevelSprite => _levelSprite;

    [SerializeField, TextArea(4, 8)] private string _levelDescription;
    public string LevelDescription => _levelDescription;

    [SerializeField] private float _parTime;
    public float ParTime => _parTime;

    [SerializeField] private int[] _scoreRequirements = new int[NUMBER_OF_RANKS];
    public int[] ScoreRequirements => _scoreRequirements;

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
            }
        }
    }

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
        _bestMedal = BaseScoring.Rank.None;
        _highScore = 0;

        EditorUtility.SetDirty(this);
    }
}
