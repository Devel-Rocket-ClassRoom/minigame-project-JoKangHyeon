
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public abstract class Dice
{
    public string name;
    public List<DiceFace> faces = new();
    public DiceObject prefab;


    [SerializeField]
    public int diceResultIndex;

    public abstract DiceFace GetFace();
    public abstract int GetDice();
    public abstract void SetDice(int number);
    
    public abstract int RollDice();
    public abstract void ResetDice();

    public abstract void ForceSetDice(int number);

    public abstract Dice Clone();

    public abstract void TrySetDice(int value);

    public abstract void SetFace(int position, int value);
    public abstract void SetFaceValue(int position, int value);
}

[Serializable]
public class NormalDice : Dice
{
    DiceFace DiceResult
    {
        get
        {
            return isTempFace? tempFace : faces[diceResultIndex];
        }
    }

    [SerializeField]
    public bool rolled;

    DiceFace tempFace;
    public bool isTempFace;

    public NormalDice()
    {
        name = "Normal Dice";
        faces.Add(new DiceFace(1, this));
        faces.Add(new DiceFace(2, this));
        faces.Add(new DiceFace(3, this));
        faces.Add(new DiceFace(4, this));
        faces.Add(new DiceFace(5, this));
        faces.Add(new DiceFace(6, this));
    }

    public override void ForceSetDice(int number)
    {
        DiceFace diceFace = faces.Find(face => face.Value == number);
        if (diceFace != null) {
            diceResultIndex = faces.IndexOf(diceFace);
            isTempFace = false;
        }
        else
        {
            tempFace = new DiceFace(number, this);
            isTempFace = true;
        }
    }

    public override int GetDice()
    {
        if (DiceResult == null)
        {
            return -1;  
        }
        return DiceResult.Value;
    }

    public override void ResetDice()
    {
        rolled = false;

        foreach (DiceFace face in faces)
        {
            face.ResetForCycle();
        }
    }

    public override int RollDice()
    {
        SetDice(Random.Range(0, faces.Count));
        return DiceResult.Value;
    }

    public override void SetDice(int faceIndex)
    {
        diceResultIndex = faceIndex;
        isTempFace = false;
    }

    public override void TrySetDice(int value)
    {
        DiceFace diceFace = faces.Find(face => face.Value == value);
        if(diceFace != null)
        {
            diceResultIndex = faces.IndexOf(diceFace);
            isTempFace = false;
        }
    }

    public override Dice Clone()
    {
        NormalDice result = new NormalDice();
        result.rolled = rolled;
        result.faces = new();
        foreach(var face in faces)
        {
            result.faces.Add(face);
        }

        result.prefab = prefab;

        return result;
    }

    public override void SetFace(int position, int value)
    {
        faces[position] = new DiceFace(value, this);
        faces.Sort((a, b) => a.Value.CompareTo(b.Value));
    }

    public override void SetFaceValue(int position, int value)
    {
        faces[position].Value = value;
        faces.Sort((a,b) => a.Value.CompareTo(b.Value));
    }

    public override DiceFace GetFace()
    {
        return isTempFace ? tempFace : faces[diceResultIndex];
    }
}


public class DiceFace
{
    private int _value;
    public int Value
    {
        get
        {
            return valueOverriden ? overrideValue : _value;
        }

        set
        {
            _value = value;
        }
    }
    public Dice dice;

    public int overrideValue;
    public bool valueOverriden = false;

    public DiceFace(int value, Dice dice)
    {
        this.Value = value;
        this.dice = dice;
    }

    public void ResetForCycle()
    {
        valueOverriden = false;
    }

    public void OverrideValue(int value)
    {
        valueOverriden = true;
        overrideValue = value;
    }

    public Action<Dice, DiceFace> OnRolled;
    public Action<Dice, DiceFace> OnSelected;
}