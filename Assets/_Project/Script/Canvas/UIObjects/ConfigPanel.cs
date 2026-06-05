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

    // 주의: 옵션 구성을 Start()에 두면, 패널이 비활성→활성으로 켜질 때
    // Refresh()가 값을 설정한 "뒤에" Start()의 ClearOptions가 값을 0으로 되돌린다.
    // 그래서 옵션 구성은 Refresh()에서만 수행한다.

    // 드롭다운 옵션을 Screen.resolutions(역순) 기준으로 채운다.
    void BuildResolutionOptions()
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

        BuildResolutionOptions();

        screenModeDropdown.SetValueWithoutNotify((int)gameManager.screenMode);
        screenModeDropdown.RefreshShownValue();

        resolutionDropdown.SetValueWithoutNotify(gameManager.currentResolutionIndex);
        resolutionDropdown.RefreshShownValue();

        framelimitDropdown.SetValueWithoutNotify((int)gameManager.frameLimitMode);
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
        gameManager.SetResolutionByIndex(mode);
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
