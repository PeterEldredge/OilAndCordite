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
    public bool InvertYOn = false;

    public int MusicVolume = 10;
    public int SFXVolume = 10;
}

public class Settings : GameEventUserObject
{
    private static string _CONFIG_PATH => Path.Combine(Application.persistentDataPath, "config_new.cfg");

    public static Settings Instance;

    //Accessable Settings
    public int FramerateCap { get; private set; }
    public int VSyncCount { get; private set; }

    public bool MotionBlur { get; private set; }
    public bool InvertY { get; private set; }

    public int MusicVolume { get; private set; }
    public int SFXVolume { get; private set; }

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
    private void SetInvertY(Events.InvertYToggledEventArgs args) => InvertY = args.InvertY;
    private void SetMusicVolume(Events.MusicVolumeChangedEventArgs args) => MusicVolume = args.MusicVolume;
    private void SetSFXVolume(Events.SFXVolumeChangedEventArgs args) => SFXVolume = args.SFXVolume;

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.FramerateChangedEventArgs>(this, SetFramerateCap);
        EventManager.Instance.AddListener<Events.VSyncChangedEventArgs>(this, SetVSyncCount);
        EventManager.Instance.AddListener<Events.MotionBlurToggledEventArgs>(this, SetMotionBlur);
        EventManager.Instance.AddListener<Events.InvertYToggledEventArgs>(this, SetInvertY);
        EventManager.Instance.AddListener<Events.MusicVolumeChangedEventArgs>(this, SetMusicVolume);
        EventManager.Instance.AddListener<Events.SFXVolumeChangedEventArgs>(this, SetSFXVolume);
    }

    public override void Unsubscribe()
    {
        EventManager.Instance.RemoveListener<Events.FramerateChangedEventArgs>(this, SetFramerateCap);
        EventManager.Instance.RemoveListener<Events.VSyncChangedEventArgs>(this, SetVSyncCount);
        EventManager.Instance.RemoveListener<Events.MotionBlurToggledEventArgs>(this, SetMotionBlur);
        EventManager.Instance.RemoveListener<Events.InvertYToggledEventArgs>(this, SetInvertY);
        EventManager.Instance.RemoveListener<Events.MusicVolumeChangedEventArgs>(this, SetMusicVolume);
        EventManager.Instance.RemoveListener<Events.SFXVolumeChangedEventArgs>(this, SetSFXVolume);
    }

    private void SetDefaults()
    {
        FramerateCap = 60;
        VSyncCount = 1;
        MotionBlur = true;
        InvertY = false;
        MusicVolume = 10;
        SFXVolume = 10;
    }

    public void SaveCurrentSettings()
    {
        SaveableSettings settings = new SaveableSettings()
        {
            VSyncCount = VSyncCount,
            FramerateCap = FramerateCap,
            MotionBlurOn = MotionBlur,
            InvertYOn = InvertY,

            ResolutionWidth = Screen.currentResolution.width,
            ResolutionHeight = Screen.currentResolution.height,
            ResolutionRefresh = Screen.currentResolution.refreshRate,

            MusicVolume = MusicVolume,
            SFXVolume = SFXVolume,
        };

        File.WriteAllText(_CONFIG_PATH, JsonUtility.ToJson(settings));

        Debug.Log("Settings Saved: " + _CONFIG_PATH);

        LoadSavedSettings();
    }

    public void SaveCurrentSettingsOnDelay()
    {
        Invoke("SaveCurrentSettings", .2f);
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
        Screen.SetResolution(settings.ResolutionWidth, settings.ResolutionHeight, true, settings.ResolutionRefresh);

        //Framerate
        QualitySettings.vSyncCount = settings.VSyncCount;
        Application.targetFrameRate = settings.FramerateCap;
        Time.fixedDeltaTime = 1f / settings.FramerateCap;

        //Accessable Settings
        FramerateCap = Application.targetFrameRate;
        VSyncCount = QualitySettings.vSyncCount;
        MotionBlur = settings.MotionBlurOn;
        InvertY = settings.InvertYOn;

        MusicVolume = settings.MusicVolume;
        SFXVolume = settings.SFXVolume;

        EventManager.Instance.TriggerEvent(new Events.RefreshSettingsUIArgs());
    }

    public void ResetSettings()
    {
        File.Delete(_CONFIG_PATH);

        SetDefaults();

        SaveCurrentSettings();
    }
}
