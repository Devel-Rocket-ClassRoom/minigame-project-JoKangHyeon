using System.Collections.Generic;
using UnityEngine;

public class ActiveItemPanel : MonoBehaviour
{
    public List<ActiveItemButton> buttons;

    public ActiveItemButton buttonPrefab;
    public Transform buttonParent;

    List<Consumable > consumables;
    GameManager gameManager;

    public void Refresh(GameManager gameManager, List<Consumable> consumables, int slotCount)
    {
        this.consumables = consumables;
        this.gameManager = gameManager;

        foreach(var button in buttons)
        {
            button.gameObject.SetActive(false);
        }

        for (int i = 0; i < slotCount; i++)
        {
            if (i >= buttons.Count)
            {
                ActiveItemButton newButton = Instantiate(buttonPrefab, buttonParent);
                buttons.Add(newButton);
            }
            else
            {
                buttons[i].gameObject.SetActive(true);
            }

            ActiveItemButton button = buttons[i];

            if (i >= consumables.Count)
            {
                button.Set(gameManager, null, i, this);
            }
            else
            {
                button.Set(gameManager, consumables[i], i, this);    
            }
        }
    }

    public void UseItem(int slot)
    {
        gameManager.currentRun.UseConsumable(consumables[slot]);
    }
}
