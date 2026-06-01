using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ConsumableDefinition")]
public class ConsumableDefinitionSO : ScriptableObject
{
    [SerializeReference]
    [SerializeReferenceDropdown]
    public List<Consumable> consumables;

    public Consumable Find(string consumableName)
    {
        foreach (var c in consumables)
            if (c.nameStringKey == consumableName) return c.Clone();
        return null;
    }
}
