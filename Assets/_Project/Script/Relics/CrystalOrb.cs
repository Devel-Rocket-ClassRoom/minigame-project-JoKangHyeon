using System;

[Serializable]
public class CrystalOrb : Relic
{
    const int bonus = 15;

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
        if (slot.hand is ChoiceHand)
        {
            slot.currentScore += bonus;
        }
    }
}
