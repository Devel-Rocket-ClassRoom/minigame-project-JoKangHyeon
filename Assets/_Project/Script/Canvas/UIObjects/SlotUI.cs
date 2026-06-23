
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class SlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI diceText;
    public TextMeshProUGUI scoreText;
    public Button setButton;
    public Image bgImage;

    public bool detailView = false;

    private HandSlot slot;
    private GameManager gameManager;

    private List<Dice> currentTemp;

    private AudioClip selectClip;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        
    }

    public bool IsUsed()
    {
        return slot.isUsed;
    }

    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;

        selectClip = gameManager.soundDefine.Find(Defines.c_assignSFXKey);
        audioSource.clip = selectClip;
    }

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

        nameText.text = slot.hand.Name;
        diceText.text = slot.hand.GetDicesString();
        currentTemp = tempView;
        bgImage.sprite = gameManager.cardImageDefine.Find(slot.slotLevel);

        if (slot.hand.Setted)
        {
            scoreText.color = Defines.colorGold;
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
            scoreText.color = Defines.colorPaper;
            
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        gameManager.tooltip.ShowHandSlotTooltip(slot);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameManager.tooltip.HideTooltip();
    }

}
