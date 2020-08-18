using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VictoryMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _timeText;
    [SerializeField] private Image _rankImage;

    [SerializeField] private List<Sprite> _medals;

    private void OnEnable()
    {
        _scoreText.text = $"{MissionControllerData.Instance.MissionController.Score}";
        _timeText.text = $"{Mathf.RoundToInt(MissionControllerData.Instance.MissionController.Timer)}";

        _rankImage.sprite = GetMedalImage(MissionControllerData.Instance.MissionController.Rank);
    }

    public Sprite GetMedalImage(BaseScoring.Rank rank)
    {
        return _medals[(int)rank];
    }
}
