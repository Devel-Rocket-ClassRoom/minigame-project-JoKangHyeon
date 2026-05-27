using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class ShopCanvas : MonoBehaviour
{
    public GameManager gameManager;
    public ShopState currentState;

    public List<ShopCard> shopCards;
    public List<ShopRelic> shopRelics;

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


        float multiplier = currentState.GetMultiplier();

        for(int i=0; i < shopState.cards.Count && i<shopCards.Count; i++)
        {
            shopCards[i].SetCard(shopState.cards[i], multiplier);
            shopCards[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < shopState.relics.Count && i < shopRelics.Count; i++)
        {
            shopRelics[i].SetRelic(shopState.relics[i],multiplier);
            shopRelics[i].gameObject.SetActive(true);
        }

        rerollCostText.text = currentState.GetRerollCost().ToString();
    }

    public void BuyCard(int pos)
    {

    }

    public void BuyRelic(int pos)
    {

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
