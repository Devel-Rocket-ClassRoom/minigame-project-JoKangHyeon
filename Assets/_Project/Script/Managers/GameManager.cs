using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [Header("Scriptable Objects")]
    public StartingDefinitionSO startingsDefine;
    public HandDefinitionSO handDefine;
    public DiceDefinitionSO diceDefine;
    public RelicDefinitionSO relicDefine;
    public CardDefinitionSO cardDefine;
    public CardImageDefinitionSO cardImageDefine;
    public ConsumableDefinitionSO consumableDefine;
    public ShopRarityDefinitionSO rarityDefine;
    public SoundDefinitionSO soundDefine;

    [Header("UI")]
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI rerollText;
    public Button rerollButton;
    public Button MenuButton;
    public Button skipButton;
    public TextMeshProUGUI skipText;

    public GameObject diceObjectSelectHandHider;
    public TextMeshProUGUI diceObjectSelectText;


    [Header("Sub Managers")]
    public RollManager rollManager;
    public AudioManager audioManager;

    [Header("Canvases")]
    public Canvas overlayPanel;
    public ShopPanel shopPanel;
    public DiceSelectPanel diceSelectPanel;
    public FaceSelectPanel faceSelectPanel;
    public HandSelectPanel handSelectPanel;
    public ActiveItemPanel activeItemPanel;
    public RelicPanel relicPanel;
    public GameOverCanvas gameOverPanel;
    public ConfigPanel configPanel;


    [Header("View Objects")]
    public List<GameObject> gameStateObjects;
    public List<GameObject> shopStateObjects;
    public GameObject pauseSceen;
    public Tooltip tooltip;

    [Header("Other")]
    public Animator pauseAnimator;
    public AudioSource bgmAudioSource;
    public GameObject DiceSpawnPoint;
    public LayerMask diceMask;
    public int currentStartingIndex = 0;

    //Not Serialized
    public RunState currentRun;
    bool isDiceObjectSelecting = false;
    Action<DiceObject> diceObjactSelectCallback;
    InputAction click;

    //Configs
    public ConfigPanel.ScreenMode screenMode;
    public Resolution currentResolution;
    public ConfigPanel.FrameLimitMode frameLimitMode;

    //Constants
    public const string c_skipCoinTextFormat = "{0:+0;-0;0}C";
    const string c_rerollTextFormat = "{0} / {1}";

    readonly int c_pauseModeAnimationKey = Animator.StringToHash("Mode");
    const string c_roundTextFormatKey = "RoundStatus";
    const string c_screenModePrefKey = "ScreenMode";
    const string c_resolutionWidthPrefKey = "Resoultion_Width";
    const string c_resolutionHeightPrefKey = "Resoultion_Height";
    const string c_resolutionNumPrefKey = "Resolution_Num";
    const string c_resolutionDenPrefKey = "Resolution_Den";

    const string c_frameLimitModePrefKey = "FrameLimit";

    public List<SlotUI> handsUI;
    public SlotUI handUIPrefab;
    public Transform handParent;

    private void Awake()
    {
        rollManager = GetComponent<RollManager>();
        click = InputSystem.actions.FindAction(Defines.c_inputActionSelect);

        diceObjactSelectCallback = (_) => {
            overlayPanel.gameObject.SetActive(true);
            diceObjectSelectHandHider.SetActive(false);
        };
    }

    public List<int> demoScoreCut = new()
    {
        45,55,60,65,
        70,75,95,105,
        115,130,150,180,
        450,540,640
    };



    void Start()
    {
        faceSelectPanel.Init(this);
        LoadConfig();
        RestartGame();
    }

    public void LoadConfig()
    {
        audioManager.LoadConfig();
        SetScreenMode((ConfigPanel.ScreenMode)PlayerPrefs.GetInt(c_screenModePrefKey, 0));
        SetFrameLimitMode((ConfigPanel.FrameLimitMode)PlayerPrefs.GetInt(c_frameLimitModePrefKey, 0));
        LoadResoluton();
    }

    public void SaveConfig()
    {
        PlayerPrefs.SetInt(c_screenModePrefKey, (int)screenMode);
        PlayerPrefs.SetInt(c_frameLimitModePrefKey , (int)frameLimitMode);
        SaveResolution();
    }

    public void SetScreenMode(ConfigPanel.ScreenMode screenMode)
    {
        this.screenMode = screenMode;
        switch (screenMode)
        {
            case ConfigPanel.ScreenMode.WindowedFullScreen:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case ConfigPanel.ScreenMode.FullScreen:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case ConfigPanel.ScreenMode.Windowed:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
        }
    }

    public void LoadResoluton()
    {
        this.currentResolution = new Resolution()
        {
            width = PlayerPrefs.GetInt(c_resolutionWidthPrefKey, 1920),
            height = PlayerPrefs.GetInt(c_resolutionHeightPrefKey, 1080),
            refreshRateRatio = new RefreshRate()
            {
                numerator = (uint)PlayerPrefs.GetInt(c_resolutionNumPrefKey, 60),
                denominator = (uint)PlayerPrefs.GetInt(c_resolutionDenPrefKey, 100),
            }
        };

        SetResolutionMode(currentResolution);
    }

    public void SaveResolution()
    {
        PlayerPrefs.SetInt(c_resolutionWidthPrefKey, currentResolution.width);
        PlayerPrefs.SetInt(c_resolutionHeightPrefKey, currentResolution.height);
        PlayerPrefs.SetInt(c_resolutionNumPrefKey, (int)currentResolution.refreshRateRatio.numerator);
        PlayerPrefs.SetInt(c_resolutionDenPrefKey, (int)currentResolution.refreshRateRatio.denominator);
    }

    public void SetResolutionMode(Resolution resolution)
    {
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, resolution.refreshRateRatio);
    }

    public void SetFrameLimitMode(ConfigPanel.FrameLimitMode frameLimitMode)
    {
        switch (frameLimitMode)
        {
            case ConfigPanel.FrameLimitMode.VSYNC:
                QualitySettings.vSyncCount = 1;
                break;
            case ConfigPanel.FrameLimitMode.F240:
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 240;
                break;
            case ConfigPanel.FrameLimitMode.F144:
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 144;
                break;
            case ConfigPanel.FrameLimitMode.F60:
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 60;
                break;
            case ConfigPanel.FrameLimitMode.None:
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = -1;
                break;
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (click.WasPerformedThisFrame())
        {
            Vector2 pos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(pos);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, diceMask))
            {
                if (hit.collider.CompareTag(Defines.c_tagDice))
                {
                    if(isDiceObjectSelecting)
                    {
                        diceObjactSelectCallback(hit.collider.GetComponent<DiceObject>());
                        isDiceObjectSelecting = false;
                        diceObjactSelectCallback = (_) => { 
                            overlayPanel.gameObject.SetActive(true);
                            diceObjectSelectHandHider.SetActive(false);
                        };
                    }
                    else
                    {
                        if(!diceSelectPanel.gameObject.activeSelf)
                            currentRun.currentRound.currentCycle.ToggleDice(hit.collider.GetComponent<DiceObject>());
                    }
                }
            }
        }



        //TODO : 키 옮기고 디버그 삭제
        #region DEBUG
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            if (isDiceObjectSelecting)
                return;

            currentRun.currentRound.currentCycle.Reroll();
            RefreshUI();
        }

        //디버그용 치트
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            currentRun.currentScore += 100;
            RefreshUI();
        }
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            currentRun.Coin += 100;
            RefreshUI();
        }

        #endregion
    }

    public void Reroll()
    {
        currentRun.currentRound.currentCycle.Reroll();
        RefreshUI();
    }

    public void SetDice(int pos)
    {
        currentRun.currentRound.currentCycle.SetDice(pos);
        RefreshUI();
    }

    public void RetreveDice(int pos)
    {
        currentRun.currentRound.currentCycle.RetreveDice(pos);
        RefreshUI();
    }

    public void SetHand(int pos)
    {
        currentRun.currentRound.currentCycle.EndCycle(pos);
        RefreshUI();
    }

    public void RestartGame()
    {
        gameOverPanel.gameObject.SetActive(false);

        foreach (var hand in handsUI)
        {
            Destroy(hand.gameObject);
        }
        handsUI.Clear();

        EventBus.Clear();


        currentRun = new(this);
        currentRun.Setup(startingsDefine.startings[currentStartingIndex]);
        EventBus.Subscribe<object>(EventType.OnGameOver, OnGameOver,Defines.c_maxPriority);
        EventBus.Subscribe<object>(EventType.OnGameClear, OnGameClear, Defines.c_maxPriority);

        currentRun.RoundStart();
        RefreshUI();
    }

    
    public void RefreshUI()
    {
        rerollText.text = string.Format(c_rerollTextFormat, currentRun.currentRound.currentCycle.reroll.ToString(), currentRun.rerollPerCycle.ToString());
        coinText.text = currentRun.Coin.ToString();

        roundText.text = string.Format(StringTable.GetString(c_roundTextFormatKey), currentRun.currentScore, currentRun.TargetScore);

        if(currentRun.currentScore >= currentRun.TargetScore)
        {
            roundText.color = Defines.colorGold;
        }
        else
        {
            roundText.color = Defines.colorPaper;
        }

        for (int i = 0; i < currentRun.currentRound.hands.Count; i++)
        {
            if (i >= handsUI.Count)
            {
                var newHand = Instantiate(handUIPrefab, handParent);
                handsUI.Add(newHand);
                int index = i;
                newHand.setButton.onClick.AddListener(() => { SetHand(index); });
                newHand.Init(this);
            }
            handsUI[i].Refresh(currentRun.currentRound.hands[i], currentRun.currentRound.currentCycle.dicesSetted);
        }

        if (currentRun.IsSkipable())
        {
            skipButton.interactable = true;
        }
        else
        {
            skipButton.interactable = false;
        }
        skipText.text = string.Format(c_skipCoinTextFormat, currentRun.GetSkipGold());

    }



    public void OnGameOver(object _)
    {
        gameOverPanel.Show(this, false);
    }

    public void OnGameClear(object _)
    {
        gameOverPanel.Show(this, true);
    }


    public void StartDiceObjectSelect(Action<DiceObject> callback)
    {
        overlayPanel.gameObject.SetActive(false);
        diceObjectSelectHandHider.SetActive(true);
        isDiceObjectSelecting = true;
        diceObjactSelectCallback +=callback;
    }

    public void StartDiceSelect(Action<Dice> callback)
    {
        diceSelectPanel.StartDiceSelect(this, callback);
    }

    public void StartDiceFaceSelect(Dice target, Action<Dice,int> callback)
    {
        faceSelectPanel.StartFaceSelect(target, callback);
    }

    public void StartHandSelect(Action<HandSlot> callback, Func<HandSlot, bool> filter = null, bool isRound = false)
    {
        handSelectPanel.StartHandSelect(this, callback, filter, isRound);
    }

    public void RerollButton()
    {
        currentRun.currentRound.currentCycle.Reroll();
    }

    public void ShowShop(ShopState state)
    {
        foreach(var gameobject in gameStateObjects)
        {
            gameobject.gameObject.SetActive(false);
        }

        foreach(var gameobject in shopStateObjects)
        {
            gameobject.gameObject.SetActive(true);
        }

        bgmAudioSource.clip = soundDefine.Find(Defines.c_shopBGMKey);
        bgmAudioSource.Play();
        tooltip.HideTooltip();
        shopPanel.Init(this, state);
    }

    public void ShowGame()
    {
        foreach (var gameobject in shopStateObjects)
        {
            gameobject.gameObject.SetActive(false);
        }

        foreach (var gameobject in gameStateObjects)
        {
            gameobject.gameObject.SetActive(true);
        }

        tooltip.HideTooltip();
        currentRun.RoundStart();
    }

    public void UpdateCoin(int coin)
    {
        coinText.text = coin.ToString();
    }

    public void Skip()
    {
        currentRun.Skip();
    }

    public void PauseGame()
    {
        pauseSceen.SetActive(true);
        pauseAnimator.SetInteger(c_pauseModeAnimationKey, 1);
        tooltip.HideTooltip();
    }

    public void ResumeGame()
    {
        pauseAnimator.SetInteger(c_pauseModeAnimationKey, 0);
        pauseSceen.SetActive(false);
    }

    public void ShowConfig()
    {
        configPanel.Refresh(this);
        pauseAnimator.SetInteger(c_pauseModeAnimationKey, 2);
    }

    public void HideConfig()
    {
        pauseAnimator.SetInteger(c_pauseModeAnimationKey, 1);
    }

    public void ExitGame()
    {
#if !UNITY_EDITOR
        Application.Quit();
#endif
    }

    public void ShowDiceView()
    {
        StartDiceSelect((_)=>{ }); 
    }

    public void PlayBGM(int stage)
    {
        AudioClip clip = soundDefine.Find(stage.ToString());
        if (clip != null)
        {
            bgmAudioSource.clip = clip;
            bgmAudioSource.Play();
        }
    }
}
