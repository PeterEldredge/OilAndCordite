using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveableSettings
{
    public int VSyncCount = 1;
    public int FramerateCap = 60;

    public int ResolutionWidth;
    public int ResolutionHeight;
    public int ResolutionRefresh;
}

public class Settings : MonoBehaviour
{
    private string _filePath => Path.Combine(Application.persistentDataPath, "config.cfg");

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        LoadSavedSettings();

        //Audio
        AudioListener.pause = false;
    }

    public void SaveCurrentSettings()
    {
        SaveableSettings settings = new SaveableSettings()
        {
            VSyncCount = QualitySettings.vSyncCount,
            FramerateCap = Application.targetFrameRate,
            ResolutionWidth = Screen.currentResolution.width,
            ResolutionHeight = Screen.currentResolution.height,
            ResolutionRefresh = Screen.currentResolution.refreshRate,
        };

        File.WriteAllText(_filePath, JsonUtility.ToJson(settings));

        Debug.Log("Settings Saved: " + _filePath);
    }

    public void LoadSavedSettings()
    {
        if (!File.Exists(_filePath)) SaveCurrentSettings();

        string json = File.ReadAllText(_filePath);

        SaveableSettings settings = JsonUtility.FromJson<SaveableSettings>(json);

        //Resolution
        Screen.SetResolution(settings.ResolutionWidth, settings.ResolutionHeight, true);

        //Framerate
        Time.fixedDeltaTime = 1f / settings.ResolutionRefresh;
        QualitySettings.vSyncCount = settings.VSyncCount;
        Application.targetFrameRate = settings.FramerateCap;
    }
}
