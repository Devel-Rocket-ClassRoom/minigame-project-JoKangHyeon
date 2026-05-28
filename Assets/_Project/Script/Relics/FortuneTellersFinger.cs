using System;
using UnityEngine;

[Serializable]
public class FortuneTellersFinger : Relic
{
    GameManager gameManager;
    bool usedThisRound;

    public override void OnObtain(GameManager gameManager)
    {
        this.gameManager = gameManager;
        usedThisRound = false;
        EventBus.Subscribe<object>(EventType.OnRoundStart, ResetUsage, c_priorityDefault);
    }

    public override void OnRemove()
    {
        EventBus.Unsubscribe<object>(EventType.OnRoundStart, ResetUsage);
    }

    void ResetUsage(object _)
    {
        usedThisRound = false;
    }

    /// <summary>
    /// UI에서 호출. Round당 1회 제한. 게임플레이 Cycle 밖에서 호출 시 false 반환.
    /// </summary>
    public bool TryActivate()
    {
        if (usedThisRound)
        {
            Debug.Log("FortuneTellersFinger: 이미 이번 Round에 사용했습니다.");
            return false;
        }

        // 상점 진입 중이거나 Cycle이 없는 경우 발동 차단
        if (gameManager.currentRun?.currentRound?.currentCycle == null)
        {
            Debug.Log("FortuneTellersFinger: 현재 발동할 수 없는 상태입니다.");
            return false;
        }

        gameManager.StartDiceSelect(target =>
        {
            gameManager.StartDiceFaceSelect(target, (dice, value) =>
            {
                dice.TrySetDice(value);
                usedThisRound = true;
                gameManager.RefreshUI();
                Debug.Log($"FortuneTellersFinger: {dice.name}의 면값을 {value}로 변경했습니다.");
            });
        });

        return true;
    }
}
