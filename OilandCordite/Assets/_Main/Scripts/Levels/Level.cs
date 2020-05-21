using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
}
