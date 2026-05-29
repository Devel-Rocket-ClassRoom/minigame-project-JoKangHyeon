using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Hand
{
    [NonSerialized]
    protected List<Dice> diceList;

    [NonSerialized]
    protected List<Dice> effectiveDices;

    public string nameStringKey;
    public virtual string Name
    {
        get
        {
            return StringTable.GetString(nameStringKey);
        }
    }
    
    public string decryptionStringKey;
    public virtual string Description
    {
        get
        {
            return StringTable.GetString(decryptionStringKey);
        }
    }
    public float baseScoreMultiplier = 1f;
    public float ScoreMultiplier
    {
        get
        {
            return baseScoreMultiplier * slot.slotLevel;
        }
    }

        
    public bool Setted => diceList != null;

    public HandSlot slot;

    public virtual bool IsAchived(List<Dice> dices)
    {
        return GetDiceScore(dices) > 0;
    }

    public virtual bool IsAchived()
    {
        return IsAchived(diceList);
    }

    public virtual bool IsUsed()
    {
        return diceList != null;
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
        effectiveDices = null;
    }

    public virtual int GetCurrentHandScore()
    {
        return GetDiceScore(diceList);
    }

    public Hand Clone()
    {
        Hand clonedHand = CloneInstance();
        clonedHand.nameStringKey = nameStringKey;
        clonedHand.decryptionStringKey = decryptionStringKey;
        clonedHand.baseScoreMultiplier = baseScoreMultiplier;

        return clonedHand;
    }

    protected abstract Hand CloneInstance();

    public string GetDicesString()
    {
        if(diceList == null) return string.Empty;
        var result = GetResult();

        var dices = result.dices.ConvertAll<int>((d) => d.GetDice());
        var effectiveDices = result.effectiveDices.ConvertAll<int>((d) => d.GetDice());

        return string.Join(',', dices) + "\neffective:" + string.Join(',', effectiveDices);
    }

    public int GetCurrentMultipliedScore()
    {
        return (int)Mathf.Ceil(GetCurrentHandScore() * ScoreMultiplier);
    }

    public string GetCurrentDetailedString()
    {
        return $"{GetCurrentHandScore()}X{ScoreMultiplier:N2}={GetCurrentMultipliedScore()}";
    }

    public int GetMultipliedScore(List<Dice> dices)
    {
        return (int)Mathf.Ceil(GetDiceScore(dices) * ScoreMultiplier);
    }
    public string GetDetailedString(List<Dice> dices)
    {
        return $"{GetDiceScore(dices)}X{ScoreMultiplier:N2}={GetMultipliedScore(dices)}";
    }

    public virtual void SetEffectiveDices()
    {
        effectiveDices = GetEffectiveDices(diceList);
    }

    public abstract List<Dice> GetEffectiveDices(List<Dice> dices);

    public struct HandResult
    {
        public List<Dice> dices;
        public List<Dice> effectiveDices;
        public int baseScore;
        public bool isAchived;
    }

    public HandResult GetResult()
    {
        SetEffectiveDices();

        HandResult result = new HandResult()
        {
            dices = diceList.ToList(),
            effectiveDices = effectiveDices.ToList(),
            baseScore = GetCurrentMultipliedScore(),
            isAchived = IsAchived(diceList)
        };

        return result;
    }
}
