
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public abstract class Dice
{
    public string name;

    public abstract int GetDice();
    public abstract void SetDice(int number);
    
    public abstract int RollDice();
    public abstract void ResetDice();

    public abstract void ForceSetDice(int number);

    public abstract Dice Clone();

    public abstract void TrySetDice(int value);
}

[Serializable]
public class NormalDice : Dice
{
    [SerializeField]
    int diceResult;
    [SerializeField]
    public List<int> faces = new(){ 1, 2, 3, 4, 5, 6 };
    [SerializeField]
    public bool rolled;

    public override void ForceSetDice(int number)
    {
        diceResult = number;
    }

    public override int GetDice()
    {
        return diceResult;
    }

    public override void ResetDice()
    {
        rolled = false;
    }

    public override int RollDice()
    {
        SetDice(Random.Range(0, faces.Count));
        return diceResult;
    }

    public override void SetDice(int faceIndex)
    {
        diceResult = faces[faceIndex];
    }

    public override void TrySetDice(int value)
    {
        if (faces.Contains(value))
        {
            diceResult = value;
        }
    }

    public override Dice Clone()
    {
        NormalDice result = new NormalDice();
        
        result.diceResult = diceResult;
        result.rolled = rolled;
        result.faces = new();
        foreach(var face in faces)
        {
            result.faces.Add(face);
        }

        return result;
    }
}