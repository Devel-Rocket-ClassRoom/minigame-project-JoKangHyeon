using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    public RunState currentRun;

    public StartingDefinitionSO startingsDefine;
    public HandDefinitionSO handDefine;
    public DiceDefinitionSO diceDefine;
    public RelicDefinitionSO relicDefine;

    public int currentStartingIndex = 0;

    public GameObject DiceSpawnPoint;

    public RollManager rollManager;

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
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            currentRun.currentRound.currentCycle.Reroll();
            RefreshUI();
        }
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
        foreach (var hand in handsUI)
        {
            Destroy(hand.gameObject);
        }
        handsUI.Clear();


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

        testOutput.text = $"reroll left : {currentRun.currentRound.currentCycle.reroll}/{currentRun.rerollPerCycle}\n\ncurrent level : {currentRun.level}\n\ncurrent score : {currentRun.currentScore}\ngoal score : {demoScoreCut[currentRun.level]}\ncurrent coin : {currentRun.coin}";


        for (int i = 0; i < currentRun.currentRound.hands.Count; i++)
        {
            if (i >= handsUI.Count)
            {
                var newHand = Instantiate(handPrefab, handParent);
                handsUI.Add(newHand);
                int index = i;
                newHand.setButton.onClick.AddListener(() => { SetHand(index); });
            }
            handsUI[i].Refresh(currentRun.currentRound.hands[i], currentRun.currentRound.currentCycle.dicesSetted);
        }
    }

    public void OnGameOver(object _)
    {
        testOutput.text = $"Game Over\nYour Score : {currentRun.currentScore}\nPress Restart to try again!";
        restartButton.SetActive(true);
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
        //TODO : UI로 주사위 선택

        //DEBUG
        if (currentRun.currentRound.currentCycle.dicesRemain.Count > 0)
        {
            callback(currentRun.currentRound.currentCycle.dicesRemain[0]);
        }
    }

    public void StartDiceFaceSelect(Dice target, Action<Dice,int> callback)
    {
        //TODO : UI로 주사위 면 선택
        //DEBUG
        callback(target, 1);
    }
}
