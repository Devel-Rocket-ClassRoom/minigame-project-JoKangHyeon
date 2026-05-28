using UnityEngine;

public class HandSlot
{
    public Hand hand;
    public int slotLevel;
    public int currentScore;

    public bool isUsed => hand.IsUsed();


    public void SetCurrentScore()
    {
        var result = hand.GetResult();
        currentScore = result.baseScore;
    }

    public void ResetSlot()
    {
        hand?.ResetHand();
        currentScore = 0;
    }

    public HandSlot Clone()
    {
        var clone = new HandSlot
        {
            hand = hand.Clone(),
            slotLevel = slotLevel,
        };
        return clone;
    }
}
