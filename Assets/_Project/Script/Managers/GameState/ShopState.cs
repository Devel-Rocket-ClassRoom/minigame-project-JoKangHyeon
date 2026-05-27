using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Defines;

public class ShopState
{

    public List<Relic> relics;
    public List<bool> relicsBuy = new();
    public List<Card> cards;
    public List<bool> cardsBuy = new();
    public List<Consumable> consumables;
    public List<bool> consumablesBuy = new();

    GameManager manager;

    public int rerollCount = 0;


    const int c_relicAppear = 4;
    const int c_consumableAppear = 4;
    const int c_cardAppear = 5;
    public int c_baseRerollCost = 2;

    public ShopState()
    {
        relics = new();
        cards = new();
        consumables = new();
    }

    public void Init(GameManager gameManager)
    {
        manager = gameManager;
        ShopRarityDefinitionSO rarity = gameManager.rarityDefine;

        relics.Clear();

        for (int randomCardCount = 0; randomCardCount < c_cardAppear; randomCardCount++)
        {
            float r = Random.value;
            Defines.Rarity targetRarity = Defines.Rarity.Common;
            int rarityIndex = 0;
            for (; rarityIndex < rarity.rarity.Count; rarityIndex++)
            {
                if (r < rarity.raritySpread[rarityIndex])
                {
                    targetRarity = rarity.rarity[rarityIndex];
                }
                else
                {
                    r -= rarity.raritySpread[rarityIndex];
                }
            }

            bool searchingFront = false;
            int originalRarityIndex = rarityIndex;
            List<Card> rareityCards = new List<Card>();
            while (true)
            {
                rareityCards.Clear();
                foreach (var card in gameManager.cardDefine.cards)
                {
                    if (card.rarity == targetRarity)
                    {
                        if (!gameManager.currentRun.cards.Any(c => c.name.Equals(card.name)) && !cards.Any(c=>c.name.Equals(card.name)))
                        {
                            rareityCards.Add(card);
                        }
                    }
                }

                if (rareityCards.Count > 0)
                {
                    cards.Add(rareityCards[Random.Range(0, rareityCards.Count)]);
                    cardsBuy.Add(false);
                    break;
                }
                else
                {
                    rarityIndex += searchingFront ? -1 : 1;

                    if (rarityIndex < 0)
                    {
                        break;
                    }

                    if (rarityIndex > rarity.rarity.Count)
                    {
                        searchingFront = true;
                        rarityIndex = originalRarityIndex - 1;
                        if (rarityIndex < 0)
                            break;
                    }

                    targetRarity = rarity.rarity[rarityIndex];
                }
            }
        }

        for (int randomRelicCount = 0; randomRelicCount < c_relicAppear; randomRelicCount++)
        {
            float r = Random.value;
            Defines.Rarity targetRarity = Defines.Rarity.Common;
            int rarityIndex = 0;
            for (; rarityIndex < rarity.rarity.Count; rarityIndex++)
            {
                if (r < rarity.raritySpread[rarityIndex])
                {
                    targetRarity = rarity.rarity[rarityIndex];
                }
                else
                {
                    r -= rarity.raritySpread[rarityIndex];
                }
            }

            bool searchingFront = false;
            int originalRarityIndex = rarityIndex;
            List<Relic> rareityRelics = new ();
            while (true)
            {
                rareityRelics.Clear();
                foreach (var relic in gameManager.relicDefine.relics)
                {
                    if (relic.rarity == targetRarity)
                    {
                        if (!gameManager.currentRun.relics.Any(c => c.name.Equals(relic.name)) && !relics.Any(c => c.name.Equals(relic.name)))
                        {
                            rareityRelics.Add(relic);
                        }
                    }
                }

                if (rareityRelics.Count > 0)
                {
                    relics.Add(rareityRelics[Random.Range(0, rareityRelics.Count)]);
                    relicsBuy.Add(false);
                    break;
                }
                else
                {
                    rarityIndex += searchingFront ? -1 : 1;

                    if (rarityIndex < 0)
                    {
                        break;
                    }

                    if (rarityIndex > rarity.rarity.Count)
                    {
                        searchingFront = true;
                        rarityIndex = originalRarityIndex - 1;
                        if (rarityIndex < 0)
                            break;
                    }

                    targetRarity = rarity.rarity[rarityIndex];
                }
            }
        }

        //TODO : 소모품도 생성

    }

