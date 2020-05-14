using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Image _levelImage;

    [SerializeField] private List<Level> _levels;

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
    }

    public void IncrementLevel() => UpdateCurrentLevel(_currentLevelIndex + 1);
    public void DecrementLevel() => UpdateCurrentLevel(_currentLevelIndex - 1);

    public void LoadLevel()
    {
        SceneController.Instance.SwitchScene(_currentLevel.SceneName);
    }
}
