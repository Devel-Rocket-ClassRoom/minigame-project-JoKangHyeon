using System;

[Serializable]
public class RewindingSands : Consumable
{
    protected override Consumable CloneInstance() => new RewindingSands();

    public override bool OnUse(GameManager gameManager)
    {
        var round = gameManager.currentRun?.currentRound;
        if (round == null) return false;

        // 점수화된 슬롯이 1개 이상 있어야 발동 가능
        bool anyUsed = false;
        foreach (var slot in round.hands)
            if (slot.isUsed) { anyUsed = true; break; }
        if (!anyUsed) return false;

        gameManager.StartHandSelect(
            slot =>
            {
                if (slot == null) return;
                // 누적 점수에서 해당 슬롯 점수 회수
                gameManager.currentRun.currentScore -= slot.currentScore;
                // 슬롯 비점수화 — IsUsed()가 false로 전환되어 추가 Cycle 발생
                slot.ResetSlot();
                gameManager.RefreshUI();
            },
            slot => slot.isUsed
        );

        return true;
    }
}
