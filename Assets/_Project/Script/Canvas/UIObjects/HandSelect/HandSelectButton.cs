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
    public Image background;

    HandSelectCanvas parentCanvas;
    Button button;
    Action<HandSlot> callback;
    HandSlot handSlot;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    /// <summary>
    /// 버튼 내용을 설정한다.
    /// </summary>
    /// <param name="slot">표시할 HandSlot</param>
    /// <param name="cb">선택 시 콜백</param>
    /// <param name="canvas">부모 캔버스 (닫기용)</param>
    /// <param name="interactable">false면 회색 비활성 (filter 미충족)</param>
    public void Show(HandSlot slot, Action<HandSlot> cb, HandSelectCanvas canvas, bool interactable)
    {
        handSlot = slot;
        callback = cb;
        parentCanvas = canvas;

        handNameText.text = slot.hand != null ? slot.hand.name : "—";
        slotLevelText.text = $"Lv {slot.slotLevel}";

        button.interactable = interactable;
        if (background != null)
            background.color = interactable ? Color.white : Color.gray;
    }

    private void OnButtonClicked()
    {
        callback?.Invoke(handSlot);
        parentCanvas.FinishHandSelect();
    }
}
