using System;

[Serializable]
public class MadWhirlwind : Consumable
{
    protected override Consumable CloneInstance() => new MadWhirlwind();

    public override bool OnUse(GameManager gameManager)
    {
        var cycle = gameManager.currentRun?.currentRound?.currentCycle;
        if (cycle == null) return false;

        cycle.freeRerollActive = true;
        gameManager.RefreshUI();
        return true;
    }
}
