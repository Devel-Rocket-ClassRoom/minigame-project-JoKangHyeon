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
}
