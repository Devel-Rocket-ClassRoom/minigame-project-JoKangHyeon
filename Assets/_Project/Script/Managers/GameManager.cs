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
    public GameObject TutorialPanel;


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
    // 드롭다운(=Screen.resolutions 역순) 기준 인덱스. 표시/저장의 단일 기준값.
    public int currentResolutionIndex;
    public ConfigPanel.FrameLimitMode frameLimitMode;

    //Constants
    public const string c_skipCoinTextFormat = "{0:+0;-0;0}C";
    const string c_rerollTextFormat = "{0} / {1}";

    readonly int c_pauseModeAnimationKey = Animator.StringToHash("Mode");
    const string c_roundTextFormatKey = "RoundStatus";
    const string c_screenModePrefKey = "ScreenMode";
    const string c_resolutionIndexPrefKey = "Resolution_Index";

    const string c_frameLimitModePrefKey = "FrameLimit";

    const string c_tutorialShow = "Tutorial";

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
        PlayerPrefs.Save();
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
        int count = Screen.resolutions.Length;

        // 저장값이 없으면(-1) 현재 데스크톱 해상도에 해당하는 인덱스를 기본값으로.
        int savedIndex = PlayerPrefs.GetInt(c_resolutionIndexPrefKey, -1);
        if (savedIndex < 0)
            savedIndex = GetDropdownIndexOf(Screen.currentResolution);

        // 모니터 구성이 바뀌어 인덱스가 범위를 벗어나는 경우 방어.
        savedIndex = Mathf.Clamp(savedIndex, 0, Mathf.Max(0, count - 1));

        SetResolutionByIndex(savedIndex);
    }

    public void SaveResolution()
    {
        PlayerPrefs.SetInt(c_resolutionIndexPrefKey, currentResolutionIndex);
    }

    // 드롭다운은 Screen.resolutions를 역순으로 표시한다(인덱스 0 = 가장 높은 해상도).
    // 적용·표시 모두 이 한 곳의 변환을 거쳐 항상 일치하게 한다.
    public void SetResolutionByIndex(int dropdownIndex)
    {
        int count = Screen.resolutions.Length;
        if (count == 0) return;

        dropdownIndex = Mathf.Clamp(dropdownIndex, 0, count - 1);
        currentResolutionIndex = dropdownIndex;
        currentResolution = Screen.resolutions[count - 1 - dropdownIndex];

        Screen.SetResolution(currentResolution.width, currentResolution.height,
                             Screen.fullScreenMode, currentResolution.refreshRateRatio);
    }

    // 해상도(주로 데스크톱 현재값)를 너비/높이로 찾아 드롭다운 인덱스로 환산. 없으면 0.
    int GetDropdownIndexOf(Resolution target)
    {
        Resolution[] list = Screen.resolutions;
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i].width == target.width && list[i].height == target.height)
                return list.Length - 1 - i;
        }
        return 0;
    }

    public void SetFrameLimitMode(ConfigPanel.FrameLimitMode frameLimitMode)
    {
        this.frameLimitMode = frameLimitMode;
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

        if (!PlayerPrefs.HasKey(c_tutorialShow))
        {
            PlayerPrefs.SetInt(c_tutorialShow, 1);
            Time.timeScale = 0;
            TutorialPanel.SetActive(true);
        }
    }

    public void EndTutorial()
    {
        Time.timeScale = 1;
        TutorialPanel.SetActive(false);
    }
    
    public void RefreshUI()
    {
        rerollText.text = string.Format(c_rerollTextFormat, currentRun.currentRound.currentCycle.reroll.ToString(), currentRun.rerollPerCycle.ToString());
        coinText.text = currentRun.Coin.ToString();

        roundText.text = string.Format(StringTable.GetString(c_roundTextFormatKey),currentRun.level,demoScoreCut.Count+1, currentRun.currentScore, currentRun.TargetScore);

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
