using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class AlchemistsTouchstone : Relic
{
    GameManager gameManager;
    public int eventPriorityOnFirstRollComplete = 20;


    public override void OnObtain(GameManager gameManager)
    {
        this.gameManager = gameManager;
        EventBus.Subscribe<List<Dice>>(EventType.OnFirstRollComplete, Effect, eventPriorityOnFirstRollComplete);
    }

    public override void OnRemove()
    {
        EventBus.Unsubscribe<List<Dice>>(EventType.OnFirstRollComplete, Effect);
    }

    void Effect(List<Dice> dices)
    {
        int randomIndex = Random.Range(0, dices.Count);
        var target = dices[randomIndex];
        int targetValue = target.GetDice() + 1;

        Debug.Log("AlchemistsTouchstone: " + target.name + " is increased by 1 from " + target.GetDice() + " to " + targetValue);
        target.TrySetDice(targetValue);

        gameManager.SetRemianDiceEffect(randomIndex);
    }
}

