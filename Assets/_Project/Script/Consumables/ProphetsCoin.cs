using System;
using System.Collections.Generic;

[Serializable]
public class ProphetsCoin : Consumable
{
    protected override Consumable CloneInstance() => new ProphetsCoin();

    public override bool OnUse(GameManager gameManager)
    {
        if (gameManager.currentRun?.currentRound?.currentCycle == null) return false;

        gameManager.StartDiceObjectSelect(target =>
        {
            gameManager.StartDiceFaceSelect(target.dice, (dice, value) =>
            {
                dice.SetDice(value);
                dice.AddEffect(new EffectView
                {
                    name = "effect_consumable_prophetscoin_name",
                    description = "effect_consumable_prophetscoin_desc",
                    isPermanent = false,
                    targetFaceValue = dice.GetFace().Value
                });
                gameManager.StartCoroutine(gameManager.rollManager.DeterministicRoll(new List<DiceObject> { target }));
                gameManager.RefreshUI();
            });
        });

        return true;
    }
}
