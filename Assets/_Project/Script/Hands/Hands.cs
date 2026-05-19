using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class NumbersHand : Hand
{
    public int numTarget;

    public NumbersHand(int num)
    {
        numTarget = num;
    }

    public override Hand Clone()
    {
        return new NumbersHand(numTarget);
    }

    public override int GetDiceScore(List<Dice> dices)
    {
        return GetEffectiveDices(dices).Sum(d => d.GetDice());
    }

    public override List<Dice> GetEffectiveDices(List<Dice> dices)
    {
        if (dices == null)
            return new List<Dice>();

        return dices.Where(d => d.GetDice() == numTarget).ToList();
    }


}

public class ChoiceHand : Hand
{
    public override Hand Clone()
    {
        return new ChoiceHand();
    }

    public override int GetDiceScore(List<Dice> dices)
    {
        return GetEffectiveDices(dices).Sum(d => d.GetDice());
    }

    public override List<Dice> GetEffectiveDices(List<Dice> dices)
    {
        if (dices == null)
            return new List<Dice>();

        return dices;
    }
}

public class FullHouseHand : Hand
{
    public override Hand Clone()
    {
        return new FullHouseHand();
    }

    public override int GetDiceScore(List<Dice> dices)
    {
        return GetEffectiveDices (dices).Sum(d => d.GetDice());
    }

    public override List<Dice> GetEffectiveDices(List<Dice> dices)
    {
        if (dices == null)
            return new List<Dice>();


        Dictionary<int, int> diceDict = new();

        HashSet<int> threeCard = new();
        HashSet<int> twoCard = new();
        foreach (Dice dice in dices)
        {
            int diceValue = dice.GetDice();
            if (!diceDict.ContainsKey(diceValue))
            {
                diceDict.Add(diceValue, 0);
            }
            diceDict[diceValue]++;

            if (diceDict[diceValue] == 2)
            {
                twoCard.Add(diceValue);
            }

            if (diceDict[diceValue] == 3)
            {
                threeCard.Add(diceValue);
            }
        }

        if (threeCard.Count == 0 || twoCard.Count < 2)
            return new List<Dice>();

        return dices;
    }
}

public class SmallAlignmentHand : Hand
{
    public override Hand Clone()
    {
        return new SmallAlignmentHand();
    }

    public override int GetDiceScore(List<Dice> dices)
    {
        return GetEffectiveDices(dices).Sum(d => d.GetDice());
    }

    public override List<Dice> GetEffectiveDices(List<Dice> dices)
    {
        if (dices == null || dices.Count < 4)
            return new List<Dice>();

        int firstDice = dices[0].GetDice();
        int secondDice = dices[1].GetDice();

        int firstCount = dices.FindAll(d => d.GetDice() == firstDice).Count();
        int secondCount = dices.FindAll(d => d.GetDice() == secondDice).Count();

        if (firstCount >= 4)
        {
            return dices.Where(d => d.GetDice() == firstDice).Take(4).ToList();
        }
        else if (secondCount >= 4)
        {
            return dices.Where(d => d.GetDice() == secondDice).Take(4).ToList();
        }
        else
        {
            return new List<Dice>();
        }
    }
}

public class BigAlignmentHand : Hand
{
    public override Hand Clone()
    {
        return new BigAlignmentHand();
    }

    public override int GetDiceScore(List<Dice> dices)
    {
        return GetEffectiveDices(dices).Sum(d => d.GetDice());
    }

    public override List<Dice> GetEffectiveDices(List<Dice> dices)
    {
        if (dices == null || dices.Count < 5)
            return new List<Dice>();

        int firstDice = dices[0].GetDice();
        int firstCount = dices.FindAll(d => d.GetDice() == firstDice).Count();

        if (firstCount >= 5)
        {
            return dices;
        }
        else
        {
            return new List<Dice>();
        }
    }
}

public class SmallStraightHand : Hand
{
    public override Hand Clone()
    {
        return new SmallStraightHand();
    }

    public override int GetDiceScore(List<Dice> dices)
    {
        return GetEffectiveDices(dices).Sum(d => d.GetDice());
    }

    public override List<Dice> GetEffectiveDices(List<Dice> dices)
    {
        if (dices == null || dices.Count < 4)
            return new List<Dice>();

        dices = dices.ToList();//Clone List 안하면 화면 꼬임
        dices.Sort((a, b) => a.GetDice().CompareTo(b.GetDice()));

        int last = dices[dices.Count - 1].GetDice();
        List<Dice> straightDices = new List<Dice>();

        straightDices.Add(dices[dices.Count - 1]);

        for (int i = dices.Count - 2; i >= 0; i--)
        {
            if (dices[i].GetDice() == last)
            {
                continue;
            }
            else if (dices[i].GetDice() != last - 1)
            {
                straightDices.Clear();
            }
            else
            {
                straightDices.Add(dices[i]);
                last = dices[i].GetDice();

                if (straightDices.Count == 4)
                {
                    break;
                }
            }
        }

        if (straightDices.Count == 4)
        {
            return straightDices;
        }
        else
        {
            return new List<Dice>();
        }
    }
}

public class BigStraightHand : Hand
{
    public override Hand Clone()
    {
        return new BigStraightHand();
    }

    public override int GetDiceScore(List<Dice> dices)
    {
        return GetEffectiveDices(dices).Sum(d => d.GetDice());
    }

    public override List<Dice> GetEffectiveDices(List<Dice> dices)
    {
        if (dices == null || dices.Count < 4)
            return new List<Dice>();

        dices = dices.ToList();//Clone List 안하면 화면 꼬임
        dices.Sort((a, b) => a.GetDice().CompareTo(b.GetDice()));

        int last = dices[dices.Count - 1].GetDice();
        List<Dice> straightDices = new List<Dice>();

        straightDices.Add(dices[dices.Count - 1]);

        for (int i = dices.Count - 2; i >= 0; i--)
        {
            if (dices[i].GetDice() == last)
            {
                continue;
            }
            else if (dices[i].GetDice() != last - 1)
            {
                straightDices.Clear();
            }
            else
            {
                straightDices.Add(dices[i]);
                last = dices[i].GetDice();

                if (straightDices.Count == 5)
                {
                    break;
                }
            }
        }

        if (straightDices.Count == 5)
        {
            return straightDices;
        }
        else
        {
            return new List<Dice>();
        }
    }
}