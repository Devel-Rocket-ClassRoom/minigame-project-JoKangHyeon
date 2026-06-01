using System;
using System.Collections.Generic;

[Serializable]
public class AlchemistsDust : Consumable
{
    protected override Consumable CloneInstance() => new AlchemistsDust();

    public override bool OnUse(GameManager gameManager)
    {
        var round = gameManager.currentRun?.currentRound;
        if (round == null) return false;

        var diceDefine = gameManager.diceDefine;
        if (diceDefine == null || diceDefine.dices == null || diceDefine.dices.Count == 0) return false;

        // 표준 주사위(dices[0]) 복제 — currentRound.dices에만 추가하므로 다음 Round Init 시 자동 제거
        Dice tempDice = diceDefine.dices[0].Clone();
        tempDice.AddEffect(new EffectView
        {
            name = "effect_consumable_alchemistsdust_name",
            description = "effect_consumable_alchemistsdust_desc",
            isPermanent = false,
            targetFaceValue = -1
        });
        round.dices.Add(tempDice);
        round.currentCycle.AddDice(new List<Dice>{ tempDice });

        gameManager.RefreshUI();
        return true;
    }
}
