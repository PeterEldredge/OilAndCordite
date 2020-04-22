using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    private void Awake()
    {
        QualitySettings.vSyncCount = 1;

        Screen.SetResolution(1920, 1080, true);
    }
}
