using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        relicsBuy.Clear();

        cards.Clear();
        cardsBuy.Clear();

        consumables.Clear();
        consumablesBuy.Clear();

        #region Card
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
                    break;
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
                        if (!cards.Any(c=>c.Name.Equals(card.Name)))
                        {
                            rareityCards.Add(card);
                        }
                    }
                }

                if (rareityCards.Count > 0)
                {
                    // §6.1 — SO 원본 오염 방지: Clone 후 OnDisplay 호출
                    Card cloned = rareityCards[Random.Range(0, rareityCards.Count)].Clone();
                    cloned.OnDisplay(manager.currentRun);
                    cards.Add(cloned);
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

                    if (rarityIndex >= rarity.rarity.Count)
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
        #endregion

        #region Relic
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
                    break;
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
                        if (!gameManager.currentRun.relics.Any(c => c.nameStringKey.Equals(relic.nameStringKey)) && !relics.Any(c => c.nameStringKey.Equals(relic.nameStringKey)))
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

                    if (rarityIndex >= rarity.rarity.Count)
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

        #endregion

        #region Consumable
        if (gameManager.consumableDefine != null)
        {
            for (int randomConsumableCount = 0; randomConsumableCount < c_consumableAppear; randomConsumableCount++)
            {
                float r = Random.value;
                Defines.Rarity targetRarity = Defines.Rarity.Common;
                int rarityIndex = 0;
                for (; rarityIndex < rarity.rarity.Count; rarityIndex++)
                {
                    if (r < rarity.raritySpread[rarityIndex])
                    {
                        targetRarity = rarity.rarity[rarityIndex];
                        break;
                    }
                    else
                    {
                        r -= rarity.raritySpread[rarityIndex];
                    }
                }

                bool searchingFront = false;
                int originalRarityIndex = rarityIndex;
                List<Consumable> rareityConsumables = new();
                while (true)
                {
                    rareityConsumables.Clear();
                    foreach (var consumable in gameManager.consumableDefine.consumables)
                    {
                        if (consumable.rarity == targetRarity)
                        {
                            if (!consumables.Any(c => c.nameStringKey.Equals(consumable.nameStringKey)))
                            {
                                rareityConsumables.Add(consumable);
                            }
                        }
                    }

                    if (rareityConsumables.Count > 0)
                    {
                        // §6.1 — SO 원본 오염 방지: Clone 후 추가
                        consumables.Add(rareityConsumables[Random.Range(0, rareityConsumables.Count)].Clone());
                        consumablesBuy.Add(false);
                        break;
                    }
                    else
                    {
                        rarityIndex += searchingFront ? -1 : 1;

                        if (rarityIndex < 0)
                        {
                            break;
                        }

                        if (rarityIndex >= rarity.rarity.Count)
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
        }
        #endregion
    }

    public bool Reroll()
    {
        if (manager.currentRun.Coin < GetRerollCost())
            return false;

        manager.currentRun.Coin -= GetRerollCost();
        rerollCount++;

        ShopRarityDefinitionSO rarity = manager.rarityDefine;

        #region Card
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
                    break;
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
                        // 카드는 중복 획득 허용 — 보유 카드는 제외하지 않고, 같은 상점 내 중복만 방지
                        if (!cards.Any(c => c.Name.Equals(card.Name)))
                        {
                            rareityCards.Add(card);
                        }
                    }
                }

                if (rareityCards.Count > 0)
                {
                    // §6.1 — Reroll 시에도 Clone + OnDisplay
                    Card cloned = rareityCards[Random.Range(0, rareityCards.Count)].Clone();
                    cloned.OnDisplay(manager.currentRun);
                    cards[randomCardCount] = cloned;
                    break;
                }
                else
                {
                    rarityIndex += searchingFront ? -1 : 1;

                    if (rarityIndex < 0)
                    {
                        break;
                    }

                    if (rarityIndex >= rarity.rarity.Count)
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
        #endregion

        #region Relic
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

                    break;
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
                        if (!manager.currentRun.relics.Any(c => c.nameStringKey.Equals(relic.nameStringKey)) && !relics.Any(c => c.nameStringKey.Equals(relic.nameStringKey)))
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

                    if (rarityIndex >= rarity.rarity.Count)
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
        if (manager.consumableDefine != null)
        {
            for (int i = 0; i < consumables.Count; i++)
            {
                if (consumablesBuy[i]) continue;

                float r = Random.value;
                Defines.Rarity targetRarity = Defines.Rarity.Common;
                int rarityIndex = 0;
                for (; rarityIndex < rarity.rarity.Count; rarityIndex++)
                {
                    if (r < rarity.raritySpread[rarityIndex])
                    {
                        targetRarity = rarity.rarity[rarityIndex];
                        break;
                    }
                    else
                    {
                        r -= rarity.raritySpread[rarityIndex];
                    }
                }

                bool searchingFront = false;
                int originalRarityIndex = rarityIndex;
                List<Consumable> candidates = new();
                while (true)
                {
                    candidates.Clear();
                    foreach (var c in manager.consumableDefine.consumables)
                    {
                        if (c.rarity == targetRarity)
                            candidates.Add(c);
                    }

                    if (candidates.Count > 0)
                    {
                        consumables[i] = candidates[Random.Range(0, candidates.Count)].Clone();
                        break;
                    }
                    else
                    {
                        rarityIndex += searchingFront ? -1 : 1;
                        if (rarityIndex < 0) break;
                        if (rarityIndex >= rarity.rarity.Count)
                        {
                            searchingFront = true;
                            rarityIndex = originalRarityIndex - 1;
                            if (rarityIndex < 0) break;
                        }
                        targetRarity = rarity.rarity[rarityIndex];
                    }
                }
            }
        }
        #endregion
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
