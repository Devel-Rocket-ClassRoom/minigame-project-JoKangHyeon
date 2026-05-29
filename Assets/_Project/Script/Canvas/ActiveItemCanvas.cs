using System.Collections.Generic;
using UnityEngine;

public class ActiveItemCanvas : MonoBehaviour
{
    public List<ActiveItemButton> buttons;

    public ActiveItemButton buttonPrefab;
    public Transform buttonParent;

    List<Consumable > consumables;

    public void Refresh(List<Consumable> consumables, int slotCount)
    {
        this.consumables = consumables;

        foreach(var button in buttons)
        {
            button.gameObject.SetActive(false);
        }

        for (int i = 0; i < slotCount; i++)
        {
            if (i <= buttons.Count)
            {
                ActiveItemButton newButton = Instantiate(buttonPrefab, buttonParent);
                buttons.Add(newButton);
            }

            ActiveItemButton button = buttons[i];

            if (i >= consumables.Count)
            {
                //button.s
            }
        }
    }
}
