using System;

[Serializable]
public class ThreadOfFate : Relic
{
    const float multiplierBonus = 1f;

    public override void OnObtain(GameManager gameManager)
    {
        foreach (var slot in gameManager.currentRun.hands)
        {
            if (IsTargetHand(slot.hand))
            {
                slot.hand.baseScoreMultiplier += multiplierBonus;
            }
        }
    }

    public override void OnRemove()
    {
        // 유물은 제거되지 않는 정책. RestartGame 시 RunState.Setup에서 새 인스턴스 생성으로 자동 리셋.
    }

    bool IsTargetHand(Hand h)
    {
        return h is FullHouseHand || h is SmallAlignmentHand || h is BigAlignmentHand;
    }
}
