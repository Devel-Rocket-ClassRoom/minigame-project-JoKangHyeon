using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class AlchemistsTouchstone : Relic
{
    GameManager gameManager;
    public int eventPriorityOnFirstRollComplete = 20;


    public override void OnAdd(GameManager gameManager)
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

[Serializable]
public class CrackedRune : Relic
{
    public override void OnAdd(GameManager gameManager)
    {
        throw new NotImplementedException();
    }

    public override void OnRemove()
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class FortuneTellersFinger : Relic
{
    public override void OnAdd(GameManager gameManager)
    {
        throw new NotImplementedException();
    }

    public override void OnRemove()
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class CrystalOrb : Relic
{
    public override void OnAdd(GameManager gameManager)
    {
        throw new NotImplementedException();
    }
    public override void OnRemove()
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class ThreadOfFate : Relic
{
    public override void OnAdd(GameManager gameManager)
    {
        throw new NotImplementedException();
    }
    public override void OnRemove()
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class StarChalice : Relic
{
    public override void OnAdd(GameManager gameManager)
    {
        throw new NotImplementedException();
    }
    public override void OnRemove()
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class GoldenMirror : Relic
{
    public override void OnAdd(GameManager gameManager)
    {
        throw new NotImplementedException();
    }
    public override void OnRemove()
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class WatchGlass : Relic
{
    public override void OnAdd(GameManager gameManager)
    {
        throw new NotImplementedException();
    }
    public override void OnRemove()
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class BrokenChain : Relic
{
    public override void OnAdd(GameManager gameManager)
    {
        throw new NotImplementedException();
    }
    public override void OnRemove()
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class ObsidianScales : Relic
{
    public override void OnAdd(GameManager gameManager)
    {
        throw new NotImplementedException();
    }
    public override void OnRemove()
    {
        throw new NotImplementedException();
    }
}

