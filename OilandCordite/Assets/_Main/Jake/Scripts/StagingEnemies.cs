using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagingEnemies : MonoBehaviour
{
    [SerializeField] private float _noiseTimeOffset = .5f;
    [SerializeField] private float _checkRate = .5f;
    [SerializeField] GameObject _enemyGroup;

    private AudioCuePlayer _acp;

    private void Awake()
    {
        _acp = GetComponent<AudioCuePlayer>();
        StartCoroutine(CheckEnemies());
    }

    private IEnumerator CheckEnemies()
    {
        while (true)
        {
            if (_enemyGroup.transform.childCount == 0)
            {
                _acp.PlayRandomSound("ShieldDown");
                yield return new WaitForSeconds(_noiseTimeOffset);
                gameObject.SetActive(false);
                yield return null;
            }
            yield return new WaitForSeconds(_checkRate);
        }
    }
}
