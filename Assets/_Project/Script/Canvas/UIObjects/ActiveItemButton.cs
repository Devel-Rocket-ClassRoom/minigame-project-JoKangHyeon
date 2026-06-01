using UnityEngine;
using UnityEngine.UI;

public class ActiveItemButton : MonoBehaviour
{
    public Button button;
    public Image image;

    public int slot;
    public ActiveItemCanvas canvas;
    public void Set(Consumable consumable, int slot, ActiveItemCanvas canvas)
    {
        this.slot = slot;
        this.canvas = canvas;

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
}
