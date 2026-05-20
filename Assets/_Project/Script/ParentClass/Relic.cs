using System;

[Serializable]
public abstract class Relic
{
    public string name;
    public string description;
    public string flavorText;

    public abstract void OnAdd(GameManager gameManager);
    public abstract void OnRemove();
}