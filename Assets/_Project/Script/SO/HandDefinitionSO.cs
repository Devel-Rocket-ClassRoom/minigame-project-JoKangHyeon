using UnityEngine;


[CreateAssetMenu(menuName= "HandDefinition")]
public class HandDefinitionSO : ScriptableObject
{

}

[CreateAssetMenu(menuName = "StartingDefinition")]
public class StartingDefinitionSO : ScriptableObject
{

}

public class Defines
{
    public enum HandType
    {
        None,
        Numbers,
        Choice,
        FullHouse,
        SmallStraight,
        BigStraight,
        SmallAlignment,
        LargeAlignment,
    }

    public enum RelicTiming
    {
        None,
        OnRoll,

    }
}