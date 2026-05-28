
using System;
using UnityEngine;

[Serializable]
public abstract class Card
{
    public string nameStringKey;
    public virtual string Name { get { return StringTable.GetString(nameStringKey); } }
    public string descriptionStringKey;
    public virtual string Description { get { return StringTable.GetString(descriptionStringKey); } }
    public int cost;
    public Defines.Rarity rarity;

    public Sprite sprite;


    public Card Clone()
    {
        Card clonedCard = CloneInstance();
        clonedCard.nameStringKey = nameStringKey;
        clonedCard.descriptionStringKey = descriptionStringKey;
        clonedCard.cost = cost;
        clonedCard.rarity = rarity;
        clonedCard.sprite = sprite;

        return clonedCard;
    }

    protected abstract Card CloneInstance();

    public abstract void OnObtain(GameManager gameManager);

    public virtual void OnCycleStart() { }
    public virtual void OnRoundStart() { }

    /// <summary>
    /// 상점 진열 시 호출. 진열 슬롯마다 별도 Clone 위에서 호출되므로 SO 원본 오염 없음.
    /// 무작위 타겟 결정, description 갱신 등 진열 시점 초기화를 여기서 수행.
    /// </summary>
    public virtual void OnDisplay(RunState run) { }

    /// <summary>
    /// 구매 가능 여부 반환. false면 ShopCard가 회색 처리 + 구매 차단.
    /// </summary>
    public virtual bool CanBuy(RunState run) => true;
}