using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfigPanel : MonoBehaviour
{
    public enum ScreenMode
    {
        WindowedFullScreen=0,
        FullScreen,
        Windowed
    }

    public enum ResolutionMode
    {
        UHD=0,
        QHD,
        FHD,
        HDP,
        HD
    }

    public enum FrameLimitMode
    {
        VSYNC=0,
        F240,
        F144,
        F60,
        None
    }

    public List<string> screenModeKeys;

    public TMP_Dropdown screenModeDropdown;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown framelimitDropdown;

    public Slider masterVolumeSlider;
    public Slider bgmVolumeSlider;
    public Slider seVolumeSlider;

    List<Resolution> resolutions;
    GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        resolutionDropdown.ClearOptions();

        var options = Screen.resolutions.Select((res, index) => {
            return $"{res.width} x {res.height} / {res.refreshRateRatio}hz";
        }).ToList();
        options.Reverse();

        resolutionDropdown.AddOptions(options);
    }

    public void Refresh(GameManager gameManager)
    {
        this.gameManager = gameManager;

        screenModeDropdown.value = (int)gameManager.screenMode;
        screenModeDropdown.RefreshShownValue();

        Resolution current = gameManager.currentResolution;
        int resolutionIndex = Screen.resolutions.ToList().IndexOf(current);
        resolutionDropdown.value = resolutionIndex == -1?0: resolutionIndex;
        resolutionDropdown.RefreshShownValue();

        framelimitDropdown.value = (int)gameManager.frameLimitMode;
        framelimitDropdown.RefreshShownValue();

        masterVolumeSlider.value = gameManager.audioManager.MasterVolume;
        bgmVolumeSlider.value = gameManager.audioManager.BGMVolume;
        seVolumeSlider.value = gameManager.audioManager.SEVolume;
    }

    public void OnScreenModeChanged(int mode)
    {
        gameManager.SetScreenMode((ScreenMode)mode);
    }

    public void OnResolutionChanged(int mode)
    {
        Resolution resolution = Screen.resolutions[Screen.resolutions.Length - mode - 1];
        gameManager.SetResolutionMode(resolution);
    }

    public void OnFrameLimitChanged(int mode)
    {
        gameManager.SetFrameLimitMode((FrameLimitMode)mode);
    }

    public void OnMasterVolumeChanged(float value)
    {
        gameManager.audioManager.MasterVolume = value;
    }

    public void OnSEVolumeChanged(float value)
    {
        gameManager.audioManager.SEVolume = value;
    }

    public void OnBGMVolumeChanged(float value)
    {
        gameManager.audioManager.BGMVolume = value;
    }

    public void OnConfigCancel()
    {
        gameManager.HideConfig();
        gameManager.LoadConfig();
        gameManager.audioManager.LoadConfig();
    }

    public void OnConfigSave()
    {
        gameManager.HideConfig();
        gameManager.SaveConfig();
        gameManager.audioManager.SaveConfig();
    }
}
