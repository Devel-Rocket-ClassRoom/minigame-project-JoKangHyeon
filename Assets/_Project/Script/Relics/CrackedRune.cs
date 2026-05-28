using System;
using System.Collections.Generic;

[Serializable]
public class CrackedRune : Relic
{
    GameManager gameManager;

    public override void OnObtain(GameManager gameManager)
    {
        this.gameManager = gameManager;
        EventBus.Subscribe<List<Dice>>(EventType.OnRollComplete, Effect, c_priorityBrokenRune);
    }

    public override void OnRemove()
    {
        EventBus.Unsubscribe<List<Dice>>(EventType.OnRollComplete, Effect);
    }

    void Effect(List<Dice> dices)
    {
        foreach (var dice in dices)
        {
            if (dice.GetDice() == 1)
            {
                dice.TrySetDice(6);
            }
        }
    }
}
