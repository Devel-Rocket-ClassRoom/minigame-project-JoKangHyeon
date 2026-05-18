using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public abstract class Hand
{
    [NonSerialized]
    protected List<Dice> diceList;

    public string name;
    public float scoreMultiplier = 1f;
    public bool Setted => diceList != null;

    public virtual bool IsAchived(List<Dice> dices)
    {
        return GetDiceScore(dices) > 0;
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

    public abstract Hand Clone();

    public string GetDicesString()
    {
        if(diceList == null) return string.Empty;
        var dices = diceList.ConvertAll<int>((d) => d.GetDice());
        return string.Join(',', dices);
    }

    public int GetCurrentMultipliedScore()
    {
        return (int)Mathf.Ceil(GetCurrentHandScore() * scoreMultiplier);
    }

    public string GetCurrentDetailedString()
    {
        return $"{GetCurrentHandScore()}X{scoreMultiplier:N2}={GetCurrentMultipliedScore()}";
    }

    public int GetMultipliedScore(List<Dice> dices)
    {
        return (int)Mathf.Ceil(GetDiceScore(dices) * scoreMultiplier);
    }
    public string GetDetailedString(List<Dice> dices)
    {
        return $"{GetDiceScore(dices)}X{scoreMultiplier:N2}={GetMultipliedScore(dices)}";
    }
}
