using System.Collections.Generic;

public abstract class Hand
{
    protected List<Dice> diceList;

    public virtual bool IsAchived(List<Dice> dices)
    {
        return GetDiceScore(diceList) > 0;
    }
    public abstract int GetDiceScore(List<Dice> dices);

    public virtual int SetDice(List<Dice> dices)
    {
        diceList = dices;
        return GetCurrentHandScore();
    }

    public virtual void ResetHand() 
    {
        diceList = null;
    }

    public virtual int GetCurrentHandScore()
    {
        return GetDiceScore(diceList);
    }
}
