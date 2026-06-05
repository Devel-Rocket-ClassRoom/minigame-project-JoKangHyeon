using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 족보 선택 UI의 개별 슬롯 버튼. HandSelectCanvas가 생성/관리.
/// </summary>
public class HandSelectButton : MonoBehaviour
{
    public TextMeshProUGUI handNameText;
    public TextMeshProUGUI slotLevelText;
    public TextMeshProUGUI handDescriptionText;

    public Image background;
    public Button button;

    HandSelectPanel parentCanvas;
    Action<HandSlot> callback;
    HandSlot handSlot;


    /// <summary>
    /// 버튼 내용을 설정한다.
    /// </summary>
    /// <param name="slot">표시할 HandSlot</param>
    /// <param name="cb">선택 시 콜백</param>
    /// <param name="canvas">부모 캔버스 (닫기용)</param>
    /// <param name="interactable">false면 회색 비활성 (filter 미충족)</param>
    public void Show(HandSlot slot, Action<HandSlot> cb, HandSelectPanel canvas, bool interactable)
    {
        handSlot = slot;
        callback = cb;
        parentCanvas = canvas;

        handNameText.text = slot.hand != null ? slot.hand.Name : "—";
        handDescriptionText.text = slot.hand != null ? slot.hand.Description : "-";
        slotLevelText.text = $"Lv {slot.slotLevel}";

        button.interactable = interactable;
        if (background != null)
            background.color = interactable ? Color.white : Color.gray;
    }

    public void OnButtonClicked()
    {
        callback?.Invoke(handSlot);
        parentCanvas.FinishHandSelect();
    }
}
