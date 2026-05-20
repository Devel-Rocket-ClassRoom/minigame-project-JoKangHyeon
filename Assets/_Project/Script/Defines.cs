public class Defines
{
    public const int c_maxPriority = 9999;

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