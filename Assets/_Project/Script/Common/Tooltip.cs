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
        Relic,
        Consumable
    }

    TooltipMode currentMode = TooltipMode.None;

    [Header("Dice Result")]
    public GameObject diceResultTooltip;
    public FaceView faceView;
    public TextMeshProUGUI diceNameText;
    public ScrollRect effectRect;
    public Transform effectsContainer;
    public GameObject effectItemPrefab;

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

    [Header("Consumable")]
    public GameObject consumableTooltip;
    public Image consumableImage;
    public TextMeshProUGUI consumableNameText;
    public TextMeshProUGUI consumableDescriptionText;
    public TextMeshProUGUI consumableFlavorText;


    RectTransform rt;
    bool flippedX;
    bool flippedY;
    const float c_flipHysteresis = 24f;
    private InputAction scrollWheel;

    const string c_levelTextFormat = "Lv.{0}";
    const string c_handMultiplierFormatStringKey = "hand_multiplier_formatString";
    const string c_handAchivedSuccessStringKey = "hand_achieved_success";
    const string c_handAchivedFailStringKey = "hand_achieved_fail";


    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        scrollWheel = InputSystem.actions.FindAction("ScrollWheel");
    }

    private void Update()
    {
        RepositionTooltip();

        switch(currentMode)
        {
            case TooltipMode.DiceResult:
                effectRect.verticalNormalizedPosition -= scrollWheel.ReadValue<Vector2>().y * 0.1f;
                break;
        }
    }

    private void RepositionTooltip()
    {
        Vector2 mousePos = Mouse.current.position.value;
        Vector2 size = rt.rect.size;

        // 플립 진입: 경계 초과 시. 플립 해제: 반대 방향에 여유(hysteresis)가 생겼을 때만.
        if (!flippedX) { if (mousePos.x + size.x > Screen.width)        flippedX = true; }
        else           { if (mousePos.x + size.x <= Screen.width - c_flipHysteresis) flippedX = false; }

        if (!flippedY) { if (mousePos.y - size.y < 0)                   flippedY = true; }
        else           { if (mousePos.y - size.y >= c_flipHysteresis)    flippedY = false; }

        float x = flippedX ? mousePos.x - size.x : mousePos.x;
        float y = flippedY ? mousePos.y + size.y : mousePos.y;

        transform.position = new Vector2(x, y);
    }

    public void HideAllSubTooltips()
    {
        diceResultTooltip.SetActive(false);
        handTooltip.SetActive(false);
        cardTooltip.SetActive(false);
        relicTooltip.SetActive(false);
        consumableTooltip.SetActive(false);
    }

    public void ShowDiceTooltip(Dice dice, int faceIndex)
    {
        if (dice == null) return;

        currentMode = TooltipMode.DiceResult;
        HideAllSubTooltips();
        transform.position = Mouse.current.position.value;

        diceResultTooltip.SetActive(true);
        if(faceIndex == -1)
        {
            faceView.faceText.text = "?";
        }
        else
        {
            faceView.Set(this, dice.faces[faceIndex], faceIndex);
        }

        if (effectsContainer != null && effectItemPrefab != null)
        {
            foreach (Transform child in effectsContainer)
                Destroy(child.gameObject);


            if (dice.effects.Count > 0)
            {
                effectRect.gameObject.SetActive(true);
                foreach (var effect in dice.effects)
                {
                    var item = Instantiate(effectItemPrefab, effectsContainer);
                    item.GetComponent<EffectItemView>().Set(effect);
                }
            }
            else
            {
                effectRect.gameObject.SetActive(false);
            }
        }

        gameObject.SetActive(true);
    }

    public void ShowHandSlotTooltip(HandSlot slot)
    {
        currentMode = TooltipMode.Hand;
        HideAllSubTooltips();
        transform.position = Mouse.current.position.value;

        handTooltip.SetActive(true);
        handNameText.text = slot.hand.Name;
        handDescriptionText.text = slot.hand.Description;

        handLevelText.text = string.Format(c_levelTextFormat, slot.slotLevel);
        handMultiplierText.text = string.Format(StringTable.GetString(c_handMultiplierFormatStringKey), slot.hand.ScoreMultiplier);
        handAchievedText.text = slot.hand.IsAchived() ? StringTable.GetString(c_handAchivedSuccessStringKey) : StringTable.GetString(c_handAchivedFailStringKey);

        gameObject.SetActive(true);
    }

    public void ShowCardTooltip(Card card)
    {
        if(card == null) return;

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
        if(relic == null) return;

        currentMode = TooltipMode.Relic;
        HideAllSubTooltips();
        transform.position = Mouse.current.position.value;

        relicTooltip.SetActive(true);
        relicNameText.text = relic.Name;
        relicDescText.text = relic.Description;
        relicFlavorText.text = relic.FlavorText;
        relicImage.sprite = relic.sprite;

        gameObject.SetActive(true);
    }

    public void ShowConsumableTooltip(Consumable consumable)
    {
        if(consumable == null) return;

        currentMode = TooltipMode.Consumable;
        HideAllSubTooltips();
        transform.position = Mouse.current.position.value;

        consumableTooltip.SetActive(true);
        consumableNameText.text = consumable.Name;
        consumableDescriptionText.text = consumable.Description;
        consumableFlavorText.text = consumable.FlavorText;
        consumableImage.sprite = consumable.sprite;

        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}
