using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class NumbersHand : Hand
{
    private int numTarget;

    public NumbersHand(int num)
    {
        numTarget = num;
    }

    public override int GetDiceScore(List<Dice> dices)
    {
        if(dices == null) 
            return 0;

        int score = 0;
        foreach (Dice dice in dices)
        {
            if (dice.GetDice() == numTarget)
            {
                score += numTarget;
            }
        }

        return score;
    }

}

public class ChoiceHand : Hand
{
    public override int GetDiceScore(List<Dice> dices)
    {
        if (dices == null)
            return 0;

        int score = 0;
        foreach (Dice dice in dices)
        {
            score += dice.GetDice();
        }

        return score;
    }
}

public class FullHouseHand : Hand
{
    public override int GetDiceScore(List<Dice> dices)
    {
        if (dices == null || dices.Count<5)
            return 0;

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
            return 0;

        int threeCardResult = threeCard.Max();
        twoCard.Remove(threeCardResult);
        int twoCardResult = twoCard.Max();

        return threeCardResult * 3 + twoCardResult * 2;
    }
}

public class SmallAlighmentHand : Hand
{
    public override int GetDiceScore(List<Dice> dices)
    {
        if (dices == null || dices.Count<4)
            return 0;
        int firstDice = dices[0].GetDice();
        int secondDice = dices[1].GetDice();

        int firstCount = dices.FindAll(d=>d.GetDice()==firstDice).Count();
        int secondCount = dices.FindAll(d => d.GetDice() == secondDice).Count();

        if (firstCount >= 4)
        {
            return firstDice * firstCount;
        }
        else if (secondCount >= 4)
        {
            return secondDice * secondCount;
        }
        else
        {
            return 0;
        }
    }
}

public class BigAlighmentHand : Hand
{
    public override int GetDiceScore(List<Dice> dices)
    {
        if (dices == null || dices.Count < 5)
            return 0;

        int firstDice = dices[0].GetDice();
        int firstCount = dices.FindAll(d => d.GetDice() == firstDice).Count();

        if (firstCount >= 5)
        {
            return firstDice * firstCount;
        }
        else
        {
            return 0;
        }
    }
}

public class SmallStraight : Hand
{
    public override int GetDiceScore(List<Dice> dices)
    {
        dices.Sort((a,b)=>a.GetDice().CompareTo(b.GetDice()));

        int straight = 1;
        int last = dices[dices.Count-1].GetDice();
        List<int> straights = new List<int>();
        for (int i = dices.Count - 2; i >= 0; i--)
        {
            if (dices[i].GetDice() == last)
            {
                continue;
            }
            else if (dices[i].GetDice() != last - 1)
            {
                straight = 1;
                straights.Clear();
            }
            else
            {
                straight++;
                if (straight == 4)
                {
                    straights.Add(dices[i].GetDice());
                    break;
                }
            }
            straights.Add(dices[i].GetDice());
            last = dices[i].GetDice();
        }

        if(straight >= 4)
        {
            return straights.Sum();
        }
        return 0;
    }
}

public class BigStraightHand : Hand
{
    public override int GetDiceScore(List<Dice> dices)
    {
        dices.Sort((a, b) => a.GetDice().CompareTo(b.GetDice()));

        int straight = 1;
        int last = dices[dices.Count - 1].GetDice();
        List<int> straights = new List<int>();
        for (int i = dices.Count - 2; i >= 0; i--)
        {
            if (dices[i].GetDice() == last)
            {
                continue;
            }
            else if (dices[i].GetDice() != last - 1)
            {
                straight = 1;
                straights.Clear();
            }
            else
            {
                straight++;
                if (straight == 5)
                {
                    straights.Add(dices[i].GetDice());
                    break;
                }
            }
            straights.Add(dices[i].GetDice());
            last = dices[i].GetDice();
        }

        if (straight >= 5)
        {
            return straights.Sum();
        }
        return 0;
    }
}