using UnityEngine;

public abstract class Consumable
{
    public string name;
    public string description;

    public abstract void OnUse();
    public virtual void OnAdd() { }
    public virtual void OnRemove() { }
}
