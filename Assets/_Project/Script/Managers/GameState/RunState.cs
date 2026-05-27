using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RunState
{
    public List<Dice> dices = new();
    public List<Relic> relics = new();
    public List<HandSlot> hands = new();
    public List<Card> cards = new();
    public List<Consumable> consumableInventory = new();

    public int level;
    private int _coin;
    public int Coin
    {
        get
        {
            return _coin;
        }
        set
        {
            _coin = value;
            gameManager.UpdateCoin(Coin);
        }
    }
    public int rerollPerCycle;
    public int currentScore;
    public int TargetScore
    {
        get
        {
            return gameManager.demoScoreCut[level];
        }
    }

    public int maxCunsumable;
    const int c_defaultMaxCunsumable = 3;

    public Starting startingSet;
    public GameManager gameManager;
    public RoundState currentRound;
    public ShopState shopState;


    public bool isGameOver = false;


    #region Constructor
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
    #endregion

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

        Coin = Defines.c_startingCoin;
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
        Coin += amount;
    }

    public void GetReroll(int amount)
    {
        currentRound.currentCycle.reroll += amount;
    }

    public void ShowShop()
    {
        if(shopState == null)
        {
            shopState = new ShopState();
        }
        shopState.Init(gameManager);
        gameManager.ShowShop(shopState);
    }

    public void GetCard(Card card)
    {

    }

    public void GetRelic(Relic relic)
    {
    }
}
