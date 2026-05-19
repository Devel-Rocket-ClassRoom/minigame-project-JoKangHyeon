using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public RunState state;

    public HandDefinitionSO handDefine;
    public StartingDefinitionSO startingsDefine;
    public DiceDefinitionSO diceDefine;

    public int currentStartingIndex = 0;

    
    List<int> demoScoreCut = new()
    {
        100,  130, 200,
        260,  340, 510,
        660,  860, 1290,
        1680, 2180,3270,
        4250, 5530,8300,
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = new(this);
        state.Setup(startingsDefine.startings[currentStartingIndex]);
        state.RoundStart();
        RefreshUI();
    }

    // Update is called once per frame
    void Update()
    {
        
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



    public void Reroll()
    {
        state.currentCycle.Reroll();
        RefreshUI();
    }

    public void SetDice(int pos)
    {
        state.currentCycle.SetDice(pos);
        RefreshUI();
    }

    public void RetreveDice(int pos)
    {
        state.currentCycle.RetreveDice(pos);
        RefreshUI();
    }

    public void SetHand(int pos)
    {
        state.currentCycle.EndCycle(pos);
        RefreshUI();
    }


    public void RefreshUI()
    {
        foreach (TextMeshProUGUI diceRemianText in dicesRemain)
        {
            diceRemianText.transform.parent.gameObject.SetActive(false);
        }

        for(int i=0; i < state.currentCycle.dicesRemain.Count; i++)
        {
            if (i < dicesRemain.Count)
            {
                dicesRemain[i].transform.parent.gameObject.SetActive(true);
                dicesRemain[i].text = state.currentCycle.dicesRemain[i].GetDice().ToString();
            }
            else
            {
                GameObject newDice = Instantiate(dicePrefab,dicesRemainParent);
                TextMeshProUGUI text = newDice.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

                text.text = state.currentCycle.dicesRemain[i].GetDice().ToString();

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

        for (int i = 0; i < state.currentCycle.dicesSetted.Count; i++)
        {
            if (i < dicesSetted.Count)
            {
                dicesSetted[i].transform.parent.gameObject.SetActive(true);
                dicesSetted[i].text = state.currentCycle.dicesSetted[i].GetDice().ToString();
            }
            else
            {
                GameObject newDice = Instantiate(dicePrefab, dicesSettedParent);
                TextMeshProUGUI text = newDice.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

                text.text = state.currentCycle.dicesSetted[i].GetDice().ToString();

                int index = i;
                newDice.GetComponent<Button>().onClick.AddListener(() => RetreveDice(index));
                newDice.SetActive(true);

                dicesSetted.Add(text);
            }
        }

        testOutput.text = $"reroll left : {state.currentCycle.reroll}/{state.rerollPerCycle}\n\ncurrent level : {state.level}\n\ncurrent score : {state.currentScore}\ngoal score : {demoScoreCut[state.level]}\ncurrent coin : {state.coin}";


        for (int i = 0; i < state.hands.Count; i++)
        {
            if (i >= handsUI.Count)
            {
                var newHand = Instantiate(handPrefab,handParent);
                newHand.Set(state.hands[i]);
                handsUI.Add(newHand);
                int index = i;
                newHand.setButton.onClick.AddListener(() => { SetHand(index); });
            }
            handsUI[i].Refresh(state.currentCycle.dicesSetted);
        }
    }

    #endregion
}

public class CycleState
{
    public RunState currentRun;
    public GameManager gameManager;

    public bool isFirstCycle = false;

    public CycleState(GameManager gameManager, RunState runState)
    {
        this.gameManager = gameManager;
        currentRun = runState;
        dicesRemain = new();
        dicesSetted = new();
    }

    public List<Dice> dicesRemain;
    public List<Dice> dicesSetted;
    public int reroll;

    public void ResetCycle()
    {
        dicesRemain.Clear();
        dicesSetted.Clear();
        foreach (Dice dice in currentRun.dices)
        {
            dicesRemain.Add(dice.Clone());
        }

        reroll = currentRun.rerollPerCycle;
        RollAll();
    }

    public void EndCycle(int targetSlot)
    {
        currentRun.hands[targetSlot].hand.SetDice(dicesSetted);
        EventBus.Publish(EventType.OnSlotScored, currentRun.hands[targetSlot]);

        if (isFirstCycle)
        {
            EventBus.Publish(EventType.OnFirstScoreOfRound, currentRun.hands[targetSlot]);
        }
    }

    public void Reroll()
    {
        if (reroll < 1)
            return;
        reroll -= 1;
        RollAll();
    }

    private void RollAll()
    {
        foreach (Dice dice in dicesRemain)
        {
            dice.RollDice();
        }

        EventBus.Publish(EventType.OnRollComplete, dicesRemain);
    }

    public void SetDice(int pos)
    {
        Dice dice = dicesRemain[pos];
        dicesRemain.RemoveAt(pos);
        dicesSetted.Add(dice);
    }

    public void SetDice(Dice dice)
    {
        int pos = dicesRemain.IndexOf(dice);
        SetDice(pos);
    }

    public void RetreveDice(int pos)
    {
        Dice dice = dicesSetted[pos];
        dicesSetted.RemoveAt(pos);
        dicesRemain.Add(dice);
    }

    public void RetreveDice(Dice dice)
    {
        int pos = dicesSetted.IndexOf(dice);
        RetreveDice(dice);
    }
}

public class RunState
{
    public List<Dice> dices;
    public List<Relic> relics;
    public List<HandSlot> hands;
    public List<Card> cards;
    public List<IConsumable> consumableInventory;
    
    public int level;
    public int coin;
    public int rerollPerCycle;
    public int currentScore;

    public int maxCunsumable;
    const int c_defaultMaxCunsumable = 3;

    public Starting startingSet;
    public GameManager gameManager;
    public CycleState currentCycle;

    public RunState(GameManager gameManager)
    {
        this.gameManager = gameManager;   
    }

    public RunState(GameManager gameManager, int seed) : this(gameManager)
    {
        Random.InitState(seed);
    }

    public RunState(GameManager gameManager, Random.State state) : this(gameManager)
    {
        Random.state = state;
    }

    public void Setup(Starting starting)
    {
        startingSet = starting;

        dices = new();
        foreach(var diceName in starting.startingDices)
        {
            dices.Add(gameManager.diceDefine.Find(diceName));
        }

        hands = new();
        foreach(var handName in starting.startingHands)
        {
            HandSlot handSlot = new HandSlot();
            handSlot.hand = gameManager.handDefine.Find(handName);
            hands.Add(handSlot);
        }

        maxCunsumable = c_defaultMaxCunsumable;
        consumableInventory = new();

        coin = 0;
        level = 0;


        //DEBUG
        rerollPerCycle = 8000;
    }




    //라운드 사이클 정의
    public void RoundStart()
    {
        EventBus.Publish(EventType.OnRoundStart, null);

        foreach (var hand in hands)
        {
            hand.hand.ResetHand();
        }

        //이번 라운드의 첫 턴 시작
        CycleStart();
        currentCycle.isFirstCycle = true;
        EventBus.Subscribe<HandSlot>(EventType.OnSlotScored, RoundEndCheck);
    }

    public void CycleStart()
    {
        foreach (var dice in dices)
        {
            dice.ResetDice();
        }

        currentCycle = new CycleState(gameManager, this);
        currentCycle.ResetCycle();
        EventBus.Publish(EventType.OnFirstRollComplete, currentCycle.dicesRemain);
        gameManager.RefreshUI();
    }

    public void RoundEndCheck(object _)
    {
        bool flag = true;
        foreach (var hand in hands)
        {
            if (!hand.hand.IsUsed())
            {
                flag = false;
                break;
            }
        }

        if(flag)
        {
            RoundEnd();
            EventBus.Unsubscribe<HandSlot>(EventType.OnSlotScored, RoundEndCheck);
        }
        else
        {
            CycleStart();
        }
    }

    public void RoundEnd()
    {
        Debug.Log("Round End");
    }
}


public enum EventType
{
    /// <summary>
    /// 족보 점수 계산 시작 전
    /// return : int HandNumber
    /// </summary>
    OnHandStart,
    /// <summary>
    /// 이번 사이클의 첫번째 주사위 굴림 후
    /// return : List<Dice>
    /// </summary>
    OnFirstRollComplete,
    /// <summary>
    /// 주사위 굴림 완료 후
    /// return : List<Dice>
    /// </summary>
    OnRollComplete,
    /// <summary>
    /// 족보 슬롯에 점수가 입력되었을 때
    /// return : HandSlot
    /// </summary>
    OnSlotScored,
    /// <summary>
    /// 이번 라운드의 첫 점수 활성화시
    /// return : HandSlot
    /// </summary>
    OnFirstScoreOfRound,
    /// <summary>
    /// 라운드 시작 시
    /// return : null
    /// </summary>
    OnRoundStart,
    /// <summary>
    /// 라운드 성공 시
    /// retrun : null
    /// </summary>
    OnRoundClear,
}


public static class EventBus
{
    private static Dictionary<EventType, Action<object>> eventTable = new();

    public static void Subscribe<T>(EventType eventType, Action<T> callback)
    {
        if (!eventTable.ContainsKey(eventType))
        {
            eventTable[eventType] = (obj) => callback((T)obj);
        }
        else
        {
            eventTable[eventType] += (obj) => callback((T)obj);
        }
    }

    public static void Unsubscribe<T>(EventType eventType, Action<T> callback)
    {
        if (eventTable.ContainsKey(eventType))
        {
            eventTable[eventType] -= (obj) => callback((T)obj);
        }
    }

    public static void Publish(EventType eventType, object eventData)
    {
        if (eventTable.ContainsKey(eventType))
        {
            eventTable[eventType]?.Invoke(eventData);
        }
    }
}