using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StartingDefinition")]
public class StartingDefinitionSO : ScriptableObject
{
    [SerializeField]
    public List<Starting> startings;

    public Starting Find(string name)
    {
        foreach (Starting s in startings)
        {
            if(s.name == name) 
                return s;
        }
        return null;
    }
}
