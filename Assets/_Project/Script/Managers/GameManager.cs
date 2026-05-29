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
    public ConsumableDefinitionSO consumableDefine;
    public ShopRarityDefinitionSO rarityDefine;

    [Header("UI")]
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI rerollText;
    public Button rerollButton;
    public Button MenuButton;
    public Button skipButton;
    public TextMeshProUGUI skipText;


    [Header("Sub Managers")]
    public RollManager rollManager;

    [Header("Canvases")]
    public ShopCanvas shopCanvas;
    public DiceSelectCanvas diceSelectCanvas;
    public FaceSelectCanvas faceSelectCanvas;
    public HandSelectCanvas handSelectCanvas;

    [Header("View Objects")]
    public List<GameObject> gameStateObjects;
    public List<GameObject> shopStateObjects;
    public GameObject pauseSceen;
    public GameObject gameOverScreen;
    public Tooltip tooltip;

    [Header("Other")]
    public GameObject DiceSpawnPoint;
    public int currentStartingIndex = 0;

    //Not Serialized
    public RunState currentRun;
    public const string c_skipCoinTextFormat = "+{0}C";



    private void Awake()
    {
        rollManager = GetComponent<RollManager>();
        attack = InputSystem.actions.FindAction("Attack");
    }

    public List<int> demoScoreCut = new()
    {
        100,  130, 200,
        260,  340, 510,
        660,  860, 1290,
        1680, 2180,3270,
        4250, 5530,8300,
    };

    InputAction attack;
    public LayerMask diceMask;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RestartGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (attack.WasPerformedThisFrame())
        {
            Vector2 pos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(pos);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, diceMask))
            {
                if (hit.collider.CompareTag("Dice"))
                {
                    currentRun.currentRound.currentCycle.ToggleDice(hit.collider.GetComponent<DiceObject>());
                }
            }

            foreach(var hitDeBug in Physics.RaycastAll(ray))
            {
                Debug.Log(hitDeBug.collider.name);
            }
        }



        //TODO : 키 옮기고 디버그 삭제
        #region DEBUG
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            currentRun.currentRound.currentCycle.Reroll();
            RefreshUI();
        }

        // 점쟁이의 손가락 임시 발동 hook (F키) — 정식 UI 연결 전까지 사용
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            var finger = currentRun?.relics.Find(r => r is FortuneTellersFinger) as FortuneTellersFinger;
            finger?.TryActivate();
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


    #region UITestMode

    public List<TextMeshProUGUI> dicesSetted;
    public List<TextMeshProUGUI> dicesRemain;
    public List<SlotUI> handsUI;

    public GameObject dicePrefab;
    public SlotUI handPrefab;

    public Transform dicesRemainParent;
    public Transform dicesSettedParent;
    public Transform handParent;

    public TextMeshProUGUI testOutput;

    public GameObject restartButton;



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
        gameOverScreen.SetActive(false);

        foreach (var hand in handsUI)
        {
            Destroy(hand.gameObject);
        }
        handsUI.Clear();

        EventBus.Clear();


        currentRun = new(this);
        currentRun.Setup(startingsDefine.startings[currentStartingIndex]);
        restartButton.gameObject.SetActive(false);
        EventBus.Subscribe<object>(EventType.OnGameOver, OnGameOver,Defines.c_maxPriority);
        EventBus.Subscribe<List<Dice>>(EventType.OnRollComplete, RemoveEffect,Defines.c_maxPriority);


        currentRun.RoundStart();
        RefreshUI();
    }

    public void RefreshUI()
    {
        foreach (TextMeshProUGUI diceRemianText in dicesRemain)
        {
            diceRemianText.transform.parent.gameObject.SetActive(false);
        }

        for (int i = 0; i < currentRun.currentRound.currentCycle.dicesRemain.Count; i++)
        {
            if (i < dicesRemain.Count)
            {
                dicesRemain[i].transform.parent.gameObject.SetActive(true);
                dicesRemain[i].text = currentRun.currentRound.currentCycle.dicesRemain[i].GetDice().ToString();
            }
            else
            {
                GameObject newDice = Instantiate(dicePrefab, dicesRemainParent);
                TextMeshProUGUI text = newDice.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

                text.text = currentRun.currentRound.currentCycle.dicesRemain[i].GetDice().ToString();

                int index = i;
                newDice.GetComponent<Button>().onClick.AddListener(() => SetDice(index));
                newDice.SetActive(true);

                dicesRemain.Add(text);
            }
        }

        foreach (TextMeshProUGUI diceSetText in dicesSetted)
        {
            diceSetText.transform.parent.gameObject.SetActive(false);
        }

        for (int i = 0; i < currentRun.currentRound.currentCycle.dicesSetted.Count; i++)
        {
            if (i < dicesSetted.Count)
            {
                dicesSetted[i].transform.parent.gameObject.SetActive(true);
                dicesSetted[i].text = currentRun.currentRound.currentCycle.dicesSetted[i].GetDice().ToString();
            }
            else
            {
                GameObject newDice = Instantiate(dicePrefab, dicesSettedParent);
                TextMeshProUGUI text = newDice.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

                text.text = currentRun.currentRound.currentCycle.dicesSetted[i].GetDice().ToString();

                int index = i;
                newDice.GetComponent<Button>().onClick.AddListener(() => RetreveDice(index));
                newDice.SetActive(true);

                dicesSetted.Add(text);
            }
        }

        //DEBUG
        //testOutput.text = $"reroll left : {currentRun.currentRound.currentCycle.reroll}/{currentRun.rerollPerCycle}\n\ncurrent level : {currentRun.level}\n\ncurrent score : {currentRun.currentScore}\ngoal score : {demoScoreCut[currentRun.level]}\ncurrent coin : {currentRun.coin}";


        rerollText.text = $"{currentRun.currentRound.currentCycle.reroll} / {currentRun.rerollPerCycle}";
        coinText.text = currentRun.Coin.ToString();
        roundText.text = string.Format(StringTable.GetString("RoundStatus"), currentRun.currentScore, currentRun.TargetScore);

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
                var newHand = Instantiate(handPrefab, handParent);
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
        skipText.text = string.Format(c_skipCoinTextFormat, currentRun.GetSkipGold().ToString());
    }



    public void OnGameOver(object _)
    {
        gameOverScreen.SetActive(true);
    }

    

    public void RemoveEffect(List<Dice> _)
    {
        foreach (var dice in dicesRemain)
        {
            dice.transform.parent.gameObject.GetComponent<Image>().color = Color.white;
        }

        foreach (var dice in dicesSetted)
        {
            dice.transform.parent.gameObject.GetComponent<Image>().color = Color.white;
        }
    }

    public void SetRemianDiceEffect(int pos)
    {
        Debug.Log(pos);


        dicesRemain[pos].transform.parent.gameObject.GetComponent<Image>().color = Color.green;
    }
    #endregion


    public void StartDiceSelect(Action<Dice> callback)
    {
        diceSelectCanvas.StartDiceSelect(this, callback);
    }

    public void StartDiceFaceSelect(Dice target, Action<Dice,int> callback)
    {
        faceSelectCanvas.StartFaceSelect(target, callback);
    }

    public void StartHandSelect(Action<HandSlot> callback, Func<HandSlot, bool> filter = null)
    {
        handSelectCanvas.StartHandSelect(this, callback, filter);
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

        tooltip.HideTooltip();
        shopCanvas.Init(this, state);
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
        tooltip.HideTooltip();
    }

    public void ResumeGame()
    {
        pauseSceen.SetActive(false);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        
#else
        Application.Quit();
#endif
    }
}
