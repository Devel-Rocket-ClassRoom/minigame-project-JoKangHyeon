using System;
using UnityEngine;


[Serializable]
public abstract class Consumable
{
    public string name;
    public string description;
    public string flavorText;
    public Sprite sprite;
    public Defines.Rarity rarity;
    public int cost;

    /// <summary>
    /// 소모품 발동. 발동 성공 시 true, 실패(가드) 시 false — false면 소비 안 함.
    /// </summary>
    public abstract bool OnUse(GameManager gameManager);

    public virtual void OnAdd(GameManager gameManager) { }

    public virtual void OnRemove() { }

    public Consumable Clone()
    {
        Consumable c = CloneInstance();
        c.name = name;
        c.description = description;
        c.flavorText = flavorText;
        c.sprite = sprite;
        c.rarity = rarity;
        c.cost = cost;
        return c;
    }

    protected abstract Consumable CloneInstance();
}
