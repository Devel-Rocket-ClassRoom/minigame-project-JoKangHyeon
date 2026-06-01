using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class RunState
{
    public List<Dice> dices = new();
    public List<Relic> relics = new();
    public List<HandSlot> hands = new();
    public List<Card> cards = new();
    public List<Consumable> consumableInventory = new();

    public int slotCount = Defines.c_defaultConsumableInventory;

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
            handSlot.hand.slot = handSlot;
            handSlot.slotLevel = 1;   // 시작 슬롯은 Lv 1
            hands.Add(handSlot);
        }

        relics = new();
        foreach (var relicName in starting.startingRelics)
        {
            var relic = gameManager.relicDefine.Find(relicName);
            relics.Add(relic);
            relic.OnObtain(gameManager);
        }

        maxCunsumable = c_defaultMaxCunsumable;
        consumableInventory = new();

        Coin = Defines.c_startingCoin;
        level = 0;


        rerollPerCycle = 2;
        gameManager.activeItemCanvas.Refresh(gameManager, consumableInventory, slotCount);
    }

   

    public void RoundStart()
    {
        // 카드 라운드 시작 훅 — CardP6/C7 등 currentRoundLimit 리셋
        cards.ForEach(c => c.OnRoundStart());

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
        Card clonedCard = card.Clone();
        cards.Add(clonedCard);
        clonedCard.OnObtain(gameManager);
    }

    public void GetRelic(Relic relic)
    {
        relics.Add(relic);
        relic.OnObtain(gameManager);
    }

    public bool CanAddConsumable() => consumableInventory.Count < maxCunsumable;

    public void AddConsumable(Consumable c)
    {
        if (consumableInventory.Count > slotCount)
            return;

        Consumable clone = c.Clone();
        consumableInventory.Add(clone);
        clone.OnAdd(gameManager);

        gameManager.activeItemCanvas.Refresh(gameManager, consumableInventory, slotCount);
    }
    

    public void UseConsumable(Consumable c)
    {
        bool success = c.OnUse(gameManager);
        if (success)
        {
            consumableInventory.Remove(c);
            c.OnRemove();
            gameManager.RefreshUI();
            gameManager.activeItemCanvas.Refresh(gameManager, consumableInventory, slotCount);
        }
    }

    public bool IsConsumableInventoryMax()
    {
        return consumableInventory.Count >= slotCount;
    }

    public void Skip()
    {
        Coin += GetSkipGold();
        currentRound.currentCycle.ClearDiceObject();
        currentRound.RoundEnd();
    }

    public int GetSkipGold()
    {
        int handLeft = currentRound.hands.Count((h) => !h.isUsed);
        return handLeft * Defines.c_coinPerHandLeft;
    }

    public bool IsSkipable()
    {
        if (currentScore < TargetScore)
            return false;

        if (currentRound.currentCycle.reroll < rerollPerCycle)
            return false;

        return true;
    }
}
