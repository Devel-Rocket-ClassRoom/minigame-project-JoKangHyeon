using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopConsumable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image image;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public GameObject buyedImage;

    public GameManager gameManager;

    Button button;
    Consumable consumable;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        if (gameManager == null)
            gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
    }

    public void SetConsumable(Consumable consumable, float multiplier)
    {
        this.consumable = consumable;

        image.sprite = consumable.sprite;
        nameText.text = consumable.Name;
        costText.text = Mathf.RoundToInt((consumable.cost * multiplier)).ToString();

        buyedImage.SetActive(false);
        button.interactable = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (consumable == null) return;

        gameManager?.tooltip.ShowConsumableTooltip(consumable);
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
