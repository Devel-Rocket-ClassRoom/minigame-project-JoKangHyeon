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


    public void StartCycle()
    {
        dicesRemain.Clear();
        dicesSetted.Clear();
        foreach (Dice dice in currentRound.dices)
        {
            dicesRemain.Add(dice.Clone());
        }

        reroll = currentRound.currentRun.rerollPerCycle;

        ShowDiceObjects();
        gameManager.RefreshUI();
        RollAll();
    }

    public void EndCycle(int targetSlot)
    {
        foreach (DiceObject dice in diceObjects)
        {
            GameObject.Destroy(dice.gameObject);
        }

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

    public void Reroll()
    {
        if (gameManager.rollManager.rolling)
            return;

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


        foreach(var diceObj in diceObjects)
        {
            diceObj.SetOutline(false);
        }

        gameManager.StartCoroutine(gameManager.rollManager.DeterministicRoll(diceObjects));
        UpdateOutline();
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

    public void SetDice(DiceObject dice)
    {
        int pos = dicesRemain.IndexOf(dice.dice);
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
            if (dicesSetted.Contains(diceObj.dice))
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
        if (dice.dice == null) return;
        if (dicesSetted.Contains(dice.dice))
        {
            RetreveDice(dice.dice);
        }
        else
        {
            SetDice(dice.dice);
        }

        UpdateOutline();
        gameManager.RefreshUI();
    }

    public void ShowDiceObjects()
    {
        foreach (var diceObject in diceObjects)
        {
            GameObject.Destroy(diceObject.gameObject);
        }

        for (int i = 0; i < dicesRemain.Count; i++)
        {
            Dice dice = dicesRemain[i];
            var diceObject = GameObject.Instantiate(dice.prefab, gameManager.DiceSpawnPoint.transform);
            diceObject.rb.isKinematic = true;
            diceObject.transform.localPosition = Vector3.left * (i - dicesRemain.Count / 2.0f);
            diceObject.dice = dice;
            diceObjects.Add(diceObject);
        }
    }

}
