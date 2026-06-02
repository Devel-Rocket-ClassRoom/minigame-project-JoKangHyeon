using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopCanvas : MonoBehaviour
{
    public GameManager gameManager;
    public ShopState currentState;

    public List<ShopCard> shopCards;
    public List<ShopRelic> shopRelics;
    public List<ShopConsumable> shopConsumables;

    public TextMeshProUGUI rerollCostText;


    public void Init(GameManager gameManager, ShopState shopState)
    {
        currentState = shopState;
        foreach (ShopCard card in shopCards)
        {
            card.gameObject.SetActive(false);
        }

        foreach(ShopRelic relic in shopRelics)
        {
            relic.gameObject.SetActive(false);
        }

        foreach (ShopConsumable consumable in shopConsumables)
        {
            consumable.gameObject.SetActive(false);
        }


        float multiplier = currentState.GetMultiplier();

        for(int i=0; i < shopState.cards.Count && i<shopCards.Count; i++)
        {
            shopCards[i].gameObject.SetActive(true);

            if (!shopState.cardsBuy[i])
                shopCards[i].SetCard(shopState.cards[i], multiplier, gameManager.currentRun);
        }

        for (int i = 0; i < shopState.relics.Count && i < shopRelics.Count; i++)
        {
            shopRelics[i].gameObject.SetActive(true);

            if (!shopState.relicsBuy[i])
                shopRelics[i].SetRelic(shopState.relics[i],multiplier);
        }

        for (int i = 0; i < shopState.consumables.Count && i < shopConsumables.Count; i++)
        {
            shopConsumables[i].gameObject.SetActive(true);

            if (!shopState.consumablesBuy[i])
                shopConsumables[i].SetConsumable(shopState.consumables[i], multiplier);
        }

        rerollCostText.text = string.Format(GameManager.c_skipCoinTextFormat, -currentState.GetRerollCost());
    }

    public void BuyCard(int pos)
    {
        if (currentState.cardsBuy[pos])
            return;

        Card card = currentState.cards[pos];

        // §2.3 — 구매 조건 미충족 시 구매 불가
        if (!card.CanBuy(gameManager.currentRun))
            return;

        if(gameManager.currentRun.Coin >= Mathf.RoundToInt((card.cost * currentState.GetMultiplier())))
        {
            gameManager.currentRun.Coin -= Mathf.RoundToInt((card.cost * currentState.GetMultiplier()));
            gameManager.currentRun.GetCard(card);
            currentState.cardsBuy[pos] = true;

            shopCards[pos].SetBuyed();
            gameManager.RefreshUI();
        }
    }

    public void BuyRelic(int pos)
    {
        if (currentState.relicsBuy[pos])
            return;

        if (gameManager.currentRun.Coin >= Mathf.RoundToInt((currentState.relics[pos].cost * currentState.GetMultiplier())))
        {
            gameManager.currentRun.Coin -= Mathf.RoundToInt((currentState.relics[pos].cost * currentState.GetMultiplier()));
            gameManager.currentRun.GetRelic(currentState.relics[pos]);
            currentState.relicsBuy[pos] = true;

            shopRelics[pos].SetBuyed();
            gameManager.RefreshUI();
        }
    }

    public void BuyConsumable(int pos)
    {
        if (currentState.consumablesBuy[pos])
            return;

        if(!gameManager.currentRun.CanAddConsumable())
            return;

        if (gameManager.currentRun.Coin >= Mathf.RoundToInt((currentState.consumables[pos].cost * currentState.GetMultiplier())))
        {
            gameManager.currentRun.Coin -= Mathf.RoundToInt((currentState.consumables[pos].cost * currentState.GetMultiplier()));
            gameManager.currentRun.AddConsumable(currentState.consumables[pos]);
            currentState.consumablesBuy[pos] = true;

            shopConsumables[pos].SetBuyed();
            gameManager.RefreshUI();
        }
    }

    public void Reroll()
    {
        if(currentState.Reroll())
            Init(gameManager, currentState);
    }

    public void CloseShopButton()
    {
        gameManager.ShowGame();
    }
}
