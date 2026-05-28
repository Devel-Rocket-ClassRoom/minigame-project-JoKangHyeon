using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopRelic : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image image;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public GameObject buyedImage;

    public GameManager gameManager;

    Button button;
    Relic relic;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        if(gameManager==null)
            gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
    }

    public void SetRelic(Relic relic, float multiplier)
    {
        this.relic = relic;

        image.sprite = relic.sprite;
        nameText.text = StringTable.GetString(relic.name);
        costText.text = Mathf.RoundToInt((relic.cost * multiplier)).ToString();

        buyedImage.SetActive(false);
        button.interactable = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (relic == null) return;

        gameManager?.tooltip.ShowRelicTooltip(relic);
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
