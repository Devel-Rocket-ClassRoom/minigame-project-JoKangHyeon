using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardDefinition")]
public class CardDefinitionSO : ScriptableObject
{
    [SerializeReference]
    [SerializeReferenceDropdown]
    public List<Card> cards;

    public Card Find(string name)
    {
        foreach (Card c in cards)
        {
            if (c.name == name) return c.Clone();
        }
        return null;
    }
}
