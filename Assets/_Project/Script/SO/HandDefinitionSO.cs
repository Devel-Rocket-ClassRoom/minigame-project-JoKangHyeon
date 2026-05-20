using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName= "HandDefinition")]
public class HandDefinitionSO : ScriptableObject
{
    [SerializeReferenceDropdown]
    [SerializeReference]
    public List<HandData> hands;

    public Hand Find(string name)
    {
        foreach (HandData hand in hands)
        {
            if (hand.name == name)
                return hand.GetHand();
        }
        return null;
    }
}


[Serializable]
public abstract class HandData
{
    public string name;
    public string desc;
    public float multiplier;

    public virtual Hand GetHand()
    {
        Hand hand = CreateBaseHand();
        hand.name = name;
        hand.scoreMultiplier = multiplier;
        hand.description = desc;
        return hand;
    }

    protected abstract Hand CreateBaseHand();
}

[Serializable]
public class NumbersHandData : HandData
{
    public int numTarget;
    protected override Hand CreateBaseHand()
    {
        return new NumbersHand(numTarget);
    }
}

[Serializable]
public class ChoiceHandData : HandData
{
    protected override Hand CreateBaseHand()
    {
        return new ChoiceHand();
    }
}

[Serializable]
public class FullHouseHandData : HandData
{
    protected override Hand CreateBaseHand()
    {
        return new FullHouseHand();
    }
}

[Serializable]
public class SmallAlignmentData : HandData
{
    protected override Hand CreateBaseHand()
    {
        return new SmallAlignmentHand();
    }
}

[Serializable]
public class BigAlignmentData : HandData
{
    protected override Hand CreateBaseHand()
    {
        return new BigAlignmentHand();
    }
}

[Serializable]
public class SmallStraightHandData : HandData
{
    protected override Hand CreateBaseHand()
    {
        return new SmallStraightHand();
    }
}

[Serializable]
public class BigStraightHandData : HandData
{
    protected override Hand CreateBaseHand()
    {
        return new BigStraightHand();
    }
}