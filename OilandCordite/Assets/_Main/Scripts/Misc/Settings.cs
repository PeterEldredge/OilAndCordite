using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Events
{
    public struct RefreshSettingsUIArgs : IGameEvent { }
}

[System.Serializable]
public class SaveableSettings
{
    public int VSyncCount = 1;
    public int FramerateCap = 60;

    public int ResolutionWidth;
    public int ResolutionHeight;
    public int ResolutionRefresh;

    public bool MotionBlurOn = true;
}

public class Settings : GameEventUserObject
{
    private static string _CONFIG_PATH => Path.Combine(Application.persistentDataPath, "config.cfg");

    public static Settings Instance;

    //Accessable Settings
    public int FramerateCap { get; private set; }
    public int VSyncCount { get; private set; }

    public bool MotionBlur { get; private set; }

    private void Awake()
    {
        if(!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }

        DontDestroyOnLoad(gameObject);

        SetDefaults();

        LoadSavedSettings();

        //Audio
        AudioListener.pause = false;
    }

    private void SetFramerateCap(Events.FramerateChangedEventArgs args) => FramerateCap = args.FramerateCap;
    private void SetVSyncCount(Events.VSyncChangedEventArgs args) => VSyncCount = args.VSyncCount;
    private void SetMotionBlur(Events.MotionBlurToggledEventArgs args) => MotionBlur = args.MotionBlur;

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.FramerateChangedEventArgs>(this, SetFramerateCap);
        EventManager.Instance.AddListener<Events.VSyncChangedEventArgs>(this, SetVSyncCount);
        EventManager.Instance.AddListener<Events.MotionBlurToggledEventArgs>(this, SetMotionBlur);
    }

    public override void Unsubscribe()
    {
        EventManager.Instance.RemoveListener<Events.FramerateChangedEventArgs>(this, SetFramerateCap);
        EventManager.Instance.RemoveListener<Events.VSyncChangedEventArgs>(this, SetVSyncCount);
        EventManager.Instance.RemoveListener<Events.MotionBlurToggledEventArgs>(this, SetMotionBlur);
    }

    private void SetDefaults()
    {
        FramerateCap = 60;
        VSyncCount = 1;
        MotionBlur = true;
    }

    public void SaveCurrentSettings()
    {
        SaveableSettings settings = new SaveableSettings()
        {
            VSyncCount = VSyncCount,
            FramerateCap = FramerateCap,
            MotionBlurOn = MotionBlur,

            ResolutionWidth = Screen.currentResolution.width,
            ResolutionHeight = Screen.currentResolution.height,
            ResolutionRefresh = Screen.currentResolution.refreshRate,
        };

        File.WriteAllText(_CONFIG_PATH, JsonUtility.ToJson(settings));

        Debug.Log("Settings Saved: " + _CONFIG_PATH);

        LoadSavedSettings();
    }

    public void LoadSavedSettings()
    {
        if (!File.Exists(_CONFIG_PATH))
        {
            SaveCurrentSettings();
            
            return;
        }

        string json = File.ReadAllText(_CONFIG_PATH);

        SaveableSettings settings = JsonUtility.FromJson<SaveableSettings>(json);

        //Resolution
        Screen.SetResolution(settings.ResolutionWidth, settings.ResolutionHeight, true);

        //Framerate
        QualitySettings.vSyncCount = settings.VSyncCount;
        Application.targetFrameRate = settings.FramerateCap;
        Time.fixedDeltaTime = 1f / settings.FramerateCap;

        //Accessable Settings
        FramerateCap = Application.targetFrameRate;
        VSyncCount = QualitySettings.vSyncCount;
        MotionBlur = settings.MotionBlurOn;

        EventManager.Instance.TriggerEvent(new Events.RefreshSettingsUIArgs());
    }

    public void ResetSettings()
    {
        File.Delete(_CONFIG_PATH);

        SetDefaults();

        SaveCurrentSettings();
    }
}
