using System;

[Serializable]
public class CardP1 : Card
{
    public GameManager gameManager;
    public override Card Clone() => new CardP1()
    {
        name = this.name,
        description = this.description,
        cost = this.cost,
        rarity = this.rarity
    };

    public override void OnObtain(GameManager gameManager)
    {
        this.gameManager = gameManager;
        gameManager.StartDiceSelect(OnDiceSelect);
    }

    private void OnDiceSelect(Dice dice)
    {
        gameManager.StartDiceFaceSelect(dice, OnFaceSelect);
    }

    public void OnFaceSelect(Dice dice, int face)
    {
        dice.SetFace(face, 6);
    }
}

[Serializable]
public class CardP6 : Card
{
    public GameManager gameManager;
    public int coinAmount = 2;
    int currentRoundLimit;
    public int roundLimit = 5;

    public override Card Clone() => new CardP6()
    {
        name = this.name,
        description = this.description,
        cost = this.cost,
        rarity = this.rarity,

        coinAmount = this.coinAmount,
        roundLimit = this.roundLimit
    };

    public override void OnObtain(GameManager gameManager)
    {
        this.gameManager = gameManager;
        gameManager.StartDiceSelect(OnDiceSelect);
    }

    private void OnDiceSelect(Dice dice)
    {
        gameManager.StartDiceFaceSelect(dice, OnFaceSelect);
    }

    public void OnFaceSelect(Dice dice, int face)
    {
        dice.faces[face].OnRolled += (d, f) =>
        {
            if (currentRoundLimit > 0)
            {
                currentRoundLimit--;
                gameManager.currentRun.GetCoin(coinAmount);
            }
        };
    }

    public override void OnRoundStart()
    {
        currentRoundLimit = roundLimit;
    }
}

[Serializable]
public class CardC7 : Card
{
    public GameManager gameManager;
    public int rerollAmount = 1;
    int currentRoundLimit;
    public int roundLimit = 1;

    public override Card Clone() => new CardC7()
    {
        name = this.name,
        description = this.description,
        cost = this.cost,
        rarity = this.rarity,
        rerollAmount = this.rerollAmount,
        roundLimit = this.roundLimit,
    };

    public override void OnObtain(GameManager gameManager)
    {
        this.gameManager = gameManager;
        gameManager.StartDiceSelect(OnDiceSelect);
    }

    private void OnDiceSelect(Dice dice)
    {
        gameManager.StartDiceFaceSelect(dice, OnFaceSelect);
    }

    public void OnFaceSelect(Dice dice, int face)
    {
        dice.faces[face].OnRolled += (d, f) =>
        {
            if (currentRoundLimit > 0)
            {
                currentRoundLimit--;
                gameManager.currentRun.GetReroll(rerollAmount);
            }
        };
    }
}

[Serializable]
public class CardM10 : Card
{
    public GameManager gameManager;
    public int increaseAmount = 5;

    public override Card Clone() => new CardM10()
    {
        name = this.name,
        description = this.description,
        cost = this.cost,
        rarity = this.rarity,
        increaseAmount = this.increaseAmount,
    };

    public override void OnObtain(GameManager gameManager)
    {
        this.gameManager = gameManager;
        gameManager.StartDiceSelect(OnDiceSelect);
    }
    private void OnDiceSelect(Dice dice)
    {
        gameManager.StartDiceFaceSelect(dice, OnFaceSelect);
    }
    public void OnFaceSelect(Dice dice, int face)
    {
        dice.SetFaceValue(face, dice.faces[face].Value + increaseAmount);
    }
}

[Serializable]
public class CardM21 : Card
{
    public GameManager gameManager;
    public int increaseAmount = 1;

    public override Card Clone() => new CardM10()
    {
        name = this.name,
        description = this.description,
        cost = this.cost,
        rarity = this.rarity,
        increaseAmount = this.increaseAmount,
    };
    public override void OnObtain(GameManager gameManager)
    {
        this.gameManager = gameManager;
        gameManager.StartDiceSelect(OnDiceSelect);
    }
    private void OnDiceSelect(Dice dice)
    {
        gameManager.StartDiceFaceSelect(dice, OnFaceSelect);
    }
    public void OnFaceSelect(Dice dice, int face)
    {
        dice.faces[face].OnRolled += (d, f) =>
        {
            foreach(var dice in gameManager.currentRun.currentRound.currentCycle.dicesRemain)
            {
                dice.GetFace().OverrideValue(dice.GetFace().Value + increaseAmount);
            }
        };
    }
}

[Serializable]

public class CardM0 : Card
{
    public DiceDefinitionSO diceSO;

    public override Card Clone() => new CardM0
    {
        name = this.name,
        description = this.description,
        cost = this.cost,
        rarity = this.rarity,
        diceSO = diceSO
    };

    public override void OnObtain(GameManager gameManager)
    {
        gameManager.currentRun.dices.Add(diceSO.dices[0].Clone());
    }
}

[Serializable]
public class CardS0 : Card
{
    public string targetHandName;
    public HandDefinitionSO handSO;

    public override Card Clone()
    => new CardS0()
    {
        name = this.name,
        description = this.description,
        cost = this.cost,
        rarity = this.rarity,
        targetHandName = this.targetHandName
    };

    public override void OnObtain(GameManager gameManager)
    {
        gameManager.StartHandSelect(OnHandSelected);
    }

    public void OnHandSelected(HandSlot slot)
    {
        slot.hand = handSO.Find(targetHandName);
    }
}

[Serializable]
public class CardW4 : Card
{
    public override Card Clone()
    {
        throw new NotImplementedException();
    }

    public override void OnObtain(GameManager gameManager)
    {
        throw new NotImplementedException();
    }
}