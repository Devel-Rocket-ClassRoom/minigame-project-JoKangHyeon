using System;

[Serializable]
public class WitchsVial : Consumable
{
    protected override Consumable CloneInstance() => new WitchsVial();

    public override bool OnUse(GameManager gameManager)
    {
        var round = gameManager.currentRun?.currentRound;
        if (round?.currentCycle == null) return false;

        // 임시 Choice 슬롯 — currentRound.hands에만 추가하므로 다음 Round Init 시 자동 제거
        var handDefine = gameManager.handDefine;
        var choiceHand = handDefine.Find("Hand_Choice_Name");
        if (choiceHand == null) return false;

        var newSlot = new HandSlot
        {
            hand = choiceHand,
            slotLevel = 1,
        };
        newSlot.hand.slot = newSlot;
        round.hands.Add(newSlot);

        gameManager.currentRun.GetReroll(1);
        gameManager.RefreshUI();
        return true;
    }
}
