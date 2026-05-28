using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image cardImage;
    public TextMeshProUGUI costText;
    public GameManager gameManager;
    public GameObject buyedImage;


    Card card;
    Button button;
    Relic relic;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        if(gameManager == null)
        {
            gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        }
    }

    public void SetCard(Card card, float multiplier, RunState run)
    {
        this.card = card;

        cardImage.sprite = card.sprite;
        costText.text = Mathf.RoundToInt((card.cost * multiplier)).ToString();

        bool canBuy = card.CanBuy(run);
        Color tint = canBuy ? Color.white : Color.gray;
        cardImage.color = tint;
        costText.color = tint;

        buyedImage.SetActive(false);
        button.interactable = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (card == null) return;

        gameManager?.tooltip.ShowCardTooltip(card);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameManager?.tooltip.HideTooltip();
    }

    public void SetBuyed()
    {
        buyedImage.SetActive(true);
        button.interactable = false;
    }
}
