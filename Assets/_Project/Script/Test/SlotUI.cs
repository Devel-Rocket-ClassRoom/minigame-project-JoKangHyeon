
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI diceText;
    public TextMeshProUGUI scoreText;
    public Button setButton;

    public bool detailView = false;

    private HandSlot slot;

    private List<Dice> currentTemp;

    private void Update()
    {
        if (Keyboard.current.leftShiftKey.wasPressedThisFrame)
        {
            detailView = true;
            Refresh(slot, currentTemp);
        }

        if (Keyboard.current.leftShiftKey.wasReleasedThisFrame)
        {
            detailView = false;
            Refresh(slot, currentTemp);
        }
    }


    public void Refresh(HandSlot slot, List<Dice> tempView)
    {
        this.slot = slot;

        nameText.text = StringTable.GetString(slot.hand.name);
        diceText.text = slot.hand.GetDicesString();
        currentTemp = tempView;

        if (slot.hand.Setted)
        {
            scoreText.color = Color.black;
            if (detailView)
            {
                scoreText.text = slot.hand.GetCurrentDetailedString();
            }
            else
            {
                scoreText.text = slot.hand.GetCurrentMultipliedScore().ToString();
            }
        }
        else
        {
            scoreText.color = Color.blue;
            
            if (tempView == null)
            {
                scoreText.text = "0";
                return;
            }

            if (detailView)
            {
                scoreText.text = slot.hand.GetDetailedString(tempView);
            }
            else
            {
                scoreText.text = slot.hand.GetMultipliedScore(tempView).ToString();
            }
        }
    }
}
