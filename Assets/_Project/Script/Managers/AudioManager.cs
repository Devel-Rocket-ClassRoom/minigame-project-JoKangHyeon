using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer audioMixer;

    float _bgmVolume = 1.0f;
    float _seVolume = 1.0f;
    float _masterVolume = 1.0f;

    public float BGMVolume
    {
        get
        {
            return _bgmVolume;
        }
        set
        {
            _bgmVolume = value;
            float db = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
            audioMixer.SetFloat(c_bgmVolumeKey, db);
        }
    }

    public float SEVolume
    {
        get
        {
            return _seVolume;
        }
        set
        {
            _seVolume = value;
            float db = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
            audioMixer.SetFloat(c_seVolumeKey, db);
        }
    }

    public float MasterVolume
    {
        get
        {
            return _masterVolume;
        }
        set
        {
            _masterVolume = value;
            float db = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
            audioMixer.SetFloat(c_masterVolumeKey, db);
        }
    }

    private const string c_bgmVolumeKey = "BGMVolume";
    private const string c_seVolumeKey = "SEVolume";
    private const string c_masterVolumeKey = "MasterVolume";

    void Start()
    {
        LoadConfig();
    }

    public void LoadConfig()
    {

        if (!PlayerPrefs.HasKey(c_bgmVolumeKey) || !PlayerPrefs.HasKey(c_seVolumeKey))
        {
            MasterVolume = 0.5f;
            SEVolume = 0.5f;
            BGMVolume = 0.5f;
            SaveConfig();
            return;
        }

        BGMVolume = PlayerPrefs.GetFloat(c_bgmVolumeKey);
        SEVolume = PlayerPrefs.GetFloat(c_seVolumeKey);
        MasterVolume = PlayerPrefs.GetFloat(c_masterVolumeKey);
    }

    public void SaveConfig()
    {
        PlayerPrefs.SetFloat(c_bgmVolumeKey, BGMVolume);
        PlayerPrefs.SetFloat(c_seVolumeKey, SEVolume);
        PlayerPrefs.SetFloat(c_masterVolumeKey, MasterVolume);
    }
}
