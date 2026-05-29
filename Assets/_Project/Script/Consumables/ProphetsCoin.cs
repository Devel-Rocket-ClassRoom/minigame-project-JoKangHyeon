using System;

[Serializable]
public class ProphetsCoin : Consumable
{
    protected override Consumable CloneInstance() => new ProphetsCoin();

    public override bool OnUse(GameManager gameManager)
    {
        if (gameManager.currentRun?.currentRound?.currentCycle == null) return false;

        gameManager.StartDiceSelect(target =>
        {
            gameManager.StartDiceFaceSelect(target, (dice, value) =>
            {
                dice.TrySetDice(value);
                gameManager.RefreshUI();
            });
        });

        return true;
    }
}
