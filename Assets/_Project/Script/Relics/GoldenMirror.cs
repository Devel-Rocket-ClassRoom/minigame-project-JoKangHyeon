using System;

[Serializable]
public class GoldenMirror : Relic
{
    const int multiplier = 2;

    GameManager gameManager;

    public override void OnObtain(GameManager gameManager)
    {
        this.gameManager = gameManager;
        EventBus.Subscribe<HandSlot>(EventType.OnFirstScoreOfRound, Effect, c_priorityGoldenMirror);
    }

    public override void OnRemove()
    {
        EventBus.Unsubscribe<HandSlot>(EventType.OnFirstScoreOfRound, Effect);
    }

    void Effect(HandSlot slot)
    {
        slot.currentScore *= multiplier;
    }
}
