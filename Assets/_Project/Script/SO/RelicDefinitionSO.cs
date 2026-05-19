using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RelicDefinition")]
public class RelicDefinitionSO : ScriptableObject
{
    [SerializeReference]
    [SerializeReferenceDropdown]
    public List<Relic> relics;


    public Relic Find(string name)
    {
        foreach (Relic r in relics)
        {
            if (r.name == name) return r;
        }
        return null;
    }
}
