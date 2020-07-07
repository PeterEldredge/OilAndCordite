using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        Time.fixedDeltaTime = 1f / Screen.currentResolution.refreshRate;
        QualitySettings.vSyncCount = 1;

        AudioListener.pause = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V)) QualitySettings.vSyncCount = 1;
        if (Input.GetKeyDown(KeyCode.B)) QualitySettings.vSyncCount = 2;
        
        if (Input.GetKeyDown(KeyCode.N))
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }
    }
}
