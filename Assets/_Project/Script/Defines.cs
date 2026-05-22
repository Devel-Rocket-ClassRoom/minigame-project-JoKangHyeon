using UnityEngine;

public class Defines
{
    public const int c_maxPriority = 9999;

    //#C9A14B
    public static readonly Color colorGold = new Color(0.7882353f, 0.6313726f, 0.2941177f);
    //#E8DDC2
    public static readonly Color colorPaper = new Color(0.9098039f, 0.8666667f, 0.7607843f);

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

    public enum Rarity
    {
        Common, 
        Rare, 
        Epic
    }

    public enum RelicCategory
    {
        DiceManipulation, 
        ScoreBoost, 
        Resource
    }
}