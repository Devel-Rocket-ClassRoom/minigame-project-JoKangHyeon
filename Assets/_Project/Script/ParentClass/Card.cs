
using System;
using UnityEngine;

[Serializable]
public abstract class Card
{
    public string name;
    public string description;
    public int cost;
    public Defines.Rarity rarity;

    public Sprite sprite;


    public abstract Card Clone();

    public abstract void OnObtain(GameManager gameManager);

    public virtual void OnCycleStart() { }
    public virtual void OnRoundStart() { }
}