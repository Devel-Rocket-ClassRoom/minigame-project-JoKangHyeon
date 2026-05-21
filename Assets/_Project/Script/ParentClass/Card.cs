
using System;

[Serializable]
public abstract class Card
{
    public string name;
    public string description;

    public abstract Card Clone();

    public abstract void OnObtain(GameManager gameManager);

    public virtual void OnCycleStart() { }
    public virtual void OnRoundStart() { }
}

public class Card1 : Card
{
    public GameManager gameManager;
    public override Card Clone()
    {
        return new Card1();
    }

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

public class Card2 : Card
{
    public GameManager gameManager;
    public int coinAmount = 2;
    int currentRoundLimit;
    public int roundLimit = 5;

    public override Card Clone()
    {
        return new Card2();
    }

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

public class Card3 : Card
{
    public GameManager gameManager;
    public int rerollAmount = 1;
    int currentRoundLimit;
    public int roundLimit = 1;

    public override Card Clone()
    {
        throw new NotImplementedException();
    }

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

public class Card4 : Card
{
    public GameManager gameManager;
    public int increaseAmount = 5;

    public override Card Clone()
    {
        return new Card4();
    }
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
        dice.SetFaceValue(face, dice.faces[face].value + increaseAmount);
    }
}


public class HandCard1 : Card
{
    public GameManager gameManager;

    public override Card Clone()
    {
        throw new NotImplementedException();
    }

    public override void OnObtain(GameManager gameManager)
    {
        throw new NotImplementedException();
    }
}