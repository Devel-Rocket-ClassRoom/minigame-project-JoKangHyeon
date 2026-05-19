using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class AlchemistsTouchstone : Relic
{
    GameManager gameManager;

    public override void OnAdd(GameManager gameManager)
    {
        this.gameManager = gameManager;
        EventBus.Subscribe<List<Dice>>(EventType.OnFirstRollComplete, Effect);
    }

    public override void OnRemove()
    {
        EventBus.Unsubscribe<List<Dice>>(EventType.OnFirstRollComplete, Effect);
    }

    void Effect(List<Dice> dices)
    {
        var target = dices[Random.Range(0, dices.Count)];
        int targetValue = target.GetDice() + 1;

        Debug.Log("AlchemistsTouchstone: " + target.name + " is increased by 1 from " + target.GetDice() + " to " + targetValue);
        target.TrySetDice(targetValue);
    }
}