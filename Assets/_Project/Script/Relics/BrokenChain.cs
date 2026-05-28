using System;
using System.Linq;

[Serializable]
public class BrokenChain : Relic
{
    const int clearBonus = 3;
    const int fullClearBonus = 6;

    GameManager gameManager;

    public override void OnObtain(GameManager gameManager)
    {
        this.gameManager = gameManager;
        EventBus.Subscribe<object>(EventType.OnRoundClear, Effect, c_priorityDefault);
    }

    public override void OnRemove()
    {
        EventBus.Unsubscribe<object>(EventType.OnRoundClear, Effect);
    }

    void Effect(object _)
    {
        gameManager.currentRun.Coin += clearBonus;

        bool fullClear = gameManager.currentRun.currentRound.hands.All(h => h.hand.IsUsed());
        if (fullClear)
        {
            gameManager.currentRun.Coin += fullClearBonus;
        }
    }
}
