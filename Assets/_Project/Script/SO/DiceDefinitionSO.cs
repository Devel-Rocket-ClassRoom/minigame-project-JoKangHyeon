using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DiceDefinition")]
public class DiceDefinitionSO : ScriptableObject
{
    [SerializeReference]
    [SerializeReferenceDropdown]
    public List<Dice> dices;

    public Dice Find(string name)
    {
        foreach (Dice d in dices)
        {
            if(d.nameStringKey == name)
            {
                Dice dice =  d.Clone();
                dice.InitFaces();
                return dice;
            }
        }
        return null;
    }
}