    public bool Reroll()
    {
        if (manager.currentRun.Coin < GetRerollCost())
            return false;

        manager.currentRun.Coin -= GetRerollCost();
        rerollCount++;

        ShopRarityDefinitionSO rarity = manager.rarityDefine;

        for (int randomCardCount = 0; randomCardCount < c_cardAppear; randomCardCount++)
        {
            if (cardsBuy[randomCardCount])
                continue;

            float r = Random.value;
            Defines.Rarity targetRarity = Defines.Rarity.Common;
            int rarityIndex = 0;
            for (; rarityIndex < rarity.rarity.Count; rarityIndex++)
            {
                if (r < rarity.raritySpread[rarityIndex])
                {
                    targetRarity = rarity.rarity[rarityIndex];
                }
                else
                {
                    r -= rarity.raritySpread[rarityIndex];
                }
            }

            bool searchingFront = false;
            int originalRarityIndex = rarityIndex;
            List<Card> rareityCards = new List<Card>();
            while (true)
            {
                rareityCards.Clear();
                foreach (var card in manager.cardDefine.cards)
                {
                    if (card.rarity == targetRarity)
                    {
                        if (!manager.currentRun.cards.Any(c => c.name.Equals(card.name)) && !cards.Any(c => c.name.Equals(card.name)))
                        {
                            rareityCards.Add(card);
                        }
                    }
                }

                if (rareityCards.Count > 0)
                {
                    cards[randomCardCount] = rareityCards[Random.Range(0, rareityCards.Count)];
                    break;
                }
                else
                {
                    rarityIndex += searchingFront ? -1 : 1;

                    if (rarityIndex < 0)
                    {
                        break;
                    }

                    if (rarityIndex > rarity.rarity.Count)
                    {
                        searchingFront = true;
                        rarityIndex = originalRarityIndex - 1;
                        if (rarityIndex < 0)
                            break;
                    }

                    targetRarity = rarity.rarity[rarityIndex];
                }
            }
        }

        for (int randomRelicCount = 0; randomRelicCount < c_relicAppear; randomRelicCount++)
        {
            if (relicsBuy[randomRelicCount])
                continue;

            float r = Random.value;
            Defines.Rarity targetRarity = Defines.Rarity.Common;
            int rarityIndex = 0;
            for (; rarityIndex < rarity.rarity.Count; rarityIndex++)
            {
                if (r < rarity.raritySpread[rarityIndex])
                {
                    targetRarity = rarity.rarity[rarityIndex];
                }
                else
                {
                    r -= rarity.raritySpread[rarityIndex];
                }
            }

            bool searchingFront = false;
            int originalRarityIndex = rarityIndex;
            List<Relic> rareityRelics = new();
            while (true)
            {
                rareityRelics.Clear();
                foreach (var relic in manager.relicDefine.relics)
                {
                    if (relic.rarity == targetRarity)
                    {
                        if (!manager.currentRun.relics.Any(c => c.name.Equals(relic.name)) && !relics.Any(c => c.name.Equals(relic.name)))
                        {
                            rareityRelics.Add(relic);
                        }
                    }
                }

                if (rareityRelics.Count > 0)
                {
                    relics[randomRelicCount]=rareityRelics[Random.Range(0, rareityRelics.Count)];
                    break;
                }
                else
                {
                    rarityIndex += searchingFront ? -1 : 1;

                    if (rarityIndex < 0)
                    {
                        break;
                    }

                    if (rarityIndex > rarity.rarity.Count)
                    {
                        searchingFront = true;
                        rarityIndex = originalRarityIndex - 1;
                        if (rarityIndex < 0)
                            break;
                    }

                    targetRarity = rarity.rarity[rarityIndex];
                }
            }
        }
        return true;
    }

    public int GetRerollCost()
    {
        return Mathf.RoundToInt(c_baseRerollCost * (rerollCount + 1) * GetMultiplier());
    }

    public float GetMultiplier()
    {
        return Mathf.Max(1f, Mathf.Sqrt(manager.currentRun.level / Defines.c_levelPerGroup));
    }
}
