using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActiveItemButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button button;
    public Image image;

    public int slot;
    public ActiveItemPanel canvas;

    Consumable consumable;
    GameManager gameManager;

    public void Set(GameManager gameManager, Consumable consumable, int slot, ActiveItemPanel canvas)
    {
        this.slot = slot;
        this.canvas = canvas;
        this.consumable = consumable;
        this.gameManager = gameManager;

        if(consumable == null)
        {
            button.interactable = false;
            image.sprite = null;
        }
        else
        {
            button.interactable = true;
            image.sprite = consumable.sprite;
        }
    }

    public void OnButtonClick()
    {
        canvas.UseItem(slot);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        gameManager.tooltip.ShowConsumableTooltip(consumable);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameManager.tooltip.HideTooltip();
    }
}
