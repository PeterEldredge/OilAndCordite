using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public bool Closing { get; private set; } = false;

    private MapBorder _optionsBorder;

    private GameObject _previousPanel;

    private void Awake()
    {
        _optionsBorder = GetComponentInChildren<MapBorder>();
    }

    public void Open(GameObject panel)
    {
        _previousPanel = panel;

        _optionsBorder.Open();
    }

    public void Close()
    {
        StartCoroutine(CloseRoutine());
    }

    private IEnumerator CloseRoutine()
    {
        yield return new WaitForSeconds(_optionsBorder.Close());

        _previousPanel.SetActive(true);
        gameObject.SetActive(false);
    }
}
