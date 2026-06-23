using System.Collections.Generic;
using UnityEngine;
using Debug= UnityEngine.Debug;

public class CycleState
{
    public RoundState currentRound;
    public GameManager gameManager;

    public bool isFirstCycle = false;

    public CycleState(GameManager gameManager, RoundState roundState)
    {
        this.gameManager = gameManager;
        currentRound = roundState;
        dicesRemain = new();
        dicesSetted = new();
    }

    public List<Dice> dicesRemain;
    public List<Dice> dicesSetted;
    public int reroll;

    public List<DiceObject> diceObjects = new();

    public bool freeRerollActive = false;

    public List<Dice> dices;

    public void StartCycle()
    {
        dicesRemain.Clear();
        dicesSetted.Clear();
        foreach (Dice dice in currentRound.dices)
        {
            dicesRemain.Add(dice.Clone());
        }

        dices = new List<Dice>(dicesRemain);
        reroll = currentRound.currentRun.rerollPerCycle;

        ShowDiceObjects();
        gameManager.RefreshUI();
        RollAll();
    }

    public void EndCycle(int targetSlot)
    {
        ClearDiceObject();

        currentRound.hands[targetSlot].hand.SetDice(dicesSetted);
        currentRound.hands[targetSlot].SetCurrentScore();
        EventBus.Publish(EventType.OnSlotScored, currentRound.hands[targetSlot]);

        if (isFirstCycle)
        {
            EventBus.Publish(EventType.OnFirstScoreOfRound, currentRound.hands[targetSlot]);
        }
        EventBus.Publish(EventType.OnSlotScoreFixed, currentRound.hands[targetSlot]);
        EventBus.Publish(EventType.OnCycleEnd, null);
    }

    public void ClearDiceObject() {
        foreach (DiceObject dice in diceObjects)
        {
            dice.Release();
        }
        diceObjects.Clear();
    }

    public void Reroll()
    {
        if (gameManager.rollManager.rolling)
            return;

        if (!freeRerollActive && reroll < 1)
            return;

        if (!freeRerollActive)
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


        foreach(var diceObj in diceObjects)
        {
            diceObj.SetOutline(false);
        }

        List<DiceObject> rerollTarget = new();

        foreach(var diceObj in diceObjects)
        {
            if(dicesRemain.Contains(diceObj.Dice))
            {
                rerollTarget.Add(diceObj);
            }
        }

        gameManager.StartCoroutine(gameManager.rollManager.DeterministicRoll(rerollTarget));
        UpdateOutline();
    }

    public void SetDice(int pos)
    {
        if (dicesSetted.Count >= 5)
            return;

        Dice dice = dicesRemain[pos];
        dicesRemain.RemoveAt(pos);
        dicesSetted.Add(dice);
    }

    public void SetDice(Dice dice)
    {
        int pos = dicesRemain.IndexOf(dice);
        SetDice(pos);
    }

    public void SetDice(DiceObject dice)
    {
        int pos = dicesRemain.IndexOf(dice.Dice);
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
        RetreveDice(pos);
    }


    public void UpdateOutline()
    {
        foreach (var diceObj in diceObjects)
        {
            if (dicesSetted.Contains(diceObj.Dice))
            {
                diceObj.SetOutline(true);
            }
            else
            {
                diceObj.SetOutline(false);
            }
        }
    }

    public void ToggleDice(DiceObject dice)
    {
        if(dice == null) return;
        if (dice.Dice == null) return;
        if (dicesSetted.Contains(dice.Dice))
        {
            RetreveDice(dice.Dice);
        }
        else
        {
            SetDice(dice.Dice);
        }

        UpdateOutline();
        gameManager.RefreshUI();
    }

    public void ShowDiceObjects()
    {
        ClearDiceObject();

        for (int i = 0; i < dicesRemain.Count; i++)
        {
            Dice dice = dicesRemain[i];
            var diceObject = DiceObjectPool.Instance.GetDiceObject(dice);
            diceObject.rb.isKinematic = true;
            diceObject.transform.localPosition = Vector3.left * (i - dicesRemain.Count / 2.5f);
            diceObject.Dice = dice;
            diceObjects.Add(diceObject);
        }
    }


    public void AddDice(List<Dice> dices)
    {
        foreach (Dice dice in dices)
        {
            dicesRemain.Add(dice);
        }

        List<DiceObject> newDiceObjects = new();

        for (int i = 0; i < dices.Count; i++)
        {
            Dice dice = dices[i];
            var diceObject = DiceObjectPool.Instance.GetDiceObject(dice); 
            diceObject.rb.isKinematic = true;
            diceObject.transform.localPosition = Vector3.left * (i - dicesRemain.Count / 2.5f);
            diceObject.Dice = dice;
            diceObjects.Add(diceObject);
            newDiceObjects.Add(diceObject);
        }

        foreach(Dice dice in dices)
        {
            dice.RollDice();
        }

        gameManager.StartCoroutine(gameManager.rollManager.DeterministicRoll(newDiceObjects));
        UpdateOutline();
    }
}
