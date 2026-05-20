
using System;

[Serializable]
public abstract class Card
{
    public string name;
    public string description;

    public abstract Card Clone();
}