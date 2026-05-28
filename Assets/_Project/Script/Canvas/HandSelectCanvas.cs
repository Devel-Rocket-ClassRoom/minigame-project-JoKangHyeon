using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 족보 슬롯 선택 UI 캔버스. DiceSelectCanvas 패턴 미러.
/// CardS0·W4·W6·M8·M1 등 슬롯 대상이 필요한 카드가 사용한다.
/// </summary>
public class HandSelectCanvas : MonoBehaviour
{
    public List<HandSelectButton> handSelectors;
    public HandSelectButton handSelectorPrefab;
    public Transform handSelectorParent;

    /// <summary>
    /// 족보 선택 UI를 열고 슬롯 버튼을 구성한다.
    /// </summary>
    /// <param name="gameManager">현재 GameManager</param>
    /// <param name="callback">슬롯 선택 시 호출</param>
    /// <param name="filter">null이면 전체 활성. 반환 false인 슬롯은 회색 비활성.</param>
    public void StartHandSelect(GameManager gameManager, Action<HandSlot> callback, Func<HandSlot, bool> filter = null)
    {
        List<HandSlot> slots = gameManager.currentRun.hands;

        // 기존 버튼 비활성화
        foreach (HandSelectButton btn in handSelectors)
        {
            btn.gameObject.SetActive(false);
        }

        for (int i = 0; i < slots.Count; i++)
        {
            if (i >= handSelectors.Count)
            {
                HandSelectButton newBtn = Instantiate(handSelectorPrefab, handSelectorParent);
                handSelectors.Add(newBtn);
            }

            bool interactable = filter == null || filter(slots[i]);
            handSelectors[i].Show(slots[i], callback, this, interactable);
            handSelectors[i].gameObject.SetActive(true);
        }

        gameObject.SetActive(true);
    }

    public void FinishHandSelect()
    {
        gameObject.SetActive(false);
    }
}
