
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
        DiceFace diceFace = faces.Find(face => face.value == number);
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
        return DiceResult.value;
    }

    public override void ResetDice()
    {
        rolled = false;
    }

    public override int RollDice()
    {
        SetDice(Random.Range(0, faces.Count));
        return DiceResult.value;
    }

    public override void SetDice(int faceIndex)
    {
        diceResultIndex = faceIndex;
        isTempFace = false;
    }

    public override void TrySetDice(int value)
    {
        DiceFace diceFace = faces.Find(face => face.value == value);
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
        faces.Sort((a, b) => a.value.CompareTo(b.value));
    }

    public override void SetFaceValue(int position, int value)
    {
        faces[position].value = value;
        faces.Sort((a,b) => a.value.CompareTo(b.value));
    }
}


public class DiceFace
{
    public int value;
    public Dice dice;

    public DiceFace(int value, Dice dice)
    {
        this.value = value;
        this.dice = dice;
    }

    public Action<Dice, DiceFace> OnRolled;
    public Action<Dice, DiceFace> OnSelected;
}