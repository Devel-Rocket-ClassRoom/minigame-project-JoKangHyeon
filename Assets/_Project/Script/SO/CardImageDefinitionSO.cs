using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardImageDefinition")]
public class CardImageDefinitionSO : ScriptableObject
{
    [Serializable]
    public class CardImage
    {
        public Sprite image;
        public List<int> levels;
    }

    public List<CardImage> cardImages;

    public Sprite Find(int level)
    {
        foreach (CardImage cardImage in cardImages)
        {
            if(cardImage.levels.Contains(level))
            {
                return cardImage.image;
            }
        }
        return null;
    }
}
