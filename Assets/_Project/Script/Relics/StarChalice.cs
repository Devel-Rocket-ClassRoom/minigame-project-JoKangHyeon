using System;

[Serializable]
public class StarChalice : Relic
{
    const int multiplierPerLevel = 5;

    GameManager gameManager;

    public override void OnObtain(GameManager gameManager)
    {
        this.gameManager = gameManager;
        EventBus.Subscribe<HandSlot>(EventType.OnSlotScored, Effect, c_priorityDefault);
    }

    public override void OnRemove()
    {
        EventBus.Unsubscribe<HandSlot>(EventType.OnSlotScored, Effect);
    }

    void Effect(HandSlot slot)
    {
        slot.currentScore += slot.slotLevel * multiplierPerLevel;
    }
}
