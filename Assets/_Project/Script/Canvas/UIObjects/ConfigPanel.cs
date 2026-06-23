using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public TMP_Text nicknameText;
    public TMP_InputField nicknameEditField;

    List<Resolution> resolutions;
    GameManager gameManager;



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

        nicknameEditField.text = gameManager.personalData.NickName;
        nicknameText.text = $"{gameManager.personalData.NickName}으로 로그인됨";
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

    public void OnNicknameChange()
    {
        if (string.IsNullOrEmpty(nicknameEditField.text.Trim()))
        {
            return;
        }

        gameManager.personalData.NickName = nicknameEditField.text;
        if (gameManager.personalData.IsDirty)
            gameManager.UpdateServerData();
    }

    public void SignOut()
    {
        FirebaseAuthManager.Instance.SignOut();
        SceneManager.LoadScene(0);
    }
}
