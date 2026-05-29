using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    enum TooltipMode
    {
        None,
        DiceResult,
        Hand,
        Card,
        Relic
    }

    TooltipMode currentMode = TooltipMode.None;

    [Header("Dice Result")]
    public GameObject diceResultTooltip;
    public FaceView faceView;
    public TextMeshProUGUI diceNameText;

    [Header("Hand")]
    public GameObject handTooltip;
    public TextMeshProUGUI handNameText;
    public TextMeshProUGUI handDescriptionText;
    public TextMeshProUGUI handLevelText;
    public TextMeshProUGUI handMultiplierText;
    public TextMeshProUGUI handAchievedText;

    [Header("Card")]
    public GameObject cardTooltip;
    public Image cardImage;
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI cardDescriptionText;

    [Header("Relic")]
    public GameObject relicTooltip;
    public Image relicImage;
    public TextMeshProUGUI relicNameText;
    public TextMeshProUGUI relicDescText;
    public TextMeshProUGUI relicFlavorText;



    const string c_levelTextFormat = "Lv.{0}";
    const string c_handMultiplierFormatStringKey = "hand_multiplier_formatString";
    const string c_handAchivedSuccessStringKey = "hand_achieved_success";
    const string c_handAchivedFailStringKey = "hand_achieved_fail";


    private void Update()
    {
        transform.position = Mouse.current.position.value;
    }

    public void HideAllSubTooltips()
    {
        diceResultTooltip.SetActive(false);
        handTooltip.SetActive(false);
        cardTooltip.SetActive(false);
        relicTooltip.SetActive(false);
    }

    public void ShowDiceTooltip(Dice dice)
    {
        if (dice == null) return;

        currentMode = TooltipMode.DiceResult;
        HideAllSubTooltips();
        transform.position = Mouse.current.position.value;

        diceResultTooltip.SetActive(true);
        if(dice.diceResultIndex == -1)
        {
            faceView.faceText.text = "?";
        }
        else
        {
            faceView.Set(dice.GetFace());
        }

        gameObject.SetActive(true);
    }

    public void ShowHandSlotTooltip(HandSlot slot)
    {
        currentMode = TooltipMode.Hand;
        HideAllSubTooltips();
        transform.position = Mouse.current.position.value;

        handTooltip.SetActive(true);
        handNameText.text = StringTable.GetString(slot.hand.name);
        handDescriptionText.text = StringTable.GetString(slot.hand.description);

        Debug.Log(slot.hand.description);

        handLevelText.text = string.Format(c_levelTextFormat, slot.slotLevel);
        handMultiplierText.text = string.Format(StringTable.GetString(c_handMultiplierFormatStringKey), slot.hand.ScoreMultiplier);
        handAchievedText.text = slot.hand.IsAchived() ? StringTable.GetString(c_handAchivedSuccessStringKey) : StringTable.GetString(c_handAchivedFailStringKey);

        gameObject.SetActive(true);
    }

    public void ShowCardTooltip(Card card)
    {
        currentMode = TooltipMode.Card;
        HideAllSubTooltips();
        transform.position= Mouse.current.position.value;

        cardTooltip.SetActive(true);
        cardNameText.text = card.Name;
        cardDescriptionText.text = card.Description;
        cardImage.sprite = card.sprite;

        gameObject.SetActive(true);
    }

    public void ShowRelicTooltip(Relic relic)
    {
        currentMode = TooltipMode.Relic;
        HideAllSubTooltips();
        transform.position = Mouse.current.position.value;

        relicTooltip.SetActive(true);
        relicNameText.text = StringTable.GetString(relic.name);
        relicDescText.text = StringTable.GetString(relic.description);
        relicFlavorText.text = StringTable.GetString(relic.flavorText);
        relicImage.sprite = relic.sprite;

        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}
