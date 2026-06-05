using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RelicView : MonoBehaviour , IPointerEnterHandler,IPointerExitHandler
{
    public Image image;
    Relic relic;
    GameManager gameManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        gameManager.tooltip.ShowRelicTooltip(relic);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameManager.tooltip.HideTooltip();
    }

    public void Refresh(Relic relic, GameManager gameManager)
    {
        this.relic = relic;
        this.gameManager = gameManager;

        image.sprite = relic.sprite;
    }


}
