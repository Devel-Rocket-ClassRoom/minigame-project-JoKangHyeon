using System.Collections.Generic;
using UnityEngine;

public class RoundState
{
    public RunState currentRun;
    public CycleState currentCycle;
    public GameManager gameManager;

    public List<HandSlot> hands;
    public List<Dice> dices;

    public RoundState(GameManager gameManager, RunState runState)
    {
        this.gameManager = gameManager;
        currentRun = runState;
        EventBus.Subscribe<object>(EventType.OnCycleEnd, RoundEndCheck, Defines.c_maxPriority);
        EventBus.Subscribe<HandSlot>(EventType.OnSlotScoreFixed, OnSlotCalcEnd, Defines.c_maxPriority);
    }

    //라운드 사이클 정의
    public void Init()
    {
        EventBus.Publish(EventType.OnRoundStart, null);

        hands = new List<HandSlot>();
        foreach (var hand in currentRun.hands)
        {
            hands.Add(hand.Clone());
        }

        dices = new List<Dice>();
        foreach(var dice in currentRun.dices)
        {
            dices.Add(dice.Clone());
        }

        //이번 라운드의 첫 턴 시작
        CycleStart();
        currentCycle.isFirstCycle = true;
    }

    public void CycleStart()
    {
        foreach (var dice in dices)
        {
            dice.ResetDice();
        }

        currentCycle = new CycleState(gameManager, this);
        gameManager.RefreshUI();
        currentCycle.StartCycle();
        EventBus.Publish(EventType.OnFirstRollComplete, currentCycle.dicesRemain);
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

        if (flag)
        {
            RoundEnd();
        }
        else
        {
            CycleStart();
        }
    }

    public void OnSlotCalcEnd(HandSlot slot)
    {
        currentRun.currentScore += slot.currentScore;
    }

    public void RoundEnd()
    {
        Debug.Log("Round End");

        if (currentRun.currentScore >= gameManager.demoScoreCut[currentRun.level])
        {
            currentRun.currentScore -= gameManager.demoScoreCut[currentRun.level];
            currentRun.level += 1;
            Debug.Log("Round Clear");
            EventBus.Publish(EventType.OnRoundClear, null);

            Init();
        }
        else
        {
            Debug.Log("Round Failed");
            currentRun.isGameOver = true;
            EventBus.Publish(EventType.OnGameOver, null);
        }
    }


}
