using System;

[Serializable]
public abstract class Relic
{
    public enum RelicCategory
    {
        dice,
        score,
        resource
    }

    public string name;
    public string description;
    public string flavorText;
    public Defines.Rarity rarity;
    public int cost;
    public RelicCategory category;

    public abstract void OnAdd(GameManager gameManager);
    public abstract void OnRemove();
}