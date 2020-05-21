using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillSelf : MonoBehaviour
{
    [SerializeField] private float _time;

    private void OnEnable()
    {
        Destroy(gameObject, _time);
    }
}
