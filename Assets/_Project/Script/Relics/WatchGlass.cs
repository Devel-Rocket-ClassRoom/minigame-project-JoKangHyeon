using System;

[Serializable]
public class WatchGlass : Relic
{
    const int rerollBonus = 1;

    public override void OnObtain(GameManager gameManager)
    {
        gameManager.currentRun.rerollPerCycle += rerollBonus;
    }

    public override void OnRemove()
    {
        // 유물은 제거되지 않는 정책. RestartGame 시 RunState 재생성으로 자동 리셋.
    }
}
