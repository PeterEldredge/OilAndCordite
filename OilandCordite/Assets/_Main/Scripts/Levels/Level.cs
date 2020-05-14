using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Level")]
public class Level : ScriptableObject
{
    [SerializeField] private string _sceneName;
    public string SceneName => _sceneName;

    [SerializeField] private Sprite _levelSprite;
    public Sprite LevelSprite => _levelSprite;
}
