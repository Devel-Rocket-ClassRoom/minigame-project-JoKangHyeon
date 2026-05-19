using UnityEngine;

public class HandSlot
{
    public Hand hand;
    public int slotLevel;
    public int currentScore;


    public bool isUsed => hand.IsUsed();

    public HandSlot()
    {
        EventBus.Subscribe<HandSlot>(EventType.OnSlotScore, OnHandScored);
    }

    public void OnHandScored(HandSlot slot)
    {
        if(slot != this) return;
        var result = hand.GetResult();
        currentScore = result.baseScore;
    }

    public void ResetSlot()
    {
        hand?.ResetHand();
        currentScore = 0;
    }
}
