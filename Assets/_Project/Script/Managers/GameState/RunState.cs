using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RunState
{
    public List<Dice> dices;
    public List<Relic> relics;
    public List<HandSlot> hands;
    public List<Card> cards;
    public List<Consumable> consumableInventory;

    public int level;
    public int coin;
    public int rerollPerCycle;
    public int currentScore;

    public int maxCunsumable;
    const int c_defaultMaxCunsumable = 3;

    public Starting startingSet;
    public GameManager gameManager;
    public RoundState currentRound;

    public bool isGameOver = false;

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
        foreach (var diceName in starting.startingDices)
        {
            Dice dice = gameManager.diceDefine.Find(diceName);
            dices.Add(dice);
        }

        hands = new();
        foreach (var handName in starting.startingHands)
        {
            HandSlot handSlot = new HandSlot();
            handSlot.hand = gameManager.handDefine.Find(handName);
            hands.Add(handSlot);
        }

        relics = new();
        foreach (var relicName in starting.startingRelics)
        {
            var relic = gameManager.relicDefine.Find(relicName);
            relics.Add(relic);
            relic.OnAdd(gameManager);
        }

        maxCunsumable = c_defaultMaxCunsumable;
        consumableInventory = new();

        coin = 0;
        level = 0;


        rerollPerCycle = 2;
    }

   

    public void RoundStart()
    {
        currentRound = new RoundState(gameManager, this);
        currentRound.Init();
    }

    public void GetCoin(int amount)
    {
        coin += amount;
    }

    public void GetReroll(int amount)
    {
        currentRound.currentCycle.reroll += amount;
    }
}
